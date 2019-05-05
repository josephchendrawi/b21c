CREATE TABLE [dbo].[ADMIN](
	Id bigint IDENTITY(1,1) NOT NULL,
	CreatedBy [bigint] NULL,
	CreatedAt [datetime] NULL,
	LastUpdBy [bigint] NULL,
	LastUpdAt [datetime] NULL,
	Status [nvarchar](50) NULL,
	
	Username [nvarchar](200) NOT NULL,
	Password [nvarchar](200) NOT NULL,
	Name [nvarchar](100) NULL,

	CONSTRAINT ADMIN_P01 PRIMARY KEY CLUSTERED 
	(
		Id
	)
)