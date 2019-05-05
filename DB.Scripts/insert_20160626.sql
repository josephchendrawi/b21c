insert ROLE(Name) values('Super Admin')

insert ROLE_ACCESS(RoleId, AccessModule, Viewable, Editable, Addable, Deletable) values(1, 'Admin', 1, 1, 1, 1)

update [User]
set RoleId = 1
where id = 1