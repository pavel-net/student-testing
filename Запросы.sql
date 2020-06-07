select * from Disciplines
select * from Topics

select * from Questions
select * from Answers

select * from Teachers
select * from Students

insert into Teachers(login, password) values ('lol', '123')

select * from Tests
select * from TestResults
select * from TestResultAnswerTable

drop Database TestDb


insert into Tests (Name, IdTopic, CountQuestions) values ('test MathAn', 2, 5)
insert into TestResults (IdTest, IdStudent, TotalScore, TestDate, Duration, Rating1, Rating2, Rating3)
	values (1, 3, 100, GETDATE(), 12, 'A', 4.0, N'Хорошо'),
	(1, 3, 100, GETDATE(), 1, 'A', 4.0, N'Хорошо'),
	(1, 3, 200, GETDATE(), 22, 'A-', 4.0, N'Хорошо'),
	(1, 3, 300, GETDATE(), 243, 'A', 4.0, N'Хорошо')

insert into Students (Name, Surname, MiddleName, GroupNumber) values ('Валенок1', 'Валенок2','Валенок3', '714')
insert into Students (Name, Surname, MiddleName, GroupNumber) values ('Валенок 2', 'Валенок 2','Валенок 2', '714')
insert into Students (Name, Surname, MiddleName, GroupNumber) values ('Валенок 3', 'Валенок','Валенок', '714')

delete Questions where Id = 1

insert into Disciplines (Name) values (N'Биология'), (N'Химия'), (N'Физика'), (N'Философия'), (N'Математика'), (N'Программирование')

insert into Topics (Name, IdDiscipline) values (N'Информатика', 1003), (N'Основы C#', 1003)

insert into Questions (IdTopic, Content, Duration, Score, Hint) 
values (2, N'Сколько планет входит в солнечную систему.', 60, 10, NULL)
insert into Questions (IdTopic, Content, Duration, Score, Hint) 
values (2, N'Какую форму имеет планета Земля', 120, 15, N'Думай сам :)')

insert into Answers(IdQuestion, Content, FlagCorrectly)
values (20005, N'ответ 1 млн', 'N')
insert into Answers(IdQuestion, Content, FlagCorrectly)
values (1, 8, 'Y')

Update Answers set FlagCorrectly = 'N' where Id in (11,12)
Update Answers set ContentImage = q.ContentImage 
	from Answers a
	join Questions q on q.Id = a.IdQuestion
	where a.Id in (12)

delete Topics where Id = 1
delete Disciplines where Id = 1