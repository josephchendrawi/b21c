CREATE TABLE [dbo].[LOOKUP](
	Id bigint IDENTITY(1,1) NOT NULL,
	CreatedBy [bigint] NULL,
	CreatedAt [datetime] NULL,
	LastUpdBy [bigint] NULL,
	LastUpdAt [datetime] NULL,
	
	Type nvarchar(255),
	Name nvarchar(255),
	Value nvarchar(max),
	[Sequence] int,
	ActiveFlag bit,

	CONSTRAINT LOOKUP_P01 PRIMARY KEY CLUSTERED 
	(
		Id
	)
)