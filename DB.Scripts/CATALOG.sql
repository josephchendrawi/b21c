CREATE TABLE [dbo].[CATALOG](
	Id bigint IDENTITY(1,1) NOT NULL,
	CreatedBy [bigint] NULL,
	CreatedAt [datetime] NULL,
	LastUpdBy [bigint] NULL,
	LastUpdAt [datetime] NULL,
	Status [nvarchar](50) NULL,
	
	URL [nvarchar](100) NOT NULL,
	Name [nvarchar](100) NULL,

	CONSTRAINT CATALOG_P01 PRIMARY KEY CLUSTERED 
	(
		Id
	)
)