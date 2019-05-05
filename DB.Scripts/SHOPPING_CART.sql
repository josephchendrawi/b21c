CREATE TABLE [dbo].[SHOPPING_CART](
	Id bigint IDENTITY(1,1) NOT NULL,
	CreatedBy [bigint] NULL,
	CreatedAt [datetime] NULL,
	LastUpdBy [bigint] NULL,
	LastUpdAt [datetime] NULL,
	
	ProductId bigint,
	Quantity int,
	UserId bigint,

	CONSTRAINT SHOPPING_CART_P01 PRIMARY KEY CLUSTERED 
	(
		Id
	)
)

alter table [SHOPPING_CART] add CONSTRAINT SHOPPING_CART_USER_F01 FOREIGN KEY ([UserId]) REFERENCES [USER](Id)
alter table [SHOPPING_CART] add CONSTRAINT SHOPPING_CART_PRODUCT_F01 FOREIGN KEY ([ProductId]) REFERENCES [PRODUCT](Id)