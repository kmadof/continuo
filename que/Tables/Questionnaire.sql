CREATE TABLE [que].[Questionnaire]
(
	[Id] INT NOT NULL PRIMARY KEY, 
    [StorageDate] DATETIME NOT NULL DEFAULT GETDATE(), 
    [ModificationDate] DATETIME NULL
)
