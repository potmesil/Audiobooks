CREATE TABLE [dbo].[ImportJobHistory] (
    [StartDateTime]     DATETIME NOT NULL,
    [EndDateTime]       DATETIME CONSTRAINT [DF_ImportJobHistory_EndDateTime] DEFAULT (sysutcdatetime()) NOT NULL,
    [DurationMillisecs] INT      NOT NULL,
    [ImportedCount]     INT      NOT NULL,
    [FailedCount]       INT      NOT NULL
);
GO

-- =============================================
-- Author:		Jaroslav Potměšil
-- Create date: 08.03.2019
-- =============================================
CREATE TRIGGER [dbo].[tr_ImportJobHistory_Insert]
   ON [dbo].[ImportJobHistory]
   INSTEAD OF INSERT
AS 
BEGIN
	SET NOCOUNT ON;

	INSERT INTO [ImportJobHistory] ([StartDateTime], [EndDateTime], [DurationMillisecs], [ImportedCount], [FailedCount])
	SELECT [StartDateTime], [EndDateTime], DATEDIFF(millisecond, [StartDateTime], [EndDateTime]), [ImportedCount], [FailedCount] FROM inserted;
END;