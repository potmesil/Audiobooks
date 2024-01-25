CREATE TABLE [dbo].[ImportJobFailure] (
    [AudiobookId] INT            NOT NULL,
    [Exception]   NVARCHAR (MAX) NULL,
    [Severity]    TINYINT        NOT NULL,
    [CreatedAt]   DATETIME       CONSTRAINT [DF_ImportJobFailure_CreatedAt] DEFAULT (sysutcdatetime()) NOT NULL,
    CONSTRAINT [PK_ImportJobFailure] PRIMARY KEY CLUSTERED ([AudiobookId] ASC),
    CONSTRAINT [CHK_ImportJobFailure_1] CHECK ([Exception]<>N'' AND ([Severity]=(3) OR [Severity]=(2) OR [Severity]=(1)))
);