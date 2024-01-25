SELECT * FROM
(
	SELECT [Genre].[Id], [Genre].[Name],
	(
		SELECT COUNT(*) FROM [Audiobook2Genre] WHERE [Audiobook2Genre].[GenreId] = [Genre].[Id] AND [Audiobook2Genre].[AudiobookId] IN
		(
			SELECT [Audiobook].[Id] FROM [Audiobook] WHERE [Audiobook].[LanguageId] IN (11)
		)
	) AS [AudiobookCount]
	FROM [Genre]
) AS [Temp]
WHERE [Temp].[AudiobookCount] > 0
ORDER BY [Temp].[Name]