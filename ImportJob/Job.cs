using Dapper;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Configuration;
using Microsoft.SqlServer.Server;
using Microsoft.WindowsAzure.Storage.Blob;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Audiobooks.ImportJob
{
	public class Job : IDisposable
	{
		private readonly HttpClient _httpClient;
		private readonly DbConnection _dbConnection;
		private IBinder _binder;

		public Job(IConfiguration configuration)
		{
			_httpClient = new HttpClient();
			_dbConnection = new SqlConnection(configuration.GetConnectionString("Database"));
			_dbConnection.Open();
		}

		public async Task RunAsync([TimerTrigger("%ImportJob:TimerInterval%", RunOnStartup = false)] TimerInfo timerInfo, IBinder binder)
		{
			Console.WriteLine(timerInfo.Schedule);
			Console.WriteLine($"Last schedule occurrence: {timerInfo.ScheduleStatus.Last}");

			_binder = binder;

			var startDateTime = DateTime.UtcNow;
			var importedCount = 0;
			var failedCount = 0;

			await _dbConnection.ExecuteAsync("DELETE FROM [Author] WHERE [Id] NOT IN (SELECT [AuthorId] FROM [Audiobook2Author]); DELETE FROM [Genre] WHERE [Id] NOT IN (SELECT [GenreId] FROM [Audiobook2Genre]);");

			foreach (var audiobookId in await GetAudiobookIdSetForImportAsync())
			{
				try
				{
					if (await DeleteAudiobookIfExistsAsync(audiobookId))
					{
						importedCount--;
					}

					var audiobook = await GetAudiobookAsync(audiobookId);
					var invalidOptionalFieldSet = await SaveAudiobookAsync(audiobook);

					importedCount++;

					if (invalidOptionalFieldSet.Any())
					{
						throw new InvalidOptionalFieldsException(invalidOptionalFieldSet.ToArray());
					}

					await _dbConnection.ExecuteAsync(sql: "DELETE FROM [ImportJobFailure] WHERE [AudiobookId] = @AudiobookId", param: new { AudiobookId = audiobookId });
				}
				catch (Exception ex)
				{
					await _dbConnection.UpsertImportJobFailureAsync(audiobookId, ex);
					failedCount++;
				}
			}

			await _dbConnection.ExecuteAsync(
				sql: "INSERT INTO [ImportJobHistory] ([StartDateTime], [ImportedCount], [FailedCount]) VALUES (@StartDateTime, @ImportedCount, @FailedCount)",
				param: new
				{
					StartDateTime = startDateTime,
					ImportedCount = importedCount,
					FailedCount = failedCount
				});

			Console.WriteLine($"Imported: {importedCount}, Failed: {failedCount}");
			Console.WriteLine($"Expected next schedule occurrence: {timerInfo.Schedule.GetNextOccurrence(DateTime.Now)}");
		}

		private async Task<HashSet<int>> GetAudiobookIdSetForImportAsync()
		{
			var libriVoxlimit = 1000;
			var libriVoxOffset = 0;
			var lastStartDateTime = await _dbConnection.QuerySingleOrDefaultAsync<DateTime?>("SELECT TOP 1 [StartDateTime] FROM [ImportJobHistory] ORDER BY [StartDateTime] DESC");
			var libriVoxSince = lastStartDateTime.HasValue
				? ((DateTimeOffset)DateTime.SpecifyKind(lastStartDateTime.Value, DateTimeKind.Utc)).ToUnixTimeSeconds()
				: 0;
			var audiobookIdSet = new HashSet<int>(await _dbConnection.QueryAsync<int>("SELECT [AudiobookId] FROM [ImportJobFailure]"));

			while (true)
			{
				var libriVoxUrl = $"https://librivox.org/api/feed/audiobooks/?format=json&limit={libriVoxlimit}&offset={libriVoxOffset}&since={libriVoxSince}&fields={{id}}";

				using (var response = await _httpClient.GetAsync(libriVoxUrl))
				{
					if (response.IsSuccessStatusCode)
					{
						var jObject = JsonConvert.DeserializeObject<JObject>(await response.Content.ReadAsStringAsync());
						var audiobookIds = jObject["books"].Values<int>("id").Where(audiobookId => audiobookId > 0);

						audiobookIdSet.UnionWith(audiobookIds);

						libriVoxOffset += libriVoxlimit;
					}
					else if (response.StatusCode == HttpStatusCode.NotFound)
					{
						break;
					}
					else
					{
						throw new Exception($"Unexpected status code when call LibriVox url '{libriVoxUrl}'");
					}
				}
			}

			return audiobookIdSet;
		}

		private async Task<Audiobook> GetAudiobookAsync(int audiobookId)
		{
			var libriVoxUrl = $"https://librivox.org/api/feed/audiobooks/?format=json&id={audiobookId}&extended=1&fields={{id,title,language,totaltime,totaltimesecs,authors,genres,url_iarchive}}";
			var jObject = JsonConvert.DeserializeObject<JObject>(await _httpClient.GetStringAsync(libriVoxUrl));
			Audiobook audiobook;

			try
			{
				audiobook = jObject["books"].ToObject<Dictionary<int, Audiobook>>().Values.Single();
			}
			catch (JsonSerializationException)
			{
				audiobook = jObject["books"].ToObject<Audiobook[]>().Single();
			}

			try
			{
				var archiveXmlUrl = $"{audiobook.ArchiveBaseUrl}/{audiobook.ArchiveBaseUrl.Split('/', StringSplitOptions.RemoveEmptyEntries).Last()}_files.xml";
				var root = XElement.Parse(await _httpClient.GetStringAsync(archiveXmlUrl));
				var elements = root.Elements("file")
					.Select(el => new
					{
						format = el.Element("format")?.Value,
						fileName = el.Attribute("name")?.Value
					})
					.ToArray();
				audiobook.TrackUrlSet.UnionWith(elements
					.Where(el => string.Equals(el.format, "Ogg Vorbis", StringComparison.OrdinalIgnoreCase))
					.Select(el => Uri.EscapeUriString($"{audiobook.ArchiveBaseUrl}/{el.fileName}".Trim())));
				audiobook.ArchiveImgUrl = elements
					.Where(el => string.Equals(el.format, "JPEG", StringComparison.OrdinalIgnoreCase))
					.Select(el => Uri.EscapeUriString($"{audiobook.ArchiveBaseUrl}/{el.fileName}".Trim()))
					.FirstOrDefault();
			}
			catch
			{
				// ignored
			}

			var invalidRequiredFieldSet = new HashSet<string>();

			if (audiobook.Id <= 0 || audiobook.Id != audiobookId)
			{
				invalidRequiredFieldSet.Add(audiobook.GetJsonPropertyName(nameof(audiobook.Id)));
			}

			if (audiobook.Title == null)
			{
				invalidRequiredFieldSet.Add(audiobook.GetJsonPropertyName(nameof(audiobook.Title)));
			}

			if (audiobook.Language == null)
			{
				invalidRequiredFieldSet.Add(audiobook.GetJsonPropertyName(nameof(audiobook.Language)));
			}

			if (audiobook.ArchiveBaseUrl == null)
			{
				invalidRequiredFieldSet.Add(audiobook.GetJsonPropertyName(nameof(audiobook.ArchiveBaseUrl)));
			}

			if (!audiobook.TrackUrlSet.Any() || audiobook.TrackUrlSet.Any(trackUrl => !trackUrl.IsWellFormedUriString()))
			{
				invalidRequiredFieldSet.Add("Ogg Vorbis");
			}

			if (invalidRequiredFieldSet.Any())
			{
				throw new InvalidRequiredFieldsException(invalidRequiredFieldSet.ToArray());
			}

			return audiobook;
		}

		private async Task<HashSet<string>> SaveAudiobookAsync(Audiobook audiobook)
		{
			var invalidOptionalFieldSet = new HashSet<string>();

			if (audiobook.TotalTime == null)
			{
				invalidOptionalFieldSet.Add(audiobook.GetJsonPropertyName(nameof(audiobook.TotalTime)));
			}

			if (audiobook.TotalTimeSecs == null)
			{
				invalidOptionalFieldSet.Add(audiobook.GetJsonPropertyName(nameof(audiobook.TotalTimeSecs)));
			}

			if (!audiobook.AuthorSet.Any() || audiobook.AuthorSet.RemoveWhere(author => author == null || author.Id <= 0 || author.LastName == null) > 0)
			{
				invalidOptionalFieldSet.Add(audiobook.GetJsonPropertyName(nameof(audiobook.AuthorSet)));
			}

			if (!audiobook.GenreSet.Any() || audiobook.GenreSet.RemoveWhere(genre => genre == null || genre.Id <= 0 || genre.Name == null) > 0)
			{
				invalidOptionalFieldSet.Add(audiobook.GetJsonPropertyName(nameof(audiobook.GenreSet)));
			}

			audiobook.ImgUrl = await UploadAudiobookImageAsync(audiobook.ArchiveImgUrl);
			if (audiobook.ImgUrl == null)
			{
				invalidOptionalFieldSet.Add("JPEG");
			}

			try
			{
				foreach (var genre in audiobook.GenreSet)
				{
					genre.Id = await _dbConnection.QuerySingleOrDefaultAsync<int?>(sql: "SELECT [Id] FROM [Genre] WHERE [Name] = @Name", param: new { Name = genre.Name }) ?? genre.Id;
				}

				await _dbConnection.ExecuteAsync(
					sql: "[sp_ImportJob]",
					param: new
					{
						AudiobookId = audiobook.Id,
						AudiobookTitle = audiobook.Title,
						AudiobookImgUrl = audiobook.ImgUrl,
						AudiobookLanguage = audiobook.Language,
						AudiobookTotalTime = audiobook.TotalTime,
						AudiobookTotalTimeSecs = audiobook.TotalTimeSecs,
						AuthorList = audiobook.AuthorSet
							.Select(author => author.ToSqlDataRecord())
							.AsTableValuedParameter(),
						GenreList = audiobook.GenreSet
							.Select(genre => genre.ToSqlDataRecord())
							.AsTableValuedParameter(),
						TrackUrlList = audiobook.TrackUrlSet
							.Select(trackUrl => new SqlDataRecord(new SqlMetaData("Url", SqlDbType.NVarChar, SqlMetaData.Max)).SetValuesFluent(trackUrl))
							.AsTableValuedParameter()
					},
					commandType: CommandType.StoredProcedure);
			}
			catch (Exception)
			{
				await DeleteAudiobookImageAsync(audiobook.ImgUrl);
				throw;
			}

			return invalidOptionalFieldSet;
		}

		private async Task<bool> DeleteAudiobookIfExistsAsync(int audiobookId)
		{
			var dbZaznam = await _dbConnection.QuerySingleOrDefaultAsync(sql: "SELECT [ImgUrl] FROM [Audiobook] WHERE [Id] = @Id", param: new { Id = audiobookId });
			if (dbZaznam == null)
			{
				return false;
			}

			await _dbConnection.ExecuteAsync(sql: "DELETE FROM [Audiobook] WHERE [Id] = @Id", param: new { Id = audiobookId });
			await this.DeleteAudiobookImageAsync(dbZaznam.ImgUrl);

			return true;
		}

		private async Task DeleteAudiobookImageAsync(string imgUrl)
		{
			try
			{
				var imgFileName = Path.GetFileName(imgUrl);
				if (imgFileName.IsNullOrWhiteSpace())
				{
					return;
				}

				var blob = await GetBlockBlobReferenceAsync(imgFileName);
				await blob.DeleteIfExistsAsync();
			}
			catch
			{
				// ignored
			}
		}

		private async Task<string> UploadAudiobookImageAsync(string archiveImgUrl)
		{
			var archiveImgResponse = (HttpResponseMessage)null;

			try
			{
				archiveImgResponse = await _httpClient.GetAsync(archiveImgUrl);

				if (archiveImgResponse.IsSuccessStatusCode && string.Equals(archiveImgResponse.Content.Headers.ContentType.MediaType, "image/jpeg", StringComparison.OrdinalIgnoreCase))
				{
					var blob = await GetBlockBlobReferenceAsync($"{Guid.NewGuid()}.jpg");
					blob.Properties.ContentType = "image/jpeg";

					await blob.UploadFromStreamAsync(await archiveImgResponse.Content.ReadAsStreamAsync());

					return blob.Uri.AbsoluteUri;
				}
			}
			catch
			{
				// ignored
			}
			finally
			{
				archiveImgResponse?.Dispose();
			}

			return null;
		}

		private Task<CloudBlockBlob> GetBlockBlobReferenceAsync(string fileName)
		{
			return _binder.BindAsync<CloudBlockBlob>(new BlobAttribute($"images/{fileName}", FileAccess.Write));
		}

		#region IDisposable

		private bool _disposed;

		public void Dispose()
		{
			if (_disposed)
			{
				return;
			}

			_httpClient?.Dispose();
			_dbConnection?.Dispose();

			_disposed = true;
		}

		#endregion IDisposable
	}
}