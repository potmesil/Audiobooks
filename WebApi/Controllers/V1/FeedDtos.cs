using Newtonsoft.Json;
using System.Collections.Generic;

namespace Audiobooks.WebApi.Controllers.V1
{
	public class LanguageDto
	{
		[JsonProperty(PropertyName = "id")]
		public int Id;

		[JsonProperty(PropertyName = "name")]
		public string Name;

		[JsonProperty(PropertyName = "book_count")]
		public int AudiobookCount;
	}

	public class GenreDto
	{
		[JsonProperty(PropertyName = "id")]
		public int Id;

		[JsonProperty(PropertyName = "name")]
		public string Name;

		[JsonProperty(PropertyName = "book_count")]
		public int AudiobookCount;
	}

	public class AuthorDto
	{
		[JsonProperty(PropertyName = "id")]
		public int Id;

		[JsonProperty(PropertyName = "first_name")]
		public string FirstName;

		[JsonProperty(PropertyName = "last_name")]
		public string LastName;

		[JsonProperty(PropertyName = "book_count")]
		public int AudiobookCount;
	}

	public class AudiobookDto : IId
	{
		[JsonProperty(PropertyName = "id")]
		public int Id { get; set; }

		[JsonProperty(PropertyName = "title")]
		public string Title { get; set; }

		[JsonProperty(PropertyName = "url_img")]
		public string ImgUrl { get; set; }

		[JsonProperty(PropertyName = "url_tracks")]
		public HashSet<string> TrackUrlSet { get; set; }

		[JsonProperty(PropertyName = "language")]
		public string Language { get; set; }

		[JsonProperty(PropertyName = "totaltime")]
		public string TotalTime { get; set; }

		[JsonProperty(PropertyName = "totaltimesecs")]
		public int? TotalTimeSecs { get; set; }

		[JsonProperty(PropertyName = "authors")]
		public HashSet<AudiobookAuthorDto> AuthorSet { get; set; }

		[JsonProperty(PropertyName = "genres")]
		public HashSet<AudiobookGenreDto> GenreSet { get; set; }
	}

	public class AudiobookAuthorDto : IId
	{
		[JsonProperty(PropertyName = "id")]
		public int Id { get; set; }

		[JsonProperty(PropertyName = "first_name")]
		public string FirstName { get; set; }

		[JsonProperty(PropertyName = "last_name")]
		public string LastName { get; set; }
	}

	public class AudiobookGenreDto : IId
	{
		[JsonProperty(PropertyName = "id")]
		public int Id { get; set; }

		[JsonProperty(PropertyName = "name")]
		public string Name { get; set; }
	}
}