CREATE TABLE [dbo].[USER_BALANCE_TOPUP](
	Id bigint IDENTITY(1,1) NOT NULL,
	CreatedBy [bigint] NULL,
	CreatedAt [datetime] NULL,
	LastUpdBy [bigint] NULL,
	LastUpdAt [datetime] NULL,
	Status [nvarchar](50) NULL,
	
	AccountName [nvarchar](max),
	Amount decimal,
	Bank [nvarchar](100),
	TransferDateTime [datetime],
	
	UserId bigint,

	CONSTRAINT USER_BALANCE_TOPUP_P01 PRIMARY KEY CLUSTERED 
	(
		Id
	)
)

alter table [USER_BALANCE_TOPUP] add CONSTRAINT USER_BALANCE_TOPUP_USER_F01 FOREIGN KEY ([UserId]) REFERENCES [USER](Id)