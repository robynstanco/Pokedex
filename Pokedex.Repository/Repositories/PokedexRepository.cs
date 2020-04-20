using Pokedex.Common;
using Pokedex.Data;
using Pokedex.Data.Models;
using Pokedex.Logging.Interfaces;
using Pokedex.Repository.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Pokedex.Repository.Repositories
{
    public class PokedexRepository : IPokedexRepository
    {
        private const string InformationalMessageWithCount = Constants.Retrieved + " {0} {1} " + Constants.From + " " + Constants.DBContext + ".";
        private const string InformationalMessageWithId = "{0} {1} {2} " + Constants.DBContext + " with Id: {3}";
        private const string InformationalMessageWithSearchCriteria = Constants.Retrieved + " {0} " + Constants.Pokemon + " " 
            + Constants.From + " " + Constants.DBContext + " matching search string: {1}";
        
        private POKEDEXDBContext _context;
        private ILoggerAdapter<PokedexRepository> _logger;
        public PokedexRepository(POKEDEXDBContext context, ILoggerAdapter<PokedexRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        public void AddPokemon(tblMyPokedex pokemon)
        {
            _context.Add(pokemon);
            _context.SaveChanges();
            
            _logger.LogInformation(string.Format(InformationalMessageWithId, Constants.Added, Constants.Pokemon, Constants.To, pokemon.Id));
        }

        public void DeletePokemonById(Guid myPokemonId)
        {
            tblMyPokedex myPokemon = GetMyPokemonById(myPokemonId);
            _context.Remove(myPokemon);
            _context.SaveChanges();

            _logger.LogInformation(string.Format(InformationalMessageWithId, Constants.Deleted, Constants.Pokemon, Constants.From, myPokemonId));
        }

        public void EditPokemon(tblMyPokedex pokemon)
        {
            DeletePokemonById(pokemon.Id);
            AddPokemon(pokemon);

            _logger.LogInformation(string.Format(InformationalMessageWithId, Constants.Updated, Constants.Pokemon, Constants.In, pokemon.Id));
        }

        public tlkpAbility GetAbilityById(int abilityId)
        {
            tlkpAbility ability = _context.tlkpAbility.FirstOrDefault(a => a.Id == abilityId);

            _logger.LogInformation(string.Format(InformationalMessageWithId, Constants.Retrieved, Constants.Ability, Constants.From, abilityId));

            return ability;
        }

        public List<tlkpAbility> GetAllAbilities()
        {
            List<tlkpAbility> abilities = _context.tlkpAbility.OrderBy(a => a.Name).ToList();
            
            _logger.LogInformation(string.Format(InformationalMessageWithCount, abilities.Count, Constants.Abilities));

            return abilities;
        }

        public List<tlkpCategory> GetAllCategories()
        {
            List<tlkpCategory> categories = _context.tlkpCategory.OrderBy(c => c.Name).ToList();

            _logger.LogInformation(string.Format(InformationalMessageWithCount, categories.Count, Constants.Categories));

            return categories;
        }

        public List<tlkpPokeball> GetAllPokeballs()
        {
            List<tlkpPokeball> pokeballs = _context.tlkpPokeball.OrderBy(p => p.Name).ToList();

            _logger.LogInformation(string.Format(InformationalMessageWithCount, pokeballs.Count, Constants.Pokeballs));

            return pokeballs;
        }

        public List<tlkpType> GetAllTypes()
        {
            List<tlkpType> types = _context.tlkpType.OrderBy(t => t.Name).ToList();

            _logger.LogInformation(string.Format(InformationalMessageWithCount, types.Count, Constants.Types));

            return types;
        }

        public tlkpCategory GetCategoryById(int categoryId)
        {
            tlkpCategory category = _context.tlkpCategory.FirstOrDefault(c => c.Id == categoryId);

            _logger.LogInformation(string.Format(InformationalMessageWithId, Constants.Retrieved, Constants.Category, Constants.From, categoryId));

            return category;
        }

        public List<tblMyPokedex> GetMyPokedex()
        {
            List<tblMyPokedex> myPokedex = _context.tblMyPokedex.OrderBy(p => p.PokemonId).ToList();

            myPokedex.ForEach(p => p.Pokemon = GetNationalDexPokemonById(p.PokemonId));
            myPokedex.ForEach(p => p.Pokeball = GetPokeballById(p.PokeballId.Value));

            _logger.LogInformation(string.Format(InformationalMessageWithCount, myPokedex.Count, Constants.Pokemon));

            return myPokedex;
        }

        public tblMyPokedex GetMyPokemonById(Guid myPokemonId)
        {
            tblMyPokedex myPokemon = _context.tblMyPokedex.FirstOrDefault(p => p.Id == myPokemonId);

            myPokemon.Pokemon = GetNationalDexPokemonById(myPokemon.PokemonId);
            myPokemon.Pokeball = GetPokeballById(myPokemon.PokeballId.Value);

            _logger.LogInformation(string.Format(InformationalMessageWithId, Constants.Retrieved, Constants.Pokemon, Constants.From, myPokemonId));

            return myPokemon;
        }

        public List<tlkpNationalDex> GetNationalDex()
        {
            List<tlkpNationalDex> nationalDex = _context.tlkpNationalDex.OrderBy(p => p.Id).ToList();

            _logger.LogInformation(string.Format(InformationalMessageWithCount, nationalDex.Count, Constants.Pokemon));

            return nationalDex;
        }

        public tlkpNationalDex GetNationalDexPokemonById(int pokemonId)
        {
            tlkpNationalDex nationalDexPokemon = _context.tlkpNationalDex.FirstOrDefault(p => p.Id == pokemonId);

            nationalDexPokemon.Ability = GetAbilityById(nationalDexPokemon.AbilityId.Value);
            nationalDexPokemon.Category = GetCategoryById(nationalDexPokemon.CategoryId.Value);

            if (nationalDexPokemon.HiddenAbilityId.HasValue)
            {
                nationalDexPokemon.HiddenAbility = GetAbilityById(nationalDexPokemon.HiddenAbilityId.Value);
            }

            nationalDexPokemon.TypeOne = GetTypeById(nationalDexPokemon.TypeOneId.Value);

            if (nationalDexPokemon.TypeTwoId.HasValue)
            {
                nationalDexPokemon.TypeTwo = GetTypeById(nationalDexPokemon.TypeTwoId.Value);
            }

            _logger.LogInformation(string.Format(InformationalMessageWithId, Constants.Retrieved, Constants.Pokemon, Constants.From, pokemonId));

            return nationalDexPokemon;
        }

        public tlkpPokeball GetPokeballById(int pokeballId)
        {
            tlkpPokeball pokeball = _context.tlkpPokeball.FirstOrDefault(p => p.Id == pokeballId);

            _logger.LogInformation(string.Format(InformationalMessageWithId, Constants.Retrieved, Constants.Pokeball, Constants.From, pokeballId));

            return pokeball;
        }

        public tlkpType GetTypeById(int typeId)
        {
            tlkpType type = _context.tlkpType.FirstOrDefault(t => t.Id == typeId);

            _logger.LogInformation(string.Format(InformationalMessageWithId, Constants.Retrieved, Constants.Type, Constants.From, typeId));

            return type;
        }

        public List<tlkpNationalDex> Search(string searchString, int? selectedAbilityId, int? selectedCategoryId, int? selectedTypeId)
        {
            List<tlkpNationalDex> nationalDexSearchResults = GetNationalDex();

            if (!string.IsNullOrWhiteSpace(searchString))
            {
                nationalDexSearchResults = nationalDexSearchResults.Where(p => p.Name.Contains(searchString, StringComparison.CurrentCultureIgnoreCase)
                    || (p.JapaneseName != null && p.JapaneseName.Contains(searchString, StringComparison.CurrentCultureIgnoreCase))
                    || (p.Description != null && p.Description.Contains(searchString, StringComparison.CurrentCultureIgnoreCase)))
                    .ToList();
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

            _logger.LogInformation(string.Format(InformationalMessageWithSearchCriteria, nationalDexSearchResults.ToList().Count, searchString));

            return nationalDexSearchResults.OrderBy(p => p.Id).ToList();
        }

        public List<tblMyPokedex> Search(string searchString, int? selectedAbilityId, int? selectedCategoryId, int? selectedTypeId, int? selectedPokeballId)
        {
            List<tblMyPokedex> myPokedexSearchResults = GetMyPokedex();

            if (!string.IsNullOrWhiteSpace(searchString))
            {
                myPokedexSearchResults = myPokedexSearchResults
                    .Where(p => p.Pokemon.Name.Contains(searchString, StringComparison.CurrentCultureIgnoreCase)
                    || (p.Pokemon.JapaneseName != null && p.Pokemon.JapaneseName.Contains(searchString, StringComparison.CurrentCultureIgnoreCase))
                    || (p.Pokemon.Description != null && p.Pokemon.Description.Contains(searchString, StringComparison.CurrentCultureIgnoreCase))
                    || (p.Nickname != null && p.Nickname.Contains(searchString, StringComparison.CurrentCultureIgnoreCase)))
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

            _logger.LogInformation(string.Format(InformationalMessageWithSearchCriteria, myPokedexSearchResults.ToList().Count, searchString));

            return myPokedexSearchResults.OrderBy(p => p.PokemonId).ToList();
        }
    }
}