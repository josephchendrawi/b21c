CREATE TABLE [dbo].[PRODUCT](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[CreatedBy] [bigint] NULL,
	[CreatedAt] [datetime] NULL,
	[LastUpdBy] [bigint] NULL,
	[LastUpdAt] [datetime] NULL,
	[Status] [nvarchar](50) NULL,
	[ProductCode] [nvarchar](50) NOT NULL,
	[Name] [nvarchar](100) NOT NULL,
	[Price] [decimal](18, 2) NULL,
	[Size] [nvarchar](100) NULL,
	[Weight] [int] NULL,
	[Color] [nvarchar](100) NULL,
	[Stock] [int] NULL,
	[Description] [nvarchar](max) NULL,
	[Sales] [bit] NULL,
	[SalesDiscount] [decimal](18, 2) NULL,
	CONSTRAINT [PRODUCT_P01] PRIMARY KEY CLUSTERED 
	(
		[Id] ASC
	)
)