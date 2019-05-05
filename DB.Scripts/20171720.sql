alter table [Product]
add CategoryId bigint 

alter table [Product] add CONSTRAINT PRODUCT_PRODUCT_CATEGORY_F01 FOREIGN KEY ([CategoryId]) REFERENCES PRODUCT_CATEGORY(Id)