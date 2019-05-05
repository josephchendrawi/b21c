CREATE TABLE [dbo].[ROLE](
	Id bigint IDENTITY(1,1) NOT NULL,
	CreatedBy [bigint] NULL,
	CreatedAt [datetime] NULL,
	LastUpdBy [bigint] NULL,
	LastUpdAt [datetime] NULL,
	Status [nvarchar](50) NULL,
	
	Name nvarchar(50),
	Description nvarchar(500),

	CONSTRAINT ROLE_P01 PRIMARY KEY CLUSTERED 
	(
		Id
	)
)
