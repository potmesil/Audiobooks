CREATE TABLE [dbo].[Genre] (
    [Id]   INT            NOT NULL,
    [Name] NVARCHAR (128) NOT NULL,
    CONSTRAINT [PK_Genre] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [CHK_Genre_1] CHECK ([Id]>(0) AND [Name]<>N'')
);
GO

CREATE UNIQUE NONCLUSTERED INDEX [IX_Genre_1] ON [dbo].[Genre]([Name] ASC);