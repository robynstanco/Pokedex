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
        private const string InformationalMessageWithCount = Constants.Retrieved + " {0} {1} " + 
            Constants.From + " " + Constants.DBContext + ".";

        private const string InformationalMessageWithId = "{0} {1} {2} " + Constants.DBContext + 
            Constants.WithId + "{3}";

        private const string InformationalMessageWithSearchCriteria = Constants.Retrieved + " {0} " 
            + Constants.Pokemon + " " + Constants.From + " " + Constants.DBContext + " matching search string: {1}";

        private const StringComparison ignoreCase = StringComparison.CurrentCultureIgnoreCase;

        private POKEDEXDBContext _context;
        private ILoggerAdapter<PokedexRepository> _logger;
        public PokedexRepository(POKEDEXDBContext context, ILoggerAdapter<PokedexRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<tblMyPokedex> AddPokemon(tblMyPokedex pokemon)
        {
            await _context.AddAsync(pokemon);

            await _context.SaveChangesAsync();
            
            _logger.LogInformation(string.Format(InformationalMessageWithId, Constants.Added, 
                Constants.Pokemon, Constants.To, pokemon.Id));

            return pokemon;
        }

        public async Task<tblMyPokedex> DeletePokemonById(Guid myPokemonId)
        {
            tblMyPokedex myPokemon = await GetMyPokemonById(myPokemonId);

            if (myPokemon != null)
            {
                _context.Remove(myPokemon);

                await _context.SaveChangesAsync();

                _logger.LogInformation(string.Format(InformationalMessageWithId, Constants.Deleted, 
                    Constants.Pokemon, Constants.From, myPokemonId));
            }

            return myPokemon;
        }

        public async Task<tblMyPokedex> EditPokemon(tblMyPokedex pokemon)
        {
            tblMyPokedex myPokemon = await DeletePokemonById(pokemon.Id);
            
            if(myPokemon != null)
            {
                await AddPokemon(pokemon);

                _logger.LogInformation(string.Format(InformationalMessageWithId, Constants.Updated, 
                    Constants.Pokemon, Constants.In, pokemon.Id));
            }

            return pokemon;
        }

        public async Task<tlkpAbility> GetAbilityById(int abilityId)
        {
            tlkpAbility ability = await _context.tlkpAbility.FindAsync(abilityId);

            if(ability != null)
            {
                _logger.LogInformation(string.Format(InformationalMessageWithId, Constants.Retrieved,
                    Constants.Ability, Constants.From, abilityId));
            }

            return ability;
        }

        public async Task<List<tlkpAbility>> GetAllAbilities()
        {
            List<tlkpAbility> abilities = await _context.tlkpAbility.ToListAsync(); 
            abilities = abilities.OrderBy(a => a.Name).ToList();
            
            _logger.LogInformation(string.Format(InformationalMessageWithCount,
                abilities.Count, Constants.Abilities));

            return abilities;
        }

        public async Task<List<tlkpCategory>> GetAllCategories()
        {
            List<tlkpCategory> categories = await _context.tlkpCategory.ToListAsync(); 
            categories = categories.OrderBy(c => c.Name).ToList();

            _logger.LogInformation(string.Format(InformationalMessageWithCount, 
                categories.Count, Constants.Categories));

            return categories;
        }

        public async Task<List<tlkpPokeball>> GetAllPokeballs()
        {
            List<tlkpPokeball> pokeballs = await _context.tlkpPokeball.ToListAsync(); 
            pokeballs = pokeballs.OrderBy(p => p.Name).ToList();

            _logger.LogInformation(string.Format(InformationalMessageWithCount, 
                pokeballs.Count, Constants.Pokeballs));

            return pokeballs;
        }

        public async Task<List<tlkpType>> GetAllTypes()
        {
            List<tlkpType> types = await _context.tlkpType.ToListAsync(); 
            types = types.OrderBy(t => t.Name).ToList();

            _logger.LogInformation(string.Format(InformationalMessageWithCount,
                types.Count, Constants.Types));

            return types;
        }

        public async Task<tlkpCategory> GetCategoryById(int categoryId)
        {
            tlkpCategory category = await _context.tlkpCategory.FindAsync(categoryId);

            if(category != null)
            {
                _logger.LogInformation(string.Format(InformationalMessageWithId, 
                    Constants.Retrieved, Constants.Category, Constants.From, categoryId));
            }

            return category;
        }

        public async Task<List<tblMyPokedex>> GetMyPokedex()
        {
            List<tblMyPokedex> myPokedex = await _context.tblMyPokedex.ToListAsync(); 
            myPokedex = myPokedex.OrderBy(p => p.PokemonId).ToList();

            foreach (tblMyPokedex pokemon in myPokedex)
            {
                pokemon.Pokemon = await GetNationalDexPokemonById(pokemon.PokemonId);

                pokemon.Pokeball = await GetPokeballById(pokemon.PokeballId.Value);
            }

            _logger.LogInformation(string.Format(InformationalMessageWithCount, 
                myPokedex.Count, Constants.Pokemon));

            return myPokedex;
        }

        public async Task<tblMyPokedex> GetMyPokemonById(Guid myPokemonId)
        {
            tblMyPokedex myPokemon = await _context.tblMyPokedex.FindAsync(myPokemonId);

            if(myPokemon != null)
            {
                myPokemon.Pokemon = await GetNationalDexPokemonById(myPokemon.PokemonId);

                myPokemon.Pokeball = await GetPokeballById(myPokemon.PokeballId.Value);

                _logger.LogInformation(string.Format(InformationalMessageWithId, Constants.Retrieved, 
                    Constants.Pokemon, Constants.From, myPokemonId));
            }

            return myPokemon;
        }

        public async Task<List<tlkpNationalDex>> GetNationalDex()
        {
            List<tlkpNationalDex> nationalDex = await _context.tlkpNationalDex.ToListAsync();

            List<tlkpNationalDex> nestedNationalDex = new List<tlkpNationalDex>();
            foreach (tlkpNationalDex pokemon in nationalDex)
            {
                tlkpNationalDex nested = await GetNestedNationalDexInfo(pokemon);
                nestedNationalDex.Add(nested);
            }

            nestedNationalDex.OrderBy(p => p.Id);

            _logger.LogInformation(string.Format(InformationalMessageWithCount, 
                nestedNationalDex.Count, Constants.Pokemon));

            return nestedNationalDex;
        }

        public async Task<tlkpNationalDex> GetNationalDexPokemonById(int pokemonId)
        {
            tlkpNationalDex nationalDexPokemon = await _context.tlkpNationalDex.FindAsync(pokemonId);

            if(nationalDexPokemon != null)
            {
                nationalDexPokemon = await GetNestedNationalDexInfo(nationalDexPokemon);

                _logger.LogInformation(string.Format(InformationalMessageWithId, Constants.Retrieved, 
                    Constants.Pokemon, Constants.From, pokemonId));
            }

            return nationalDexPokemon;
        }

        private async Task<tlkpNationalDex> GetNestedNationalDexInfo(tlkpNationalDex nationalDexPokemon)
        {
            nationalDexPokemon.Ability = await GetAbilityById(nationalDexPokemon.AbilityId.Value);

            nationalDexPokemon.Category = await GetCategoryById(nationalDexPokemon.CategoryId.Value);

            if (nationalDexPokemon.HiddenAbilityId.HasValue)
            {
                nationalDexPokemon.HiddenAbility = await GetAbilityById(nationalDexPokemon.HiddenAbilityId.Value);
            }

            nationalDexPokemon.TypeOne = await GetTypeById(nationalDexPokemon.TypeOneId.Value);

            if (nationalDexPokemon.TypeTwoId.HasValue)
            {
                nationalDexPokemon.TypeTwo = await GetTypeById(nationalDexPokemon.TypeTwoId.Value);
            }

            return nationalDexPokemon;
        }

        public async Task<tlkpPokeball> GetPokeballById(int pokeballId)
        {
            tlkpPokeball pokeball = await _context.tlkpPokeball.FindAsync(pokeballId);

            if(pokeball != null)
            {
                _logger.LogInformation(string.Format(InformationalMessageWithId, 
                    Constants.Retrieved, Constants.Pokeball, Constants.From, pokeballId));
            }

            return pokeball;
        }

        public async Task<tlkpType> GetTypeById(int typeId)
        {
            tlkpType type = await _context.tlkpType.FindAsync(typeId);

            if(type != null)
            {
                _logger.LogInformation(string.Format(InformationalMessageWithId, 
                    Constants.Retrieved, Constants.Type, Constants.From, typeId));
            }

            return type;
        }

        public async Task<List<tlkpNationalDex>> Search(string searchString, int? selectedAbilityId, 
            int? selectedCategoryId, int? selectedTypeId)
        {
            List<tlkpNationalDex> nationalDexSearchResults = await GetNationalDex();

            if (!string.IsNullOrWhiteSpace(searchString))
            {
                nationalDexSearchResults = nationalDexSearchResults
                    .Where(p => p.Name.Contains(searchString, ignoreCase) || 
                    (p.JapaneseName != null && p.JapaneseName.Contains(searchString, ignoreCase))|| 
                    (p.Description != null && p.Description.Contains(searchString, ignoreCase))).ToList();
            }

            if (selectedAbilityId.HasValue)
            {
                nationalDexSearchResults = nationalDexSearchResults
                    .Where(p => p.AbilityId == selectedAbilityId.Value || p.HiddenAbilityId == selectedAbilityId)
                    .ToList();
            }

            if (selectedCategoryId.HasValue)
            {
                nationalDexSearchResults = nationalDexSearchResults
                    .Where(p => p.CategoryId == selectedCategoryId.Value)
                    .ToList();
            }

            if (selectedTypeId.HasValue)
            {
                nationalDexSearchResults = nationalDexSearchResults
                    .Where(p => p.TypeOneId == selectedTypeId.Value || p.TypeTwoId == selectedTypeId.Value)
                    .ToList();
            }

            _logger.LogInformation(string.Format(InformationalMessageWithSearchCriteria, 
                nationalDexSearchResults.ToList().Count, searchString));

            return nationalDexSearchResults.OrderBy(p => p.Id).ToList();
        }

        public async Task<List<tblMyPokedex>> Search(string searchString, int? selectedAbilityId, 
            int? selectedCategoryId, int? selectedTypeId, int? selectedPokeballId)
        {
            List<tblMyPokedex> myPokedexSearchResults = await GetMyPokedex();

            if (!string.IsNullOrWhiteSpace(searchString))
            {
                myPokedexSearchResults = myPokedexSearchResults
                    .Where(p => p.Pokemon.Name.Contains(searchString, ignoreCase)
                    || (p.Pokemon.JapaneseName != null && p.Pokemon.JapaneseName.Contains(searchString, ignoreCase))
                    || (p.Pokemon.Description != null && p.Pokemon.Description.Contains(searchString, ignoreCase))
                    || (p.Nickname != null && p.Nickname.Contains(searchString, ignoreCase)))
                    .ToList();
            }

            if (selectedAbilityId.HasValue)
            {
                myPokedexSearchResults = myPokedexSearchResults
                    .Where(p => p.Pokemon.AbilityId == selectedAbilityId.Value || p.Pokemon.HiddenAbilityId == selectedAbilityId)
                    .ToList();
            }

            if (selectedCategoryId.HasValue)
            {
                myPokedexSearchResults = myPokedexSearchResults
                    .Where(p => p.Pokemon.CategoryId == selectedCategoryId.Value)
                    .ToList();
            }

            if (selectedTypeId.HasValue)
            {
                myPokedexSearchResults = myPokedexSearchResults
                    .Where(p => p.Pokemon.TypeOneId == selectedTypeId.Value || p.Pokemon.TypeTwoId == selectedTypeId.Value)
                    .ToList();
            }

            if (selectedPokeballId.HasValue)
            {
                myPokedexSearchResults = myPokedexSearchResults
                    .Where(p => p.PokeballId == selectedPokeballId.Value)
                    .ToList();
            }

            _logger.LogInformation(string.Format(InformationalMessageWithSearchCriteria,
                myPokedexSearchResults.ToList().Count, searchString));

            return myPokedexSearchResults.OrderBy(p => p.PokemonId).ToList();
        }
    }
}