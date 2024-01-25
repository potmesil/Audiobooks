SELECT * FROM
(
	SELECT [Author].[Id], [Author].[FirstName], [Author].[LastName],
	(
		SELECT COUNT(*) FROM [Audiobook2Author] WHERE [Audiobook2Author].[AuthorId] = [Author].[Id] AND [Audiobook2Author].[AudiobookId] IN
		(
			SELECT [Audiobook].[Id] FROM [Audiobook] WHERE [Audiobook].[LanguageId] IN (11)
		)
	) AS [AudiobookCount]
	FROM [Author]
	--WHERE [Author].[LastName] LIKE '%'
) AS [Temp]
WHERE [Temp].[AudiobookCount] > 0
ORDER BY [Temp].[LastName], [Temp].[FirstName]
OFFSET 0 ROWS FETCH NEXT 100 ROWS ONLY