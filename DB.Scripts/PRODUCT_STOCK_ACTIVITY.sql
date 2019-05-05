CREATE TABLE [dbo].[PRODUCT_STOCK_ACTIVITY](
	Id bigint IDENTITY(1,1) NOT NULL,
	CreatedBy [bigint] NULL,
	CreatedAt [datetime] NULL,
	LastUpdBy [bigint] NULL,
	LastUpdAt [datetime] NULL,
	
	ProductId [bigint] NOT NULL,
	StockBefore int NOT NULL,
	StockAfter int NOT NULL,
	Remarks [nvarchar](max) NULL,

	CONSTRAINT PRODUCT_STOCK_ACTIVITY_P01 PRIMARY KEY CLUSTERED 
	(
		Id
	)
)

alter table [PRODUCT_STOCK_ACTIVITY] add CONSTRAINT PRODUCT_STOCK_ACTIVITY_PRODUCT_F01 FOREIGN KEY ([ProductId]) REFERENCES [PRODUCT](Id)