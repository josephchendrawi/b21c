alter table [Order_Product]
add [Price] [decimal](18, 2),
	[Quantity] int,
	[Name] nvarchar(100),
	[Weight] int
	
alter table [ORDER]
add AdditionalDiscount [decimal](18, 2)

//////////////////////////////////

UPDATE
    order_product
SET
    order_product.price = P.Price,
	order_product.name = P.name,
	order_product.weight = P.weight
FROM
    order_product OP
INNER JOIN
    product P
ON 
    OP.ProductId = P.Id;

update order_product
set quantity = 1

update [order]
set additionaldiscount = discount

update [order]
set discount = 0
