CREATE TABLE [dbo].[SURVEY](
	Id bigint IDENTITY(1,1) NOT NULL,
	CreatedBy [bigint] NULL,
	CreatedAt [datetime] NULL,
	LastUpdBy [bigint] NULL,
	LastUpdAt [datetime] NULL,
	
	Name [nvarchar](100),
	URL [nvarchar](max),

	CONSTRAINT SURVEY_P01 PRIMARY KEY CLUSTERED 
	(
		Id
	)
)