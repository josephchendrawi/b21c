CREATE TABLE [dbo].[USER_STATUS](
	Id bigint IDENTITY(1,1) NOT NULL,
	CreatedBy [bigint] NULL,
	CreatedAt [datetime] NULL,
	LastUpdBy [bigint] NULL,
	LastUpdAt [datetime] NULL,
	
	Name [nvarchar](100),
	Discount decimal(18,2),
	PointNeeded int,

	CONSTRAINT USER_STATUS_P01 PRIMARY KEY CLUSTERED 
	(
		Id
	)
)

Insert USER_STATUS(name, discount, pointneeded) values('New', 0, 0)