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
    public class PokedexRepository : IPokedexRepository
    {
        private const string InformationalMessageWithCount = Constants.Retrieved + " {0} {1} " + Constants.From + " " + Constants.DBContext + ".";
        private const string InfoMessageWithId = "{0} {1} {2} " + Constants.DBContext + Constants.WithId + "{3}";
        private const string InfoMessageWithSearchCriteria = Constants.Retrieved + " {0} " + Constants.Pokemon + " " + Constants.From + " " + Constants.DBContext + " matching search string: {1}";
        private const StringComparison ignoreCase = StringComparison.CurrentCultureIgnoreCase;

        private POKEDEXDBContext _context;
        private ILoggerAdapter<PokedexRepository> _logger;
        public PokedexRepository(POKEDEXDBContext context, ILoggerAdapter<PokedexRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        /// <summary>
        /// Add the given Pokémon to Pokédex context & save changes.
        /// </summary>
        /// <param name="pokemon">entity to add</param>
        /// <returns>the added entity</returns>
        public async Task<tblMyPokedex> AddPokemon(tblMyPokedex pokemon)
        {
            await _context.AddAsync(pokemon);

            await _context.SaveChangesAsync();
            
            _logger.LogInformation(string.Format(InfoMessageWithId, Constants.Added, Constants.Pokemon, Constants.To, pokemon.Id));

            return pokemon;
        }

        /// <summary>
        /// Delete the given Pokémon from Pokédex context & save changes.
        /// </summary>
        /// <param name="myPokemonId">guid to find & delete</param>
        /// <returns>deleted entity</returns>
        public async Task<tblMyPokedex> DeletePokemonById(Guid myPokemonId)
        {
            tblMyPokedex myPokemon = await GetMyPokemonById(myPokemonId);

            if (myPokemon != null)
            {
                _context.Remove(myPokemon);

                await _context.SaveChangesAsync();

                _logger.LogInformation(string.Format(InfoMessageWithId, Constants.Deleted, Constants.Pokemon, Constants.From, myPokemonId));
            }

            return myPokemon;
        }

        /// <summary>
        /// Delete & re-add updated entity to Pokédex context.
        /// </summary>
        /// <param name="pokemon">entity to update</param>
        /// <returns>updated entity</returns>
        public async Task<tblMyPokedex> EditPokemon(tblMyPokedex pokemon)
        {
            tblMyPokedex myPokemon = await DeletePokemonById(pokemon.Id);
            
            if(myPokemon != null)
            {
                await AddPokemon(pokemon);

                _logger.LogInformation(string.Format(InfoMessageWithId, Constants.Updated, Constants.Pokemon, Constants.In, pokemon.Id));
            }

            return pokemon;
        }

        /// <summary>
        /// Get the ability entity from a given abilityId.
        /// </summary>
        /// <param name="abilityId">abilityId to find</param>
        /// <returns>the found ability entity</returns>
        public async Task<tlkpAbility> GetAbilityById(int abilityId)
        {
            tlkpAbility ability = await _context.tlkpAbility.FindAsync(abilityId);

            if(ability != null)
            {
                _logger.LogInformation(string.Format(InfoMessageWithId, Constants.Retrieved, Constants.Ability, Constants.From, abilityId));
            }

            return ability;
        }

        /// <summary>
        /// Get all the ability entities from context. Results ordered by Name ascending.
        /// </summary>
        /// <returns>all ability entities</returns>
        public async Task<List<tlkpAbility>> GetAllAbilities()
        {
            List<tlkpAbility> abilities = await _context.tlkpAbility.ToListAsync(); 
            abilities = abilities.OrderBy(a => a.Name).ToList();
            
            _logger.LogInformation(string.Format(InformationalMessageWithCount,abilities.Count, Constants.Abilities));

            return abilities;
        }

        /// <summary>
        /// Get all the category entities from context. Results ordered by Name ascending.
        /// </summary>
        /// <returns>all category entities</returns>
        public async Task<List<tlkpCategory>> GetAllCategories()
        {
            List<tlkpCategory> categories = await _context.tlkpCategory.ToListAsync(); 
            categories = categories.OrderBy(c => c.Name).ToList();

            _logger.LogInformation(string.Format(InformationalMessageWithCount, categories.Count, Constants.Categories));

            return categories;
        }

        /// <summary>
        /// Get all Pokéball entities from context. Results ordered by Name ascending.
        /// </summary>
        /// <returns>all Pokéball entities</returns>
        public async Task<List<tlkpPokeball>> GetAllPokeballs()
        {
            List<tlkpPokeball> pokeballs = await _context.tlkpPokeball.ToListAsync(); 
            pokeballs = pokeballs.OrderBy(p => p.Name).ToList();

            _logger.LogInformation(string.Format(InformationalMessageWithCount, pokeballs.Count, Constants.Pokeballs));

            return pokeballs;
        }

        /// <summary>
        /// Get all type entities from context. Results ordered by Name ascending.
        /// </summary>
        /// <returns>all type entities</returns>
        public async Task<List<tlkpType>> GetAllTypes()
        {
            List<tlkpType> types = await _context.tlkpType.ToListAsync(); 
            types = types.OrderBy(t => t.Name).ToList();

            _logger.LogInformation(string.Format(InformationalMessageWithCount, types.Count, Constants.Types));

            return types;
        }

        /// <summary>
        /// Get the category entity from a given categoryId.
        /// </summary>
        /// <param name="categoryId">categoryId to find</param>
        /// <returns>the found category entity</returns>
        public async Task<tlkpCategory> GetCategoryById(int categoryId)
        {
            tlkpCategory category = await _context.tlkpCategory.FindAsync(categoryId);

            if(category != null)
            {
                _logger.LogInformation(string.Format(InfoMessageWithId, Constants.Retrieved, Constants.Category, Constants.From, categoryId));
            }

            return category;
        }

        /// <summary>
        /// Get all the Pokédex entities with lookup information from context. Results ordered by PokemonId ascending.
        /// </summary>
        /// <returns>all Pokédex entities</returns>
        public async Task<List<tblMyPokedex>> GetMyPokedex()
        {
            List<tblMyPokedex> myPokedex = await _context.tblMyPokedex.ToListAsync(); 
            myPokedex = myPokedex.OrderBy(p => p.PokemonId).ToList();

            //Grab all nested lookup data
            foreach (tblMyPokedex pokemon in myPokedex)
            {
                pokemon.Pokemon = await GetNationalDexPokemonById(pokemon.PokemonId);
                pokemon.Pokeball = await GetPokeballById(pokemon.PokeballId.Value);
            }

            _logger.LogInformation(string.Format(InformationalMessageWithCount, myPokedex.Count, Constants.Pokemon));

            return myPokedex;
        }

        /// <summary>
        /// Get the Pokédex entity from a given myPokemonId with lookup information from context.
        /// </summary>
        /// <param name="myPokemonId">the myPokemonId to find</param>
        /// <returns>the found Pokédex entity</returns>
        public async Task<tblMyPokedex> GetMyPokemonById(Guid myPokemonId)
        {
            tblMyPokedex myPokemon = await _context.tblMyPokedex.FindAsync(myPokemonId);

            if(myPokemon != null)
            {
                //Grab nested lookup data
                myPokemon.Pokemon = await GetNationalDexPokemonById(myPokemon.PokemonId);
                myPokemon.Pokeball = await GetPokeballById(myPokemon.PokeballId.Value);

                _logger.LogInformation(string.Format(InfoMessageWithId, Constants.Retrieved, Constants.Pokemon, Constants.From, myPokemonId));
            }

            return myPokemon;
        }

        /// <summary>
        /// Get all national dex entities and subsequent lookup information from context. Results ordered Id ascending.
        /// </summary>
        /// <returns>all national dex Pokémon with related lookup information</returns>
        public async Task<List<tlkpNationalDex>> GetNationalDex()
        {
            List<tlkpNationalDex> nationalDex = await _context.tlkpNationalDex.ToListAsync();

            //Grab all nested lookup data
            List<tlkpNationalDex> nestedNationalDex = new List<tlkpNationalDex>();
            foreach (tlkpNationalDex pokemon in nationalDex)
            {
                tlkpNationalDex nested = await GetNestedNationalDexInfo(pokemon);
                nestedNationalDex.Add(nested);
            }

            nestedNationalDex.OrderBy(p => p.Id);

            _logger.LogInformation(string.Format(InformationalMessageWithCount, nestedNationalDex.Count, Constants.Pokemon));

            return nestedNationalDex;
        }

        /// <summary>
        /// Get the national dex entity from a given pokemonId.
        /// </summary>
        /// <param name="pokemonId">the pokemonId to find</param>
        /// <returns>the found national dex Pokémon</returns>
        public async Task<tlkpNationalDex> GetNationalDexPokemonById(int pokemonId)
        {
            tlkpNationalDex nationalDexPokemon = await _context.tlkpNationalDex.FindAsync(pokemonId);

            if(nationalDexPokemon != null)
            {
                //Grab nested national dex lookup
                nationalDexPokemon = await GetNestedNationalDexInfo(nationalDexPokemon);

                _logger.LogInformation(string.Format(InfoMessageWithId, Constants.Retrieved, Constants.Pokemon, Constants.From, pokemonId));
            }

            return nationalDexPokemon;
        }

        /// <summary>
        /// Get the Pokéball entity from a given pokeballId.
        /// </summary>
        /// <param name="pokeballId">the pokeballId to find</param>
        /// <returns>the found Pokéball entity</returns>
        public async Task<tlkpPokeball> GetPokeballById(int pokeballId)
        {
            tlkpPokeball pokeball = await _context.tlkpPokeball.FindAsync(pokeballId);

            if(pokeball != null)
            {
                _logger.LogInformation(string.Format(InfoMessageWithId, Constants.Retrieved, Constants.Pokeball, Constants.From, pokeballId));
            }

            return pokeball;
        }

        /// <summary>
        /// Get the type entity from a given typeId.
        /// </summary>
        /// <param name="typeId">the typeId to find</param>
        /// <returns>the found type entity</returns>
        public async Task<tlkpType> GetTypeById(int typeId)
        {
            tlkpType type = await _context.tlkpType.FindAsync(typeId);

            if(type != null)
            {
                _logger.LogInformation(string.Format(InfoMessageWithId, Constants.Retrieved, Constants.Type, Constants.From, typeId));
            }

            return type;
        }

        /// <summary>
        /// Search the National Pokédex based on a searchString and lookup parameters.
        /// Results filtered by occurence of string in name, Japanese name, & description.
        /// Additional filtering applied based on optional Pokémon characteristics. Results ordered by Id ascending.
        /// </summary>
        /// <param name="searchString">the string to filter on</param>
        /// <param name="abilityId">optional ability characteristic</param>
        /// <param name="categoryId">otpinal category characteristic</param>
        /// <param name="typeId">optional type characteristic</param>
        /// <returns>filtered list of Pokémon from the National Pokédex</returns>
        public async Task<List<tlkpNationalDex>> Search(string searchString, int? abilityId, int? categoryId, int? typeId)
        {
            List<tlkpNationalDex> nationalDexSearchResults = await GetNationalDex();

            if (!string.IsNullOrWhiteSpace(searchString))
            {
                nationalDexSearchResults = nationalDexSearchResults.Where(p => p.Name.Contains(searchString, ignoreCase) || 
                    (p.JapaneseName != null && p.JapaneseName.Contains(searchString, ignoreCase))|| 
                    (p.Description != null && p.Description.Contains(searchString, ignoreCase))).ToList();
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
        /// Results filtered by occurence of string in name, Japanese name, description, & nickname.
        /// Additional filtering applied based on optional Pokémon characteristics.
        /// Results ordered by PokemonId ascending.
        /// </summary>
        /// <param name="searchString">the string to filter on</param>
        /// <param name="abilityId">optional ability characteristic</param>
        /// <param name="categoryId">otpinal category characteristic</param>
        /// <param name="typeId">optional type characteristic</param>
        /// <param name="pokeballId">optional Pokéball characteristic</param>
        /// <returns>a filtered list of Pokémon from the personal Pokédex</returns>
        public async Task<List<tblMyPokedex>> Search(string searchString, int? abilityId, int? categoryId, int? typeId, int? pokeballId)
        {
            List<tblMyPokedex> myPokedexSearchResults = await GetMyPokedex();

            if (!string.IsNullOrWhiteSpace(searchString))
            {
                myPokedexSearchResults = myPokedexSearchResults.Where(p => p.Pokemon.Name.Contains(searchString, ignoreCase)
                    || (p.Pokemon.JapaneseName != null && p.Pokemon.JapaneseName.Contains(searchString, ignoreCase))
                    || (p.Pokemon.Description != null && p.Pokemon.Description.Contains(searchString, ignoreCase))
                    || (p.Nickname != null && p.Nickname.Contains(searchString, ignoreCase))).ToList();
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
        /// <param name="nationalDexPokemon">the National Dex Pokémon to grab lookups for.</param>
        /// <returns>the National Dex Pokémon with lookups filled.</returns>
        private async Task<tlkpNationalDex> GetNestedNationalDexInfo(tlkpNationalDex nationalDexPokemon)
        {
            nationalDexPokemon.Ability = await GetAbilityById(nationalDexPokemon.AbilityId.Value);

            if (nationalDexPokemon.HiddenAbilityId.HasValue)
            {
                nationalDexPokemon.HiddenAbility = await GetAbilityById(nationalDexPokemon.HiddenAbilityId.Value);
            }

            nationalDexPokemon.Category = await GetCategoryById(nationalDexPokemon.CategoryId.Value);

            nationalDexPokemon.TypeOne = await GetTypeById(nationalDexPokemon.TypeOneId.Value);

            if (nationalDexPokemon.TypeTwoId.HasValue)
            {
                nationalDexPokemon.TypeTwo = await GetTypeById(nationalDexPokemon.TypeTwoId.Value);
            }

            return nationalDexPokemon;
        }
    }
}