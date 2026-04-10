
IF NOT EXISTS(SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[TestTable]') AND type in (N'U'))
BEGIN	
	CREATE TABLE [dbo].[TestTable]
	(
		Id                INT IDENTITY PRIMARY KEY,
		Success           BIT           NOT NULL
	)
END 
GO