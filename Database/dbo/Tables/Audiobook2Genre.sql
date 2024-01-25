CREATE TABLE [dbo].[Audiobook2Genre] (
    [AudiobookId] INT NOT NULL,
    [GenreId]     INT NOT NULL,
    CONSTRAINT [PK_Audiobook2Genre] PRIMARY KEY CLUSTERED ([AudiobookId] ASC, [GenreId] ASC),
    CONSTRAINT [FK_Audiobook2Genre_Audiobook] FOREIGN KEY ([AudiobookId]) REFERENCES [dbo].[Audiobook] ([Id]) ON DELETE CASCADE ON UPDATE CASCADE,
    CONSTRAINT [FK_Audiobook2Genre_Genre] FOREIGN KEY ([GenreId]) REFERENCES [dbo].[Genre] ([Id]) ON DELETE CASCADE ON UPDATE CASCADE
);