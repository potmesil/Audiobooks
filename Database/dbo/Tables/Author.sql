CREATE TABLE [dbo].[Author] (
    [Id]        INT           NOT NULL,
    [FirstName] NVARCHAR (64) NULL,
    [LastName]  NVARCHAR (64) NOT NULL,
    CONSTRAINT [PK_Author] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [CHK_Author_1] CHECK ([Id]>(0) AND [FirstName]<>N'' AND [LastName]<>N'')
);