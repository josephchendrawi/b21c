CREATE TABLE [dbo].[PRODUCT_CATEGORY](
	Id bigint IDENTITY(1,1) NOT NULL,
	CreatedBy [bigint] NULL,
	CreatedAt [datetime] NULL,
	LastUpdBy [bigint] NULL,
	LastUpdAt [datetime] NULL,
	
	Name [nvarchar](max),
	ParentCategoryId bigint,

	CONSTRAINT PRODUCT_CATEGORY_P01 PRIMARY KEY CLUSTERED 
	(
		Id
	)
)