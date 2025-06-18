CREATE TABLE [dbo].[Login_table] (
    [Login_Id]    INT          IDENTITY (1, 1) NOT NULL,
    [Username_Id] NCHAR (10)   NOT NULL,
    [Password_Id] NVARCHAR(50)   NOT NULL,
    [Email]       TEXT         NOT NULL,
    [Number]      NCHAR (10)   NOT NULL,
    [Address]     VARCHAR (50) NOT NULL,
    [State]       NCHAR (10)   NOT NULL,
    [Zip]         NCHAR (10)   NOT NULL,
    PRIMARY KEY CLUSTERED ([Login_Id] ASC)
);

