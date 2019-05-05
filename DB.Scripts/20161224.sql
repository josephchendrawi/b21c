alter table [Order]
add ShippingCode nvarchar(100)

ALTER TABLE [Order]
  ALTER COLUMN Address NVARCHAR(max)
  
ALTER TABLE [Order]
  ALTER COLUMN TrackingNo NVARCHAR(max)