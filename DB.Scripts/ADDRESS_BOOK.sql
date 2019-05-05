CREATE TABLE [dbo].[ADDRESS_BOOK](
	Id bigint IDENTITY(1,1) NOT NULL,
	CreatedBy [bigint] NULL,
	CreatedAt [datetime] NULL,
	LastUpdBy [bigint] NULL,
	LastUpdAt [datetime] NULL,
	
	AddressName [nvarchar](100),
	Receiver [nvarchar](100),
	Address [nvarchar](max),
	Postcode [nvarchar](15),
	Province [nvarchar](100),
	City [nvarchar](100),
	Subdistrict [nvarchar](100),
	SubdistrictId int,
	Phone [nvarchar](100),
	UserId bigint,

	CONSTRAINT ADDRESS_BOOK_P01 PRIMARY KEY CLUSTERED 
	(
		Id
	)
)

alter table [ADDRESS_BOOK] add CONSTRAINT ADDRESS_BOOK_USER_F01 FOREIGN KEY ([UserId]) REFERENCES [USER](Id)