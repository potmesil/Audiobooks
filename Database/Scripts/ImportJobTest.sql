DECLARE @p7 [AuthorType];
INSERT INTO @p7 VALUES(5781,N' Walter',N'Lippmann');

DECLARE @p8 [GenreType];
INSERT INTO @p8 VALUES(36,N'*Non-fiction');
INSERT INTO @p8 VALUES(99,N'Political Science');

DECLARE @p9 [TrackUrlType];
INSERT INTO @p9 VALUES(N'http://archive.org/download/public_opinion_1312_librivox/publicopinion_01_lippmann.ogg');
INSERT INTO @p9 VALUES(N'http://archive.org/download/public_opinion_1312_librivox/publicopinion_02_lippmann.ogg');
INSERT INTO @p9 VALUES(N'http://archive.org/download/public_opinion_1312_librivox/publicopinion_03_lippmann.ogg');
INSERT INTO @p9 VALUES(N'http://archive.org/download/public_opinion_1312_librivox/publicopinion_04_lippmann.ogg');
INSERT INTO @p9 VALUES(N'http://archive.org/download/public_opinion_1312_librivox/publicopinion_05_lippmann.ogg');
INSERT INTO @p9 VALUES(N'http://archive.org/download/public_opinion_1312_librivox/publicopinion_06_lippmann.ogg');
INSERT INTO @p9 VALUES(N'http://archive.org/download/public_opinion_1312_librivox/publicopinion_07_lippmann.ogg');
INSERT INTO @p9 VALUES(N'http://archive.org/download/public_opinion_1312_librivox/publicopinion_08_lippmann.ogg');
INSERT INTO @p9 VALUES(N'http://archive.org/download/public_opinion_1312_librivox/publicopinion_09_lippmann.ogg');
INSERT INTO @p9 VALUES(N'http://archive.org/download/public_opinion_1312_librivox/publicopinion_10_lippmann.ogg');
INSERT INTO @p9 VALUES(N'http://archive.org/download/public_opinion_1312_librivox/publicopinion_11_lippmann.ogg');
INSERT INTO @p9 VALUES(N'http://archive.org/download/public_opinion_1312_librivox/publicopinion_12_lippmann.ogg');
INSERT INTO @p9 VALUES(N'http://archive.org/download/public_opinion_1312_librivox/publicopinion_13_lippmann.ogg');
INSERT INTO @p9 VALUES(N'http://archive.org/download/public_opinion_1312_librivox/publicopinion_14_lippmann.ogg');
INSERT INTO @p9 VALUES(N'http://archive.org/download/public_opinion_1312_librivox/publicopinion_15_lippmann.ogg');
INSERT INTO @p9 VALUES(N'http://archive.org/download/public_opinion_1312_librivox/publicopinion_16_lippmann.ogg');
INSERT INTO @p9 VALUES(N'http://archive.org/download/public_opinion_1312_librivox/publicopinion_17_lippmann.ogg');
INSERT INTO @p9 VALUES(N'http://archive.org/download/public_opinion_1312_librivox/publicopinion_18_lippmann.ogg');
INSERT INTO @p9 VALUES(N'http://archive.org/download/public_opinion_1312_librivox/publicopinion_19_lippmann.ogg');
INSERT INTO @p9 VALUES(N'http://archive.org/download/public_opinion_1312_librivox/publicopinion_20_lippmann.ogg');
INSERT INTO @p9 VALUES(N'http://archive.org/download/public_opinion_1312_librivox/publicopinion_21_lippmann.ogg');
INSERT INTO @p9 VALUES(N'http://archive.org/download/public_opinion_1312_librivox/publicopinion_22_lippmann.ogg');
INSERT INTO @p9 VALUES(N'http://archive.org/download/public_opinion_1312_librivox/publicopinion_23_lippmann.ogg');
INSERT INTO @p9 VALUES(N'http://archive.org/download/public_opinion_1312_librivox/publicopinion_24_lippmann.ogg');
INSERT INTO @p9 VALUES(N'http://archive.org/download/public_opinion_1312_librivox/publicopinion_25_lippmann.ogg');
INSERT INTO @p9 VALUES(N'http://archive.org/download/public_opinion_1312_librivox/publicopinion_26_lippmann.ogg');
INSERT INTO @p9 VALUES(N'http://archive.org/download/public_opinion_1312_librivox/publicopinion_27_lippmann.ogg');
INSERT INTO @p9 VALUES(N'http://archive.org/download/public_opinion_1312_librivox/publicopinion_28_lippmann.ogg');

EXEC [sp_ImportJob]
	@AudiobookId=5678,
	@AudiobookTitle=N'Public Opinion',
	@AudiobookImgUrl=N'http://127.0.0.1:10000/devstoreaccount1/testik/0e5b756a-cac6-4932-8679-fb69feded45b.jpg',
	@AudiobookLanguage=N'English',
	@AudiobookTotalTime=N'10:42:35',
	@AudiobookTotalTimeSecs=38555,
	@AuthorList=@p7,
	@GenreList=@p8,
	@TrackUrlList=@p9;