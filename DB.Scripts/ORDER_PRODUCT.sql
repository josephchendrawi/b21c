CREATE TABLE [dbo].[ORDER_PRODUCT](
	Id bigint IDENTITY(1,1) NOT NULL,
	CreatedBy [bigint] NULL,
	CreatedAt [datetime] NULL,
	LastUpdBy [bigint] NULL,
	LastUpdAt [datetime] NULL,
	Status [nvarchar](50) NULL,
	
	ProductId [bigint] NOT NULL,
	OrderId [bigint] NOT NULL,

	CONSTRAINT ORDER_PRODUCT_P01 PRIMARY KEY CLUSTERED 
	(
		Id
	)
)