
IF NOT EXISTS(SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[TestTable]') AND type in (N'U'))
BEGIN	
	CREATE TABLE [dbo].[TestTable]
	(
		Id                INT IDENTITY PRIMARY KEY,
		Success           BIT           NOT NULL
	)
END 
GO

IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'dbo.TestProcedure') AND type = 'P')
BEGIN 
	PRINT 'DROP PROCEDURE dbo.TestProcedure'
	DROP PROCEDURE dbo.TestProcedure
END
GO


PRINT 'CREATE PROCEDURE dbo.TestProcedure'
GO

CREATE PROCEDURE dbo.TestProcedure
AS
BEGIN
    SELECT GETDATE(), 1, 'IO29640569', 'ABC001', 'Mr', 'John Doe', '123 Main St', '', '', 'London', 'SW1A 1AA', GETDATE(), 'Open', 'Equifax', GETDATE(), GETDATE(), GETDATE(), GETDATE() 

END
GO