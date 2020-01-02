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
        //todo abstract out const reused in sln
        private const string DBContext = nameof(DBContext);
        private const string From = "from";
        private const string InformationalMessageWithCount = Retrieved + " {0} {1} " + From + " " + DBContext + ".";
        private const string InformationalMessageWithId = "{0} {1} {2} " + DBContext + " with Id: {3}";
        private const string InformationalMessageWithSearchCriteria = Retrieved + " {0} " + Pokemon + " " + From + " " + DBContext + " matching search string: {1}";
        private const string Pokemon = nameof(Pokemon);
        private const string Retrieved = nameof(Retrieved);
        
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
            
            _logger.LogInformation(string.Format(InformationalMessageWithId, "Added", Pokemon, "to", pokemon.Id));
        }

        public void DeletePokemonById(int pokemonId)
        {
            _context.Remove(GetMyPokemonById(pokemonId));
            _context.SaveChanges();

            _logger.LogInformation(string.Format(InformationalMessageWithId, "Deleted", Pokemon, From, pokemonId));
        }

        public void EditPokemon(tblMyPokedex pokemon)
        {
            _context.Update(pokemon);
            _context.SaveChanges();

            _logger.LogInformation(string.Format(InformationalMessageWithId, "Updated", Pokemon, "in", pokemon.Id));
        }

        public tlkpAbility GetAbilityById(int abilityId)
        {
            tlkpAbility ability = _context.tlkpAbility.FirstOrDefault(a => a.Id == abilityId);

            _logger.LogInformation(string.Format(InformationalMessageWithId, Retrieved, "Ability", From, abilityId));

            return ability;
        }

        public List<tlkpAbility> GetAllAbilities()
        {
            List<tlkpAbility> abilities = _context.tlkpAbility.OrderBy(a => a.Id).ToList();
            
            _logger.LogInformation(string.Format(InformationalMessageWithCount, abilities.Count, "Abilities"));

            return abilities;
        }

        public List<tlkpCategory> GetAllCategories()
        {
            List<tlkpCategory> categories = _context.tlkpCategory.OrderBy(c => c.Id).ToList();

            _logger.LogInformation(string.Format(InformationalMessageWithCount, categories.Count, "Categories"));

            return categories;
        }

        public List<tlkpPokeball> GetAllPokeballs()
        {
            List<tlkpPokeball> pokeballs = _context.tlkpPokeball.OrderBy(p => p.Id).ToList();

            _logger.LogInformation(string.Format(InformationalMessageWithCount, pokeballs.Count, "Pokeballs"));

            return pokeballs;
        }

        public List<tlkpType> GetAllTypes()
        {
            List<tlkpType> types = _context.tlkpType.OrderBy(t => t.Id).ToList();

            _logger.LogInformation(string.Format(InformationalMessageWithCount, types.Count, "Types"));

            return types;
        }

        public tlkpCategory GetCategoryById(int categoryId)
        {
            tlkpCategory category = _context.tlkpCategory.FirstOrDefault(c => c.Id == categoryId);

            _logger.LogInformation(string.Format(InformationalMessageWithId, Retrieved, "Category", From, categoryId));

            return category;
        }

        public List<tblMyPokedex> GetMyPokedex()
        {
            List<tblMyPokedex> myPokedex = _context.tblMyPokedex.OrderBy(p => p.PokemonId).ToList();

            _logger.LogInformation(string.Format(InformationalMessageWithCount, myPokedex.Count, Pokemon));

            return myPokedex;
        }

        public tblMyPokedex GetMyPokemonById(int pokemonId)
        {
            tblMyPokedex myPokemon = _context.tblMyPokedex.FirstOrDefault(p => p.Id == pokemonId);

            _logger.LogInformation(string.Format(InformationalMessageWithId, Retrieved, Pokemon, From, pokemonId));

            return myPokemon;
        }

        public List<tlkpNationalDex> GetNationalDex()
        {
            List<tlkpNationalDex> nationalDex = _context.tlkpNationalDex.OrderBy(p => p.Id).ToList();

            _logger.LogInformation(string.Format(InformationalMessageWithCount, nationalDex.Count, Pokemon));

            return nationalDex;
        }

        public tlkpNationalDex GetNationalDexPokemonById(int pokemonId)
        {
            tlkpNationalDex nationalDexPokemon = _context.tlkpNationalDex.FirstOrDefault(p => p.Id == pokemonId);

            _logger.LogInformation(string.Format(InformationalMessageWithId, Retrieved, Pokemon, From, pokemonId));

            return nationalDexPokemon;
        }

        public tlkpPokeball GetPokeballById(int pokeballId)
        {
            tlkpPokeball pokeball = _context.tlkpPokeball.FirstOrDefault(p => p.Id == pokeballId);

            _logger.LogInformation(string.Format(InformationalMessageWithId, Retrieved, "Pokeball", From, pokeballId));

            return pokeball;
        }

        public tlkpType GetTypeById(int typeId)
        {
            tlkpType type = _context.tlkpType.FirstOrDefault(t => t.Id == typeId);

            _logger.LogInformation(string.Format(InformationalMessageWithId, Retrieved, "Type", From, typeId));

            return type;
        }

        public List<tlkpNationalDex> Search(string searchString, int? selectedAbilityId, int? selectedCategoryId, int? selectedTypeId)
        {
            List<tlkpNationalDex> nationalDexSearchResults = GetNationalDex()
                .Where(p => p.Name.Contains(searchString, StringComparison.CurrentCultureIgnoreCase)
                || (p.JapaneseName != null && p.JapaneseName.Contains(searchString, StringComparison.CurrentCultureIgnoreCase))
                || (p.Description != null && p.Description.Contains(searchString, StringComparison.CurrentCultureIgnoreCase)))
                .ToList();

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
            List<tblMyPokedex> myPokedexSearchResults = GetMyPokedex()
                .Where(p => p.Pokemon.Name.Contains(searchString, StringComparison.CurrentCultureIgnoreCase)
                || (p.Pokemon.JapaneseName != null && p.Pokemon.JapaneseName.Contains(searchString, StringComparison.CurrentCultureIgnoreCase))
                || (p.Pokemon.Description != null && p.Pokemon.Description.Contains(searchString, StringComparison.CurrentCultureIgnoreCase))
                || (p.Nickname != null && p.Nickname.Contains(searchString, StringComparison.CurrentCultureIgnoreCase)))
                .ToList();

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

            _logger.LogInformation(string.Format(InformationalMessageWithSearchCriteria, myPokedexSearchResults.ToList().Count, searchString));

            return myPokedexSearchResults.OrderBy(p => p.PokemonId).ToList();
        }
    }
}