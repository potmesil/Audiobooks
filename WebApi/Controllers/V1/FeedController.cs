using Dapper;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Audiobooks.WebApi.Controllers.V1
{
	[Route("api/v1/feed")]
	[ApiExplorerSettings(GroupName = "v1")]
	public class FeedController : ControllerBase
	{
		private readonly DbConnection _dbConnection;

		public FeedController(DbConnection dbConnection)
		{
			_dbConnection = dbConnection;
		}

		[HttpGet("languages")]
		[Produces("application/json", Type = typeof(IEnumerable<LanguageDto>))]
		public async Task<IActionResult> GetLanguagesAsync()
		{
			var sqlBuilder = new StringBuilder();

			sqlBuilder.AppendLine("SELECT * FROM");
			sqlBuilder.AppendLine("(");
			sqlBuilder.AppendLineWithTabs("SELECT [Language].[Id], [Language].[Name],", 1);
			sqlBuilder.AppendLineWithTabs("(", 1);
			sqlBuilder.AppendLineWithTabs("SELECT COUNT(*) FROM [Audiobook] WHERE [Audiobook].[LanguageId] = [Language].[Id]", 2);
			sqlBuilder.AppendLineWithTabs(") AS [AudiobookCount]", 1);
			sqlBuilder.AppendLineWithTabs("FROM [Language]", 1);
			sqlBuilder.AppendLine(") AS [Temp]");
			sqlBuilder.AppendLine("WHERE [Temp].[AudiobookCount] > 0");
			sqlBuilder.Append("ORDER BY [Temp].[Name]");

			return Ok(await _dbConnection.QueryAsync<LanguageDto>(sqlBuilder.ToString()));
		}

		[HttpGet("genres")]
		[Produces("application/json", Type = typeof(IEnumerable<GenreDto>))]
		[ProducesResponseType(typeof(ValidationProblemDetails), 400)]
		public async Task<IActionResult> GetGenresAsync([Required, ModelBinder(typeof(CommaSeparatedArrayModelBinder))] int[] languageIds)
		{
			var sqlParameters = new DynamicParameters(new { LanguageIds = languageIds });
			var sqlBuilder = new StringBuilder();

			sqlBuilder.AppendLine("SELECT * FROM");
			sqlBuilder.AppendLine("(");
			sqlBuilder.AppendLineWithTabs("SELECT [Genre].[Id], [Genre].[Name],", 1);
			sqlBuilder.AppendLineWithTabs("(", 1);
			sqlBuilder.AppendLineWithTabs("SELECT COUNT(*) FROM [Audiobook2Genre] WHERE [Audiobook2Genre].[GenreId] = [Genre].[Id] AND [Audiobook2Genre].[AudiobookId] IN", 2);
			sqlBuilder.AppendLineWithTabs("(", 2);
			sqlBuilder.AppendLineWithTabs("SELECT [Audiobook].[Id] FROM [Audiobook] WHERE [Audiobook].[LanguageId] IN @LanguageIds", 3);
			sqlBuilder.AppendLineWithTabs(")", 2);
			sqlBuilder.AppendLineWithTabs(") AS [AudiobookCount]", 1);
			sqlBuilder.AppendLineWithTabs("FROM [Genre]", 1);
			sqlBuilder.AppendLine(") AS [Temp]");
			sqlBuilder.AppendLine("WHERE [Temp].[AudiobookCount] > 0");
			sqlBuilder.Append("ORDER BY [Temp].[Name]");

			return Ok(await _dbConnection.QueryAsync<GenreDto>(sqlBuilder.ToString(), sqlParameters));
		}

		[HttpGet("authors")]
		[Produces("application/json", Type = typeof(IEnumerable<AuthorDto>))]
		[ProducesResponseType(typeof(ValidationProblemDetails), 400)]
		public async Task<IActionResult> GetAuthorsAsync(
			[Required, ModelBinder(typeof(CommaSeparatedArrayModelBinder))] int[] languageIds,
			string lastName,
			[Range(0, int.MaxValue)] int offset,
			[Range(1, 100)] int limit = 100)
		{
			var sqlParameters = new DynamicParameters(new
			{
				LanguageIds = languageIds,
				Offset = offset,
				Limit = limit
			});
			var sqlBuilder = new StringBuilder();

			sqlBuilder.AppendLine("SELECT * FROM");
			sqlBuilder.AppendLine("(");
			sqlBuilder.AppendLineWithTabs("SELECT [Author].[Id], [Author].[FirstName], [Author].[LastName],", 1);
			sqlBuilder.AppendLineWithTabs("(", 1);
			sqlBuilder.AppendLineWithTabs("SELECT COUNT(*) FROM [Audiobook2Author] WHERE [Audiobook2Author].[AuthorId] = [Author].[Id] AND [Audiobook2Author].[AudiobookId] IN", 2);
			sqlBuilder.AppendLineWithTabs("(", 2);
			sqlBuilder.AppendLineWithTabs("SELECT [Audiobook].[Id] FROM [Audiobook] WHERE [Audiobook].[LanguageId] IN @LanguageIds", 3);
			sqlBuilder.AppendLineWithTabs(")", 2);
			sqlBuilder.AppendLineWithTabs(") AS [AudiobookCount]", 1);
			sqlBuilder.AppendLineWithTabs("FROM [Author]", 1);

			if (lastName != null)
			{
				sqlBuilder.AppendLineWithTabs("WHERE [Author].[LastName] LIKE @LastName + '%'", 1);
				sqlParameters.Add("@LastName", lastName);
			}

			sqlBuilder.AppendLine(") AS [Temp]");
			sqlBuilder.AppendLine("WHERE [Temp].[AudiobookCount] > 0");
			sqlBuilder.AppendLine("ORDER BY [Temp].[LastName], [Temp].[FirstName]");
			sqlBuilder.Append("OFFSET @Offset ROWS FETCH NEXT @Limit ROWS ONLY");

			return Ok(await _dbConnection.QueryAsync<AuthorDto>(sqlBuilder.ToString(), sqlParameters));
		}

		[HttpGet("audiobooks")]
		[Produces("application/json", Type = typeof(IEnumerable<AudiobookDto>))]
		public async Task<IActionResult> GetAudiobooksAsync(
			[ModelBinder(typeof(CommaSeparatedArrayModelBinder))] int[] languageIds,
			[ModelBinder(typeof(CommaSeparatedArrayModelBinder))] int[] genreIds,
			int? authorId,
			string authorLastName,
			string title,
			int? id,
			bool extended,
			bool random,
			[Range(0, int.MaxValue)] int offset,
			[Range(1, 100)] int limit = 100)
		{
			var sqlParameters = new DynamicParameters(new
			{
				Offset = offset,
				Limit = limit
			});
			var sqlBuilder = new StringBuilder();

			sqlBuilder.AppendLine("SELECT");
			sqlBuilder.AppendLineWithTabs("[AudiobookTemp].*,", 1);
			sqlBuilder.AppendLineWithTabs("[Language].[Name] AS [LanguageName],", 1);
			sqlBuilder.AppendLineWithTabs("[Author].[Id] AS [AuthorId],", 1);
			sqlBuilder.AppendLineWithTabs("[Author].[FirstName] AS [AuthorFirstName],", 1);
			sqlBuilder.AppendLineWithTabs("[Author].[LastName] AS [AuthorLastName],", 1);
			sqlBuilder.AppendLineWithTabs("[Genre].[Id] AS [GenreId],", 1);
			sqlBuilder.AppendLineWithTabs("[Genre].[Name] AS [GenreName]", 1);
			sqlBuilder.AppendLine("FROM");
			sqlBuilder.AppendLine("(");
			sqlBuilder.AppendLineWithTabs("SELECT", 1);
			sqlBuilder.AppendLineWithTabs("[Audiobook].[Id],", 2);
			sqlBuilder.AppendLineWithTabs("[Audiobook].[Title],", 2);
			sqlBuilder.AppendLineWithTabs("[Audiobook].[ImgUrl],", 2);
			sqlBuilder.AppendLineWithTabs("[Audiobook].[LanguageId],", 2);
			sqlBuilder.AppendLineWithTabs("[Audiobook].[TotalTime],", 2);
			sqlBuilder.AppendLineWithTabs("[Audiobook].[TotalTimeSecs]", 2);
			sqlBuilder.AppendLineWithTabs("FROM [Audiobook]", 1);
			sqlBuilder.AppendLineWithTabs("WHERE 1=1", 1);

			if (languageIds != null)
			{
				sqlBuilder.AppendLineWithTabs("AND [Audiobook].[LanguageId] IN @LanguageIds", 2);
				sqlParameters.Add("@LanguageIds", languageIds);
			}

			if (genreIds != null)
			{
				sqlBuilder.AppendLineWithTabs("AND [Audiobook].[Id] IN", 2);
				sqlBuilder.AppendLineWithTabs("(", 2);
				sqlBuilder.AppendLineWithTabs("SELECT [Audiobook2Genre].[AudiobookId] FROM [Audiobook2Genre] WHERE [Audiobook2Genre].[GenreId] IN @GenreIds", 3);
				sqlBuilder.AppendLineWithTabs(")", 2);
				sqlParameters.Add("@GenreIds", genreIds);
			}

			if (authorId.HasValue)
			{
				sqlBuilder.AppendLineWithTabs("AND [Audiobook].[Id] IN", 2);
				sqlBuilder.AppendLineWithTabs("(", 2);
				sqlBuilder.AppendLineWithTabs("SELECT [Audiobook2Author].[AudiobookId] FROM [Audiobook2Author] WHERE [Audiobook2Author].[AuthorId] = @AuthorId", 3);
				sqlBuilder.AppendLineWithTabs(")", 2);
				sqlParameters.Add("@AuthorId", authorId);
			}

			if (authorLastName != null)
			{
				sqlBuilder.AppendLineWithTabs("AND [Audiobook].[Id] IN", 2);
				sqlBuilder.AppendLineWithTabs("(", 2);
				sqlBuilder.AppendLineWithTabs("SELECT [Audiobook2Author].[AudiobookId] FROM [Audiobook2Author] WHERE [Audiobook2Author].[AuthorId] IN", 3);
				sqlBuilder.AppendLineWithTabs("(", 3);
				sqlBuilder.AppendLineWithTabs("SELECT [Author].[Id] FROM [Author] WHERE [Author].[LastName] = @AuthorLastName", 4);
				sqlBuilder.AppendLineWithTabs(")", 3);
				sqlBuilder.AppendLineWithTabs(")", 2);
				sqlParameters.Add("@AuthorLastName", authorLastName);
			}

			if (title != null)
			{
				sqlBuilder.AppendLineWithTabs("AND [Audiobook].[Title] LIKE @Title + '%'", 2);
				sqlParameters.Add("@Title", title);
			}

			if (id.HasValue)
			{
				sqlBuilder.AppendLineWithTabs("AND [Audiobook].[Id] = @Id", 2);
				sqlParameters.Add("@Id", id);
			}

			sqlBuilder.AppendLineWithTabs(random ? "ORDER BY NEWID()" : "ORDER BY [Audiobook].[Title]", 1);
			sqlBuilder.AppendLineWithTabs("OFFSET @Offset ROWS FETCH NEXT @Limit ROWS ONLY", 1);
			sqlBuilder.AppendLine(") AS [AudiobookTemp]");
			sqlBuilder.AppendLine("JOIN [Language] ON [Language].[Id] = [AudiobookTemp].[LanguageId]");
			sqlBuilder.AppendLine("LEFT JOIN [Author] ON [Author].[Id] IN (SELECT [Audiobook2Author].[AuthorId] FROM [Audiobook2Author] WHERE [Audiobook2Author].[AudiobookId] = [AudiobookTemp].[Id])");
			sqlBuilder.AppendLine("LEFT JOIN [Genre] ON [Genre].[Id] IN (SELECT [Audiobook2Genre].[GenreId] FROM [Audiobook2Genre] WHERE [Audiobook2Genre].[AudiobookId] = [AudiobookTemp].[Id])");
			sqlBuilder.Append("ORDER BY [AudiobookTemp].[Title]");

			var audiobookList = (List<dynamic>)await _dbConnection.QueryAsync(sqlBuilder.ToString(), sqlParameters);

			if (!audiobookList.Any())
			{
				return Ok(Enumerable.Empty<AudiobookDto>());
			}

			var audiobookDtoSet = audiobookList
				.Select(audiobook => new AudiobookDto
				{
					Id = audiobook.Id,
					Title = audiobook.Title,
					ImgUrl = audiobook.ImgUrl,
					Language = audiobook.LanguageName,
					TotalTime = audiobook.TotalTime,
					TotalTimeSecs = audiobook.TotalTimeSecs
				})
				.ToHashSet(new IdEqualityComparer<AudiobookDto>());
			var audiobookTrackList = extended
				? (List<dynamic>)await _dbConnection.QueryAsync($"SELECT [AudiobookId], [Url] FROM [AudiobookTrack] WHERE [AudiobookId] IN ({string.Join(",", audiobookDtoSet.Select(audiobookDto => audiobookDto.Id))}) ORDER BY [Url]")
				: null;

			foreach (var audiobookDto in audiobookDtoSet)
			{
				var audiobooks = audiobookList
					.Where(audiobook => audiobook.Id == audiobookDto.Id)
					.ToArray();

				if (audiobooks.All(audiobook => audiobook.AuthorId != null))
				{
					audiobookDto.AuthorSet = audiobooks
						.Select(audiobook => new AudiobookAuthorDto
						{
							Id = audiobook.AuthorId,
							FirstName = audiobook.AuthorFirstName,
							LastName = audiobook.AuthorLastName
						})
						.ToHashSet(new IdEqualityComparer<AudiobookAuthorDto>());
				}

				if (audiobooks.All(audiobook => audiobook.GenreId != null))
				{
					audiobookDto.GenreSet = audiobooks
						.Select(audiobook => new AudiobookGenreDto
						{
							Id = audiobook.GenreId,
							Name = audiobook.GenreName
						})
						.ToHashSet(new IdEqualityComparer<AudiobookGenreDto>());
				}

				audiobookDto.TrackUrlSet = audiobookTrackList?
					.Where(audiobookTrack => audiobookTrack.AudiobookId == audiobookDto.Id)
					.Select(audiobookTrack => audiobookTrack.Url as string)
					.ToHashSet(StringComparer.OrdinalIgnoreCase);
			}

			return Ok(audiobookDtoSet);
		}
	}
}