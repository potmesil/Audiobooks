CREATE TABLE [dbo].[AudiobookTrack] (
    [Id]          INT            IDENTITY (1, 1) NOT NULL,
    [AudiobookId] INT            NOT NULL,
    [Url]         NVARCHAR (256) NOT NULL,
    CONSTRAINT [PK_AudiobookTrack] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_AudiobookTrack_Audiobook] FOREIGN KEY ([AudiobookId]) REFERENCES [dbo].[Audiobook] ([Id]) ON DELETE CASCADE ON UPDATE CASCADE,
    CONSTRAINT [CHK_AudiobookTrack_1] CHECK ([Url]<>N'')
);