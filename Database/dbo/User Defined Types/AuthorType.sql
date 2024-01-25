CREATE TYPE [dbo].[AuthorType] AS TABLE
(
    [Id]        INT           NOT NULL,
    [FirstName] NVARCHAR (64) NULL,
    [LastName]  NVARCHAR (64) NOT NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC)
);