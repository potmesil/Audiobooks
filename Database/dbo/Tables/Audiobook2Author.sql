CREATE TABLE [dbo].[Audiobook2Author] (
    [AudiobookId] INT NOT NULL,
    [AuthorId]    INT NOT NULL,
    CONSTRAINT [PK_Audiobook2Author] PRIMARY KEY CLUSTERED ([AudiobookId] ASC, [AuthorId] ASC),
    CONSTRAINT [FK_Audiobook2Author_Audiobook] FOREIGN KEY ([AudiobookId]) REFERENCES [dbo].[Audiobook] ([Id]) ON DELETE CASCADE ON UPDATE CASCADE,
    CONSTRAINT [FK_Audiobook2Author_Author] FOREIGN KEY ([AuthorId]) REFERENCES [dbo].[Author] ([Id]) ON DELETE CASCADE ON UPDATE CASCADE
);