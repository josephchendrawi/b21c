CREATE TABLE [dbo].[PRODUCT_IMAGE](
	Id bigint IDENTITY(1,1) NOT NULL,
	CreatedBy [bigint] NULL,
	CreatedAt [datetime] NULL,
	LastUpdBy [bigint] NULL,
	LastUpdAt [datetime] NULL,
	Status [nvarchar](50) NULL,
	
	ProductId [bigint] NOT NULL,
	
	URL [nvarchar](300) NOT NULL,

	CONSTRAINT PRODUCT_IMAGE_P01 PRIMARY KEY CLUSTERED 
	(
		Id
	)
)

ALTER TABLE PRODUCT_IMAGE
ADD DisplayOrder [int] NULL