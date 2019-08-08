CREATE TABLE [dbo].[tblMyPokedex]
(
	[Id] INT NOT NULL PRIMARY KEY, 
    [PokemonId] INT NOT NULL, 
    [Nickname] VARCHAR(MAX) NULL, 
    [Level] INT NULL, 
    [Sex] BIT NULL, 
    [Date] DATE NULL, 
    [Location] VARCHAR(MAX) NULL, 
    [PokeballId] INT NULL
	CONSTRAINT [FK_tblMyPokedex_tlkpNationalPokedex] FOREIGN KEY ([PokemonId]) REFERENCES [tlkpNationalDex]([Id]),
	CONSTRAINT [FK_tblMyPokedex_tlkpPokeball] FOREIGN KEY ([PokeballId]) REFERENCES [tlkpPokeball]([Id])
)
