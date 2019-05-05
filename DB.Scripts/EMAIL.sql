CREATE TABLE [dbo].[EMAIL](
	Id bigint IDENTITY(1,1) NOT NULL,
	CreatedBy [bigint] NULL,
	CreatedAt [datetime] NULL,
	LastUpdBy [bigint] NULL,
	LastUpdAt [datetime] NULL,
	Status [nvarchar](50) NULL,
	
	Subject [nvarchar](100) NULL,
	Body  nvarchar(max) NULL,

	CONSTRAINT EMAIL_P01 PRIMARY KEY CLUSTERED 
	(
		Id
	)
)