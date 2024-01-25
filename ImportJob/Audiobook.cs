using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Audiobooks.ImportJob
{
	public class Audiobook
	{
		private string _title;
		private string _language;
		private string _totalTime;
		private int? _totalTimeSecs;
		private string _archiveBaseUrl;
		private string _imgUrl;

		private Audiobook()
		{
		}

		[JsonProperty(PropertyName = "id")]
		public int Id { get; private set; }

		[JsonProperty(PropertyName = "title")]
		public string Title
		{
			get => _title;
			private set => _title = value.WhiteSpaceToNull();
		}

		[JsonProperty(PropertyName = "language")]
		public string Language
		{
			get => _language;
			private set => _language = value.WhiteSpaceToNull();
		}

		[JsonProperty(PropertyName = "totaltime")]
		public string TotalTime
		{
			get => _totalTime;
			private set => _totalTime = value.WhiteSpaceToNull();
		}

		[JsonProperty(PropertyName = "totaltimesecs")]
		public int? TotalTimeSecs
		{
			get => _totalTimeSecs;
			private set => _totalTimeSecs = value > 0 ? value : null;
		}

		[JsonProperty(PropertyName = "authors")]
		public HashSet<Author> AuthorSet { get; } = new HashSet<Author>(new AuthorEqualityComparer());

		[JsonProperty(PropertyName = "genres")]
		public HashSet<Genre> GenreSet { get; } = new HashSet<Genre>(new GenreEqualityComparer());

		[JsonProperty(PropertyName = "url_iarchive")]
		public string ArchiveBaseUrl
		{
			get => _archiveBaseUrl;
			private set
			{
				if (value.IsWellFormedUriString())
				{
					_archiveBaseUrl = value
						.Trim()
						.Replace("http://", "https://")
						.Replace("https://www.", "https://")
						.Replace("/details/", "/download/")
						.TrimEnd('/');
				}
			}
		}

		[JsonIgnore]
		public HashSet<string> TrackUrlSet { get; } = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

		[JsonIgnore]
		public string ArchiveImgUrl { get; set; }

		[JsonIgnore]
		public string ImgUrl
		{
			get => _imgUrl;
			set
			{
				if (value.IsWellFormedUriString())
				{
					_imgUrl = value.Trim();
				}
			}
		}
	}
}