alter table [Order]
add BookedBy nvarchar(100),
	Shipping nvarchar(100),
	Discount decimal(18,2)
	
alter table [Product]
add Sales bit,
	SalesDiscount decimal(18,2)