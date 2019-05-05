CREATE TABLE [dbo].[USER_BALANCE_TRX](
	Id bigint IDENTITY(1,1) NOT NULL,
	CreatedBy [bigint] NULL,
	CreatedAt [datetime] NULL,
	LastUpdBy [bigint] NULL,
	LastUpdAt [datetime] NULL,
	
	TransactionDate DateTime,
	Amount decimal(18,2),
	Remarks [nvarchar](max),
	UserId bigint,

	CONSTRAINT USER_BALANCE_TRX_P01 PRIMARY KEY CLUSTERED 
	(
		Id
	)
)

alter table [USER_BALANCE_TRX] add CONSTRAINT USER_BALANCE_TRX_USER_F01 FOREIGN KEY ([UserId]) REFERENCES [USER](Id)