CREATE TABLE [dbo].[BANNER](
	Id bigint IDENTITY(1,1) NOT NULL,
	CreatedBy [bigint] NULL,
	CreatedAt [datetime] NULL,
	LastUpdBy [bigint] NULL,
	LastUpdAt [datetime] NULL,
	Status [nvarchar](50) NULL,
	
	URL [nvarchar](100) NULL,
	ImageURL [nvarchar](100) NULL,
	Name [nvarchar](100) NULL,

	CONSTRAINT BANNER_P01 PRIMARY KEY CLUSTERED 
	(
		Id
	)
)