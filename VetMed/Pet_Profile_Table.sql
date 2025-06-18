CREATE TABLE [dbo].[Pet_Profile_Table]
(
	[PetProfile_Id] INT NOT NULL PRIMARY KEY, 
    [PetName] NVARCHAR(50) NOT NULL, 
    [Animal] NVARCHAR(50) NOT NULL, 
    [Breed] NVARCHAR(50) NOT NULL, 
    [KnownComplications] NVARCHAR(50) NULL, 
    [Age] NVARCHAR(50) NOT NULL, 
    [Weight] NVARCHAR(50) NOT NULL, 
    [Height] NVARCHAR(50) NOT NULL, 
    [Prescriptions] NVARCHAR(50) NULL
)
