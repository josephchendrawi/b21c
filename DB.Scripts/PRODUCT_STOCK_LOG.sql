CREATE TABLE [dbo].[PRODUCT_STOCK_LOG](
	Id bigint IDENTITY(1,1) NOT NULL,
	CreatedBy [bigint] NULL,
	CreatedAt [datetime] NULL,
	LastUpdBy [bigint] NULL,
	LastUpdAt [datetime] NULL,
	Status [nvarchar](50) NULL,
	
	Available [nvarchar](max) NULL,
	SoldOut [nvarchar](max) NULL,
	Canceled [nvarchar](max) NULL,
	
	Action [nvarchar](100) NULL,

	CONSTRAINT PRODUCT_STOCK_LOG_P01 PRIMARY KEY CLUSTERED 
	(
		Id
	)
)