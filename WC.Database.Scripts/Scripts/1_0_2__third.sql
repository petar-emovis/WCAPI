
IF NOT EXISTS(SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[TestTable2]') AND type in (N'U'))
BEGIN	
	CREATE TABLE [dbo].[TestTable2]
	(
		Id                INT IDENTITY PRIMARY KEY,
		Success           BIT           NOT NULL
	)
END 
GO

