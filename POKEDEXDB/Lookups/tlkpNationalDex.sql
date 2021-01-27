CREATE TABLE [dbo].[tlkpNationalDex]
(
	[Id] INT NOT NULL PRIMARY KEY, 
    [Name] VARCHAR(MAX) NOT NULL, 
    [JapaneseName] VARCHAR(MAX) NULL, 
    [Description] VARCHAR(MAX) NULL, 
    [CategoryId] INT NULL, 
    [TypeOneId] INT NULL, 
    [TypeTwoId] INT NULL, 
    [AbilityId] INT NULL, 
    [HiddenAbilityId] INT NULL, 
    [HeightInInches] INT NOT NULL, 
    [WeightInPounds] INT NOT NULL, 
    [ImageURL] VARCHAR(MAX) NULL, 
    CONSTRAINT [FK_tlkpNationalDex_tlkpCategory] FOREIGN KEY ([CategoryId]) REFERENCES [tlkpCategory]([Id]),
	CONSTRAINT [FK_tlkpNationalDex_tlkpType1] FOREIGN KEY ([TypeOneId]) REFERENCES [tlkpType]([Id]),
	CONSTRAINT [FK_tlkpNationalDex_tlkpType2] FOREIGN KEY ([TypeTwoId]) REFERENCES [tlkpType]([Id]),
	CONSTRAINT [FK_tlkpNationalDex_tlkpAbility1] FOREIGN KEY ([AbilityId]) REFERENCES [tlkpAbility]([Id]),
	CONSTRAINT [FK_tlkpNationalDex_tlkpAbility2] FOREIGN KEY ([HiddenAbilityId]) REFERENCES [tlkpAbility]([Id])
)