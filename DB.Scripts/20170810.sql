ALTER TABLE [USER_STATUS]
add PartnerDiscount decimal(18,2)

ALTER TABLE [SURVEY]
add Status nvarchar(50)

ALTER TABLE [USER_STATUS]
add PartnerName nvarchar(100)

ALTER TABLE [USER_STATUS]
add PartnerIconImage nvarchar(max)
