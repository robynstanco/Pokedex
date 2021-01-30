In Nuget Package Manager Console run the following command in context of the Pokedex.Data project:
	Scaffold-DbContext "Data Source=DESKTOP-OTPCPIS;initial catalog=POKEDEXDB;integrated security=True;" Microsoft.EntityFrameworkCore.SqlServer -UseDatabaseNames -Tables tlkpAbility, tlkpCategory, tlkpNationalDex, tlkpPokeball, tlkpType, tblMyPokedex -OutputDir Models -ContextDir . -Force

	Multiple EntityFrameworkCore nuget packages utilized.

	Please see https://docs.microsoft.com/en-us/ef/core/ for detailed docs.