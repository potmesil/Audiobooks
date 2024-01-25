CREATE TABLE [dbo].[Language] (
    [Id]   INT           IDENTITY (1, 1) NOT NULL,
    [Name] NVARCHAR (32) NOT NULL,
    CONSTRAINT [PK_Language] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [CHK_Language_1] CHECK ([Name]<>N'')
);
GO

CREATE UNIQUE NONCLUSTERED INDEX [IX_Language_1] ON [dbo].[Language]([Name] ASC);