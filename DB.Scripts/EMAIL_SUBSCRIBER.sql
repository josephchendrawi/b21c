CREATE TABLE [dbo].[EMAIL_SUBSCRIBER](
	Id bigint IDENTITY(1,1) NOT NULL,
	CreatedBy [bigint] NULL,
	CreatedAt [datetime] NULL,
	LastUpdBy [bigint] NULL,
	LastUpdAt [datetime] NULL,
	Status [nvarchar](50) NULL,
	
	Email [nvarchar](100) NOT NULL,

	CONSTRAINT CATALOG_SUBSCRIBER_EMAIL_P01 PRIMARY KEY CLUSTERED 
	(
		Id
	)
)