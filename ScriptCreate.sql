CREATE TABLE Disciplines (Id int primary key identity(1,1), Name nvarchar(128));

CREATE TABLE Topics (Id int primary key identity(1,1), Name nvarchar(256), 
	IdDiscipline int references Disciplines(Id));

CREATE TABLE Questions (Id bigint primary key identity(1,1),
	IdTopic int references Topics(Id) ON DELETE CASCADE,
	Content nvarchar(max),
	ContentImage varbinary(max),
	Duration int NOT NULL,
	Score int NOT NULL,
	Hint nvarchar(512),
	Hint2 nvarchar(512),
	Hint3 nvarchar(512))

CREATE TABLE Answers (Id bigint primary key identity(1,1),
	IdQuestion bigint references Questions(Id) ON DELETE CASCADE,
	Content nvarchar(max),
	ContentImage varbinary(max),
	FlagCorrectly char)

CREATE TABLE Tests (Id int primary key identity(1,1),
	Name nvarchar(256),
	IdTopic int references Topics(Id) ON DELETE CASCADE,
	IdDiscipline int references Disciplines(Id) ON DELETE CASCADE,
	CountQuestions int NOT NULL)

--CREATE TABLE TestTopicTable (Id bigint primary key identity(1,1),
--	IdTest int references Tests(Id),
--	IdTopic int references Topics(Id))

CREATE TABLE Teachers (Id int primary key identity(1,1), Fio nvarchar(256),
	Post nvarchar(128),
	Department nvarchar(128),
	Login nvarchar(32) NOT NULL,
	Password nvarchar(32))

CREATE TABLE Students (Id int primary key identity(1,1), 
	Name nvarchar(128),
	Surname nvarchar(128),
	MiddleName nvarchar(128),
	GroupNumber nvarchar(128))

CREATE TABLE TestResults (Id int primary key identity(1,1), 
	IdTest int references Tests(Id) ON DELETE CASCADE,
	IdStudent int references Students(Id) ON DELETE CASCADE,
	TotalScore int NOT NULL,
	TestDate datetime, 
	Duration int NOT NULL)

CREATE TABLE TestResultAnswerTable (Id bigint primary key identity(1,1),
	IdTestResult int references TestResults(Id),
	IdAnswer bigint references Answers(Id) ON DELETE CASCADE)

ALTER TABLE TestResults add Rating1 nvarchar(4), Rating2 float not null, Rating3 nvarchar(32)
ALTER TABLE Questions add HintImage varbinary(max), Hint2Image varbinary(max), Hint3Image varbinary(max)


DROP TABLE TestResultAnswerTable
DROP TABLE TestResults
DROP TABLE Answers
DROP TABLE Questions
DROP TABLE TestTopicTable
DROP TABLE Tests
DROP TABLE Topics
DROP TABLE Disciplines
DROP TABLE Students
DROP TABLE Teachers
