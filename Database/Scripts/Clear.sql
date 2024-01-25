truncate table AudiobookTrack;
delete from Audiobook;
delete from Author;
delete from Genre;
delete from [Language];
delete from ImportJobFailure;
delete from ImportJobHistory;
insert into ImportJobHistory (StartDateTime, EndDateTime, ImportedCount, FailedCount) values ('2019-03-05 00:00:00.000','2019-03-05 00:01:00.000',0,0);