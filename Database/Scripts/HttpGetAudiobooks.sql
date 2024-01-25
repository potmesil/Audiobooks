SELECT
	[AudiobookTemp].*,
	[Language].[Name] AS [LanguageName],
	[Author].[Id] AS [AuthorId],
	[Author].[FirstName] AS [AuthorFirstName],
	[Author].[LastName] AS [AuthorLastName],
	[Genre].[Id] AS [GenreId],
	[Genre].[Name] AS [GenreName]
FROM
(
	SELECT
		[Audiobook].[Id],
		[Audiobook].[Title],
		[Audiobook].[ImgUrl],
		[Audiobook].[LanguageId],
		[Audiobook].[TotalTime],
		[Audiobook].[TotalTimeSecs]
	FROM [Audiobook]
	WHERE 1=1
		--AND [Audiobook].[LanguageId] IN (11)
		--AND [Audiobook].[Id] IN
		--(
		--	SELECT [Audiobook2Genre].[AudiobookId] FROM [Audiobook2Genre] WHERE [Audiobook2Genre].[GenreId] IN (100)
		--)
		--AND [Audiobook].[Id] IN
		--(
		--	SELECT [Audiobook2Author].[AudiobookId] FROM [Audiobook2Author] WHERE [Audiobook2Author].[AuthorId] = 2
		--)
		--AND [Audiobook].[Id] IN
		--(
		--	SELECT [Audiobook2Author].[AudiobookId] FROM [Audiobook2Author] WHERE [Audiobook2Author].[AuthorId] IN
		--	(
		--		SELECT [Author].[Id] FROM [Author] WHERE [Author].[LastName] = 'Cummings'
		--	)
		--)
		--AND [Audiobook].[Title] LIKE '%'
		--AND [Audiobook].[Id] = 10039
	--ORDER BY NEWID()
	ORDER BY [Audiobook].[Title]
	OFFSET 0 ROWS FETCH NEXT 100 ROWS ONLY
) AS [AudiobookTemp]
JOIN [Language] ON [Language].[Id] = [AudiobookTemp].[LanguageId]
LEFT JOIN [Author] ON [Author].[Id] IN (SELECT [Audiobook2Author].[AuthorId] FROM [Audiobook2Author] WHERE [Audiobook2Author].[AudiobookId] = [AudiobookTemp].[Id])
LEFT JOIN [Genre] ON [Genre].[Id] IN (SELECT [Audiobook2Genre].[GenreId] FROM [Audiobook2Genre] WHERE [Audiobook2Genre].[AudiobookId] = [AudiobookTemp].[Id])
ORDER BY [AudiobookTemp].[Title]