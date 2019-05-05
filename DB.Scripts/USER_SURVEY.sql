CREATE TABLE [dbo].[USER_SURVEY](
	Id bigint IDENTITY(1,1) NOT NULL,
	CreatedBy [bigint] NULL,
	CreatedAt [datetime] NULL,
	LastUpdBy [bigint] NULL,
	LastUpdAt [datetime] NULL,
	Status [nvarchar](50) NULL,
	
	UserId bigint,
	SurveyId bigint,

	CONSTRAINT USER_SURVEY_P01 PRIMARY KEY CLUSTERED 
	(
		Id
	)
)

alter table [USER_SURVEY] add CONSTRAINT USER_SURVEY_F01 FOREIGN KEY ([UserId]) REFERENCES [USER](Id)
alter table [USER_SURVEY] add CONSTRAINT USER_SURVEY_F02 FOREIGN KEY ([SurveyId]) REFERENCES [SURVEY](Id)