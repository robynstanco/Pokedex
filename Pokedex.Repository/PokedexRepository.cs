using Microsoft.EntityFrameworkCore;
using Pokedex.Common;
using Pokedex.Data;
using Pokedex.Data.Models;
using Pokedex.Logging.Interfaces;
using Pokedex.Repository.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Pokedex.Repository
{
    /// <summary>
    /// The Pokédex Repository.
    /// </summary>
    public class PokedexRepository : IPokedexRepository
    {
        private const string InformationalMessageWithCount = Constants.Retrieved + " {0} {1} " + Constants.From + " " + Constants.DBContext + ".";
        private const string InfoMessageWithId = "{0} {1} {2} " + Constants.DBContext + Constants.WithId + "{3}";
        private const string InfoMessageWithSearchCriteria = Constants.Retrieved + " {0} " + Constants.Pokemon + " " + Constants.From + " " + Constants.DBContext + " matching search string: {1}";
        private const StringComparison IgnoreCase = StringComparison.CurrentCultureIgnoreCase;

        private readonly POKEDEXDBContext _context;
        private readonly ILoggerAdapter<PokedexRepository> _logger;
        public PokedexRepository(POKEDEXDBContext context, ILoggerAdapter<PokedexRepository> logger)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Add the given Pokémon to Pokédex context and save changes.
        /// </summary>
        /// <param name="pokemon">The entity to add.</param>
        /// <returns>The added entity.</returns>
        public async Task<tblMyPokedex> AddPokemon(tblMyPokedex pokemon)
        {
            await _context.AddAsync(pokemon);

            await _context.SaveChangesAsync();

            _logger.LogInformation(string.Format(InfoMessageWithId, Constants.Added, Constants.Pokemon, Constants.To, pokemon.Id));

            return pokemon;
        }

        /// <summary>
        /// Delete the given Pokémon from Pokédex context and save changes.
        /// </summary>
        /// <param name="myPokemonId">The guid to find and delete.</param>
        /// <returns>The deleted entity.</returns>
        public async Task<tblMyPokedex> DeletePokemonById(Guid myPokemonId)
        {
            var myPokemon = await GetMyPokemonById(myPokemonId);

            if (myPokemon == null)
            {
                return null;
            }

            _context.Remove(myPokemon);

            await _context.SaveChangesAsync();

            _logger.LogInformation(string.Format(InfoMessageWithId, Constants.Deleted, Constants.Pokemon, Constants.From, myPokemonId));

            return myPokemon;
        }

        /// <summary>
        /// Delete and re-add updated entity to Pokédex context.
        /// </summary>
        /// <param name="pokemon">The entity to update.</param>
        /// <returns>The updated entity.</returns>
        public async Task<tblMyPokedex> EditPokemon(tblMyPokedex pokemon)
        {
            var myPokemon = await DeletePokemonById(pokemon.Id);

            if (myPokemon == null)
            {
                return null;
            }

            await AddPokemon(pokemon);

            _logger.LogInformation(string.Format(InfoMessageWithId, Constants.Updated, Constants.Pokemon, Constants.In, pokemon.Id));

            return pokemon;
        }

        /// <summary>
        /// Get the ability entity from a given abilityId.
        /// </summary>
        /// <param name="abilityId">The abilityId to find.</param>
        /// <returns>The found ability entity.</returns>
        public async Task<tlkpAbility> GetAbilityById(int? abilityId)
        {
            var ability = await _context.tlkpAbility.FindAsync(abilityId);

            if (ability != null)
            {
                _logger.LogInformation(string.Format(InfoMessageWithId, Constants.Retrieved, Constants.Ability, Constants.From, abilityId));
            }

            return ability;
        }

        /// <summary>
        /// Get all the ability entities from context. Results ordered by Name ascending.
        /// </summary>
        /// <returns>All ability entities.</returns>
        public async Task<List<tlkpAbility>> GetAllAbilities()
        {
            var abilities = await _context.tlkpAbility.ToListAsync();
            abilities = abilities.OrderBy(a => a.Name).ToList();

            _logger.LogInformation(string.Format(InformationalMessageWithCount, abilities.Count, Constants.Abilities));

            return abilities;
        }

        /// <summary>
        /// Get the ability entities from context. Pagination applied.
        /// </summary>
        /// <param name="pageNumber">The page number.</param>
        /// <param name="pageSize">The page size.</param>
        /// <returns>All paginated ability entities.</returns>
        public async Task<List<tlkpAbility>> GetAllAbilities(int pageNumber, int pageSize)
        {
            var excludeRecords = (pageNumber * pageSize) - pageSize;

            var abilities = await _context.tlkpAbility.Skip(excludeRecords).Take(pageSize).ToListAsync();

            _logger.LogInformation(string.Format(InformationalMessageWithCount, abilities.Count, Constants.Abilities));

            return abilities;
        }

        /// <summary>
        /// Get all the category entities from context. Results ordered by Name ascending.
        /// </summary>
        /// <returns>All category entities.</returns>
        public async Task<List<tlkpCategory>> GetAllCategories()
        {
            var categories = await _context.tlkpCategory.ToListAsync();
            categories = categories.OrderBy(c => c.Name).ToList();

            _logger.LogInformation(string.Format(InformationalMessageWithCount, categories.Count, Constants.Categories));

            return categories;
        }

        /// <summary>
        /// Get the category entities from context. Pagination applied.
        /// </summary>
        /// <param name="pageNumber">The page number.</param>
        /// <param name="pageSize">The page size.</param>
        /// <returns>All paginated category entities.</returns>
        public async Task<List<tlkpCategory>> GetAllCategories(int pageNumber, int pageSize)
        {
            var excludeRecords = (pageNumber * pageSize) - pageSize;

            var categories = await _context.tlkpCategory.Skip(excludeRecords).Take(pageSize).ToListAsync();

            _logger.LogInformation(string.Format(InformationalMessageWithCount, categories.Count, Constants.Categories));

            return categories;
        }

        /// <summary>
        /// Get all Pokéball entities from context. Results ordered by Name ascending.
        /// </summary>
        /// <returns>All Pokéball entities.</returns>
        public async Task<List<tlkpPokeball>> GetAllPokeballs()
        {
            var pokeballs = await _context.tlkpPokeball.ToListAsync();
            pokeballs = pokeballs.OrderBy(p => p.Name).ToList();

            _logger.LogInformation(string.Format(InformationalMessageWithCount, pokeballs.Count, Constants.Pokeballs));

            return pokeballs;
        }

        /// <summary>
        /// Get all type entities from context. Results ordered by Name ascending.
        /// </summary>
        /// <returns>All type entities.</returns>
        public async Task<List<tlkpType>> GetAllTypes()
        {
            var types = await _context.tlkpType.ToListAsync();
            types = types.OrderBy(t => t.Name).ToList();

            _logger.LogInformation(string.Format(InformationalMessageWithCount, types.Count, Constants.Types));

            return types;
        }

        /// <summary>
        /// Get the category entity from a given categoryId.
        /// </summary>
        /// <param name="categoryId">The categoryId to find.</param>
        /// <returns>The found category entity.</returns>
        public async Task<tlkpCategory> GetCategoryById(int? categoryId)
        {
            var category = await _context.tlkpCategory.FindAsync(categoryId);

            if (category != null)
            {
                _logger.LogInformation(string.Format(InfoMessageWithId, Constants.Retrieved, Constants.Category, Constants.From, categoryId));
            }

            return category;
        }

        /// <summary>
        /// Get all the Pokédex entities with lookup information from context. Results ordered by PokemonId ascending.
        /// </summary>
        /// <returns>All Pokédex entities.</returns>
        public async Task<List<tblMyPokedex>> GetMyPokedex()
        {
            var myPokedex = await _context.tblMyPokedex.ToListAsync();
            myPokedex = myPokedex.OrderBy(p => p.PokemonId).ToList();

            //Grab all nested lookup data
            foreach (var pokemon in myPokedex)
            {
                pokemon.Pokemon = await GetNationalDexPokemonById(pokemon.PokemonId);
                pokemon.Pokeball = await GetPokeballById(pokemon.PokeballId);
            }

            _logger.LogInformation(string.Format(InformationalMessageWithCount, myPokedex.Count, Constants.Pokemon));

            return myPokedex;
        }

        /// <summary>
        /// Get the Pokédex entity from a given myPokemonId with lookup information from context.
        /// </summary>
        /// <param name="myPokemonId">The myPokemonId to find.</param>
        /// <returns>The found Pokédex entity.</returns>
        public async Task<tblMyPokedex> GetMyPokemonById(Guid myPokemonId)
        {
            var myPokemon = await _context.tblMyPokedex.FindAsync(myPokemonId);

            if (myPokemon == null)
            {
                return null;
            }

            //Grab nested lookup data
            myPokemon.Pokemon = await GetNationalDexPokemonById(myPokemon.PokemonId);
            myPokemon.Pokeball = await GetPokeballById(myPokemon.PokeballId);

            _logger.LogInformation(string.Format(InfoMessageWithId, Constants.Retrieved, Constants.Pokemon, Constants.From, myPokemonId));

            return myPokemon;
        }

        /// <summary>
        /// Get all national dex entities and subsequent lookup information from context. Results ordered Id ascending.
        /// </summary>
        /// <returns>All national dex Pokémon with related lookup information.</returns>
        public async Task<List<tlkpNationalDex>> GetNationalDex()
        {
            var nationalDex = await _context.tlkpNationalDex.ToListAsync();

            //Grab all nested lookup data
            var nestedNationalDex = new List<tlkpNationalDex>();
            foreach (var pokemon in nationalDex)
            {
                var nested = await GetNestedNationalDexInfo(pokemon);
                nestedNationalDex.Add(nested);
            }

            nestedNationalDex = nestedNationalDex.OrderBy(p => p.Id).ToList();

            _logger.LogInformation(string.Format(InformationalMessageWithCount, nestedNationalDex.Count, Constants.Pokemon));

            return nestedNationalDex;
        }

        /// <summary>
        /// Get the national dex entity from a given pokemonId.
        /// </summary>
        /// <param name="pokemonId">The pokemonId to find.</param>
        /// <returns>The found national dex Pokémon.</returns>
        public async Task<tlkpNationalDex> GetNationalDexPokemonById(int pokemonId)
        {
            var nationalDexPokemon = await _context.tlkpNationalDex.FindAsync(pokemonId);

            if (nationalDexPokemon == null)
            {
                return null;
            }

            //Grab nested national dex lookup
            nationalDexPokemon = await GetNestedNationalDexInfo(nationalDexPokemon);

            _logger.LogInformation(string.Format(InfoMessageWithId, Constants.Retrieved, Constants.Pokemon, Constants.From, pokemonId));

            return nationalDexPokemon;
        }

        /// <summary>
        /// Get the Pokéball entity from a given pokeballId.
        /// </summary>
        /// <param name="pokeballId">The pokeballId to find.</param>
        /// <returns>The found Pokéball entity.</returns>
        public async Task<tlkpPokeball> GetPokeballById(int? pokeballId)
        {
            var pokeball = await _context.tlkpPokeball.FindAsync(pokeballId);

            if (pokeball != null)
            {
                _logger.LogInformation(string.Format(InfoMessageWithId, Constants.Retrieved, Constants.Pokeball, Constants.From, pokeballId));
            }

            return pokeball;
        }

        /// <summary>
        /// Get the type entity from a given typeId.
        /// </summary>
        /// <param name="typeId">The typeId to find.</param>
        /// <returns>The found type entity.</returns>
        public async Task<tlkpType> GetTypeById(int? typeId)
        {
            var type = await _context.tlkpType.FindAsync(typeId);

            if (type != null)
            {
                _logger.LogInformation(string.Format(InfoMessageWithId, Constants.Retrieved, Constants.Type, Constants.From, typeId));
            }

            return type;
        }

        /// <summary>
        /// Search the National Pokédex based on a searchString and lookup parameters.
        /// Results filtered by occurrence of string in name, Japanese name, and description.
        /// Additional filtering applied based on optional Pokémon characteristics. Results ordered by Id ascending.
        /// </summary>
        /// <param name="searchString">The string to filter on.</param>
        /// <param name="abilityId">The optional ability characteristic.</param>
        /// <param name="categoryId">The optional category characteristic.</param>
        /// <param name="typeId">The optional type characteristic.</param>
        /// <returns>The filtered list of Pokémon from the National Pokédex.</returns>
        public async Task<List<tlkpNationalDex>> Search(string searchString, int? abilityId, int? categoryId, int? typeId)
        {
            var nationalDexSearchResults = await GetNationalDex();

            if (!string.IsNullOrWhiteSpace(searchString))
            {
                nationalDexSearchResults = nationalDexSearchResults.Where(p => p.Name.Contains(searchString, IgnoreCase) ||
                    (p.JapaneseName != null && p.JapaneseName.Contains(searchString, IgnoreCase)) ||
                    (p.Description != null && p.Description.Contains(searchString, IgnoreCase))).ToList();
            }

            if (abilityId.HasValue)
            {
                nationalDexSearchResults = nationalDexSearchResults.Where(p => p.AbilityId == abilityId.Value || p.HiddenAbilityId == abilityId).ToList();
            }

            if (categoryId.HasValue)
            {
                nationalDexSearchResults = nationalDexSearchResults.Where(p => p.CategoryId == categoryId.Value).ToList();
            }

            if (typeId.HasValue)
            {
                nationalDexSearchResults = nationalDexSearchResults.Where(p => p.TypeOneId == typeId.Value || p.TypeTwoId == typeId.Value).ToList();
            }

            nationalDexSearchResults = nationalDexSearchResults.OrderBy(p => p.Id).ToList();

            _logger.LogInformation(string.Format(InfoMessageWithSearchCriteria, nationalDexSearchResults.Count, searchString));

            return nationalDexSearchResults;
        }

        /// <summary>
        /// Search the personal Pokédex based on a searchString and lookup parameters.
        /// Results filtered by occurrence of string in name, Japanese name, description, and nickname.
        /// Additional filtering applied based on optional Pokémon characteristics.
        /// Results ordered by PokemonId ascending.
        /// </summary>
        /// <param name="searchString">The string to filter on.</param>
        /// <param name="abilityId">The optional ability characteristic.</param>
        /// <param name="categoryId">The optional category characteristic.</param>
        /// <param name="typeId">The optional type characteristic.</param>
        /// <param name="pokeballId">The optional Pokéball characteristic.</param>
        /// <returns>The filtered list of Pokémon from the personal Pokédex.</returns>
        public async Task<List<tblMyPokedex>> Search(string searchString, int? abilityId, int? categoryId, int? typeId, int? pokeballId)
        {
            var myPokedexSearchResults = await GetMyPokedex();

            if (!string.IsNullOrWhiteSpace(searchString))
            {
                myPokedexSearchResults = myPokedexSearchResults.Where(p => p.Pokemon.Name.Contains(searchString, IgnoreCase)
                    || (p.Pokemon.JapaneseName != null && p.Pokemon.JapaneseName.Contains(searchString, IgnoreCase))
                    || (p.Pokemon.Description != null && p.Pokemon.Description.Contains(searchString, IgnoreCase))
                    || (p.Nickname != null && p.Nickname.Contains(searchString, IgnoreCase))).ToList();
            }

            if (abilityId.HasValue)
            {
                myPokedexSearchResults = myPokedexSearchResults.Where(p => p.Pokemon.AbilityId == abilityId.Value || p.Pokemon.HiddenAbilityId == abilityId).ToList();
            }

            if (categoryId.HasValue)
            {
                myPokedexSearchResults = myPokedexSearchResults.Where(p => p.Pokemon.CategoryId == categoryId.Value).ToList();
            }

            if (typeId.HasValue)
            {
                myPokedexSearchResults = myPokedexSearchResults.Where(p => p.Pokemon.TypeOneId == typeId.Value || p.Pokemon.TypeTwoId == typeId.Value).ToList();
            }

            if (pokeballId.HasValue)
            {
                myPokedexSearchResults = myPokedexSearchResults.Where(p => p.PokeballId == pokeballId.Value).ToList();
            }

            myPokedexSearchResults = myPokedexSearchResults.OrderBy(p => p.PokemonId).ToList();

            _logger.LogInformation(string.Format(InfoMessageWithSearchCriteria, myPokedexSearchResults.Count, searchString));

            return myPokedexSearchResults;
        }

        /// <summary>
        /// Get all the nested NationalDex lookup information for a given Pokémon.
        /// </summary>
        /// <param name="nationalDexPokemon">The National Dex Pokémon to grab lookups for.</param>
        /// <returns>The National Dex Pokémon with lookups filled.</returns>
        private async Task<tlkpNationalDex> GetNestedNationalDexInfo(tlkpNationalDex nationalDexPokemon)
        {
            nationalDexPokemon.Ability = await GetAbilityById(nationalDexPokemon.AbilityId);

            if (nationalDexPokemon.HiddenAbilityId.HasValue)
            {
                nationalDexPokemon.HiddenAbility = await GetAbilityById(nationalDexPokemon.HiddenAbilityId.Value);
            }

            nationalDexPokemon.Category = await GetCategoryById(nationalDexPokemon.CategoryId);

            nationalDexPokemon.TypeOne = await GetTypeById(nationalDexPokemon.TypeOneId);

            if (nationalDexPokemon.TypeTwoId.HasValue)
            {
                nationalDexPokemon.TypeTwo = await GetTypeById(nationalDexPokemon.TypeTwoId);
            }

            return nationalDexPokemon;
        }
    }
}