CREATE TABLE [dbo].[PRODUCT_TRENDING](
	Id bigint IDENTITY(1,1) NOT NULL,
	CreatedBy [bigint] NULL,
	CreatedAt [datetime] NULL,
	LastUpdBy [bigint] NULL,
	LastUpdAt [datetime] NULL,
	Status [nvarchar](50) NULL,
	
	ProductId [bigint] NOT NULL,

	CONSTRAINT PRODUCT_TRENDING_P01 PRIMARY KEY CLUSTERED 
	(
		Id
	)
)

alter table [PRODUCT_TRENDING] add CONSTRAINT PRODUCT_TRENDING_PRODUCT_F01 FOREIGN KEY ([ProductId]) REFERENCES [PRODUCT](Id)
