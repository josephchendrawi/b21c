CREATE TABLE [dbo].[ORDER](
	Id bigint IDENTITY(1,1) NOT NULL,
	CreatedBy [bigint] NULL,
	CreatedAt [datetime] NULL,
	LastUpdBy [bigint] NULL,
	LastUpdAt [datetime] NULL,
	Status [nvarchar](50) NULL,
	
	Sender [nvarchar](100) NULL,
	TotalWeight [int] NULL,
	ShippingFee [decimal](18,2) NULL,
	TotalPrice [decimal](18,2) NULL,
	OrderCode [nvarchar](100) NULL,
	PaymentMethod [nvarchar](100) NULL,
	ExpirationDate [datetime] NULL,
	
	Receiver [nvarchar](100) NULL,
	Address [nvarchar](100) NULL,
	ContactNo [nvarchar](100) NULL,
	FlgAdminLogo [bit] NULL,
	
	TrackingNo [nvarchar](100) NULL,

	CONSTRAINT ORDER_P01 PRIMARY KEY CLUSTERED 
	(
		Id
	)
)