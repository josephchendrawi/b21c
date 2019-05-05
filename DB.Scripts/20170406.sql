alter table [ORDER]
add City nvarchar(255),
	Province nvarchar(255),
	Subdistrict nvarchar(255),
	SubdistrictId int
	
alter table [PRODUCT]
add OldStock int
	
EXEC sp_rename 'ADMIN', 'USER';  

sp_rename 'USER.AdminType', 'Type', 'COLUMN';

alter table [USER]
add Phone nvarchar(100),
	BirthDate DateTime,
	Gender nvarchar(50),
	Balance decimal(18,2),
	Point int,
	UserStatusId bigint
	
alter table [USER] add CONSTRAINT USER_USER_STATUS_F01 FOREIGN KEY ([UserStatusId]) REFERENCES [USER_STATUS](Id)

