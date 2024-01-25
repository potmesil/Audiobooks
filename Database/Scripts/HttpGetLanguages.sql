SELECT * FROM
(
	SELECT [Language].[Id], [Language].[Name],
	(
		SELECT COUNT(*) FROM [Audiobook] WHERE [Audiobook].[LanguageId] = [Language].[Id]
	) AS [AudiobookCount]
	FROM [Language]
) AS [Temp]
WHERE [Temp].[AudiobookCount] > 0
ORDER BY [Temp].[Name]