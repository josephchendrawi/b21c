CREATE TABLE [dbo].[ROLE_ACCESS](
	CreatedBy [bigint] NULL,
	CreatedAt [datetime] NULL,
	LastUpdBy [bigint] NULL,
	LastUpdAt [datetime] NULL,
	Status [nvarchar](50) NULL,
	
	RoleId bigint,
	AccessModule [nvarchar](50),
	Viewable bit,
	Editable bit,
	Addable bit,
	Deletable bit,

	CONSTRAINT ROLE_ACCESS_P01 PRIMARY KEY CLUSTERED 
	(
		RoleId, AccessModule
	)
)

alter table [ROLE_ACCESS] add CONSTRAINT ROLE_ACCESS_F01 FOREIGN KEY ([RoleId]) REFERENCES [ROLE](Id)