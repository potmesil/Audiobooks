-- =============================================
-- Author:		Jaroslav Potměšil
-- Create date: 08.03.2019
-- =============================================
CREATE PROCEDURE [dbo].[sp_ImportJob]
	@AudiobookId			INT,
	@AudiobookTitle			NVARCHAR (256),
	@AudiobookImgUrl		NVARCHAR (256),
	@AudiobookLanguage		NVARCHAR (32),
	@AudiobookTotalTime		NVARCHAR (32),
	@AudiobookTotalTimeSecs	INT,
	@AuthorList				[AuthorType]	READONLY,
	@GenreList				[GenreType]		READONLY,
	@TrackUrlList			[TrackUrlType]	READONLY
AS
BEGIN
	SET NOCOUNT ON;
	SET XACT_ABORT ON;

	DECLARE @StartTranCount INT = @@TRANCOUNT;

	IF (@StartTranCount = 0)
	BEGIN
		BEGIN TRANSACTION;
	END;

	-- Language
	DECLARE @AudiobookLanguageId INT;
	SELECT @AudiobookLanguageId = [Id] FROM [Language] WHERE [Name] = @AudiobookLanguage;
	IF (@AudiobookLanguageId IS NULL)
	BEGIN
		INSERT INTO [Language] ([Name]) VALUES (@AudiobookLanguage);
		SELECT @AudiobookLanguageId = SCOPE_IDENTITY();
	END;

	-- Audiobook
	INSERT INTO [Audiobook] ([Id], [Title], [ImgUrl], [LanguageId], [TotalTime], [TotalTimeSecs])
	VALUES (@AudiobookId, @AudiobookTitle, @AudiobookImgUrl, @AudiobookLanguageId, @AudiobookTotalTime, @AudiobookTotalTimeSecs);
		
	-- Authors
	MERGE [Author] AS [Target]
	USING @AuthorList AS [Source]
	ON ([Source].[Id] = [Target].[Id])
	WHEN MATCHED THEN
		UPDATE SET
			[FirstName] = [Source].[FirstName],
			[LastName] = [Source].[LastName]
	WHEN NOT MATCHED THEN
		INSERT ([Id], [FirstName], [LastName])
		VALUES ([Source].[Id], [Source].[FirstName], [Source].[LastName]);
	INSERT INTO [Audiobook2Author] ([AudiobookId], [AuthorId]) SELECT @AudiobookId, [Id] FROM @AuthorList;

	-- Genres	
	MERGE [Genre] AS [Target]
	USING @GenreList AS [Source]
	ON ([Source].[Id] = [Target].[Id])
	WHEN MATCHED THEN
		UPDATE SET [Name] = [Source].[Name]
	WHEN NOT MATCHED THEN
		INSERT ([Id], [Name])
		VALUES ([Source].[Id], [Source].[Name]);
	INSERT INTO [Audiobook2Genre] ([AudiobookId], [GenreId]) SELECT @AudiobookId, [Id] FROM @GenreList;

	-- Tracks
	INSERT INTO [AudiobookTrack] ([AudiobookId], [Url]) SELECT @AudiobookId, [Url] FROM @TrackUrlList;

	IF (@StartTranCount = 0)
	BEGIN
		COMMIT TRANSACTION;
	END;
END;