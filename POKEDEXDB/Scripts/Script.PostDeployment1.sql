/*DELETE FROM [dbo].tblMyPokedex;*/
DELETE FROM [dbo].tlkpNationalDex;

:r .\PopulateTlkpAbility.sql
:r .\PopulateTlkpCategory.sql
:r .\PopulateTlkpPokeball.sql
:r .\PopulateTlkpType.sql
:r .\PopulateTlkpNationalDex.sql