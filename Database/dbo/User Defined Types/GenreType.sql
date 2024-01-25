CREATE TYPE [dbo].[GenreType] AS TABLE
(
    [Id]   INT            NOT NULL,
    [Name] NVARCHAR (128) NOT NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC),
    INDEX [IX_GenreType_1] UNIQUE ([Name])
);