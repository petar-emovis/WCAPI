IF NOT EXISTS(SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Country]') AND type in (N'U'))
BEGIN	
	CREATE TABLE [dbo].[Country]
	(
		[Id]					[int]          IDENTITY(1, 1) NOT FOR REPLICATION    NOT NULL,
		[Name]					[NVARCHAR](100)                                                NOT NULL,
		[CountryCodeIso2]		[NVARCHAR](2) NOT NULL,
		[CountryCodeIso3]		[NVARCHAR](3) NOT NULL,
		[CountryCodeNumerical]	[NVARCHAR](3) NULL,
		[CreationDate]			[Datetime] NOT NULL DEFAULT GETDATE(),
		[UpdateDate]			[Datetime] NOT NULL DEFAULT GETDATE(),

		CONSTRAINT [PK_Country]
		PRIMARY KEY CLUSTERED ([Id])
		ON [PRIMARY]
	) ON [PRIMARY]

END 
GO

IF NOT EXISTS(SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[IpRange]') AND type in (N'U'))
BEGIN
	CREATE TABLE [dbo].[IpRange]
	(
		[Id]             [int]          IDENTITY(1, 1) NOT FOR REPLICATION    NOT NULL,
		[CountryId]               [int]                                                NOT NULL,

		[IpVersion]             [int] NULL,
		[StartIp]				[varchar](100) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
		[EndIp]					[varchar](100) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
		[StartIpNumeric]        [BIGINT]   NULL,
		[EndIpNumeric]          [BIGINT]   NULL,
		[StartIpBinary]         [VARBINARY](16) NULL,
		[EndIpBinary]           [VARBINARY](16) NULL,
		[StartIpv6High] 		[BIGINT] NULL,
		[StartIpv6Low] 			[BIGINT] NULL,
		[EndIpv6High]   		[BIGINT] NULL,
		[EndIpv6Low]   			[BIGINT] NULL,
		[Active]                [bit]	 NOT NULL DEFAULT 1,
		[CreationDate]			[Datetime] NOT NULL DEFAULT GETDATE(),
		[UpdateDate]			[Datetime] NOT NULL DEFAULT GETDATE(),

		CONSTRAINT [PK_IpRange]
		PRIMARY KEY CLUSTERED ([Id])
		ON [PRIMARY]
	) ON [PRIMARY]

	ALTER TABLE [dbo].[IpRange]  WITH CHECK ADD CONSTRAINT [FK_IpRange_Country] FOREIGN KEY([CountryId])
	REFERENCES [dbo].[Country] ([Id])
	ALTER TABLE [dbo].[IpRange] CHECK CONSTRAINT [FK_IpRange_Country]
END
GO


IF NOT EXISTS(SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[IpRange]') AND name in (N'IX_IpRange_IPv4_Active_StartIpNumeric'))
BEGIN
	CREATE NONCLUSTERED INDEX IX_IpRange_IPv4_Active_StartIpNumeric
	ON dbo.IpRange (StartIpNumeric)
	INCLUDE (EndIpNumeric, CountryId)
	WHERE IpVersion = 4 AND Active = 1;
END
GO

IF NOT EXISTS(SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[IpRange]') AND name in (N'IX_IpRange_IPv6_Active_Start'))
BEGIN
	CREATE NONCLUSTERED INDEX IX_IpRange_IPv6_Active_Start
	ON dbo.IpRange (StartIpv6High, StartIpv6Low)
	INCLUDE (EndIpv6High, EndIpv6Low, CountryId)
	WHERE IpVersion = 6 AND Active = 1;
END
GO

IF NOT EXISTS(SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[IpRange]') AND name in (N'IX_IpRange_CountryId'))
BEGIN
	CREATE INDEX IX_IpRange_CountryId ON dbo.IpRange (CountryId);
END
GO

IF NOT EXISTS(SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[IpRange]') AND name in (N'IX_IpRange_IpVersion_Active'))
BEGIN
	CREATE INDEX IX_IpRange_IpVersion_Active ON dbo.IpRange (IpVersion, Active);
END
GO

