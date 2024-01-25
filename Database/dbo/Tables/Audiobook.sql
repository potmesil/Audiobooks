CREATE TABLE [dbo].[Audiobook] (
    [Id]            INT            NOT NULL,
    [Title]         NVARCHAR (256) NOT NULL,
    [ImgUrl]        NVARCHAR (256) NULL,
    [LanguageId]    INT            NOT NULL,
    [TotalTime]     NVARCHAR (32)  NULL,
    [TotalTimeSecs] INT            NULL,
    [CreatedAt]     DATETIME       CONSTRAINT [DF_Audiobook_CreatedAt] DEFAULT (sysutcdatetime()) NOT NULL,
    CONSTRAINT [PK_Audiobook] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_Audiobook_Language] FOREIGN KEY ([LanguageId]) REFERENCES [dbo].[Language] ([Id]) ON DELETE CASCADE ON UPDATE CASCADE,
    CONSTRAINT [CHK_Audiobook_1] CHECK ([Id]>(0) AND [Title]<>N'' AND [ImgUrl]<>N'' AND [TotalTime]<>N'' AND [TotalTimeSecs]>(0))
);