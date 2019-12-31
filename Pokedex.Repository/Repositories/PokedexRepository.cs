using Microsoft.Extensions.Logging;
using Pokedex.Data.Models;
using Pokedex.Repository.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Pokedex.Repository.Repositories
{
    public class PokedexRepository : IPokedexRepository
    {
        private POKEDEXDBContext _context;
        private ILogger _logger;
        public PokedexRepository(POKEDEXDBContext context, ILogger<PokedexRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        public void AddPokemon(tblMyPokedex pokemon)
        {
            _context.Add(pokemon);
            _context.SaveChanges();
            
            _logger.LogInformation("Added Pokemon to DBContext with Id: " + pokemon.Id);
        }

        public void DeletePokemonById(int pokemonId)
        {
            _context.Remove(GetMyPokemonById(pokemonId));
            _context.SaveChanges();

            _logger.LogInformation("Deleted Pokemon from DBContext with Id: " + pokemonId);
        }

        public void EditPokemon(tblMyPokedex pokemon)
        {
            _context.Update(pokemon);
            _context.SaveChanges();

            _logger.LogInformation("Updated Pokemon in DBContext with Id: " + pokemon.Id);
        }

        public tlkpAbility GetAbilityById(int abilityId)
        {
            tlkpAbility ability = _context.tlkpAbility.FirstOrDefault(a => a.Id == abilityId);

            _logger.LogInformation("Retrieved Ability from DBContext with Id: " + abilityId);

            return ability;
        }

        public IEnumerable<tlkpAbility> GetAllAbilities()
        {
            IEnumerable<tlkpAbility> abilities = _context.tlkpAbility;

            _logger.LogInformation("Retrieved " + abilities.ToList().Count + " Abilities from DBContext.");

            return abilities;
        }

        public IEnumerable<tlkpCategory> GetAllCategories()
        {
            IEnumerable<tlkpCategory> categories = _context.tlkpCategory;

            _logger.LogInformation("Retrieved " + categories.ToList().Count + " Categories from DBContext.");

            return categories;
        }

        public IEnumerable<tlkpPokeball> GetAllPokeballs()
        {
            IEnumerable<tlkpPokeball> pokeballs = _context.tlkpPokeball;

            _logger.LogInformation("Retrieved " + pokeballs.ToList().Count + " Pokeballs from DBContext.");

            return pokeballs;
        }

        public IEnumerable<tlkpType> GetAllTypes()
        {
            IEnumerable<tlkpType> types = _context.tlkpType;

            _logger.LogInformation("Retrieved " + types.ToList().Count + " Types from DBContext.");

            return types;
        }

        public tlkpCategory GetCategoryById(int categoryId)
        {
            tlkpCategory category = _context.tlkpCategory.FirstOrDefault(c => c.Id == categoryId);

            _logger.LogInformation("Retrieved Category from DBContext with Id: " + categoryId);

            return category;
        }

        public IEnumerable<tblMyPokedex> GetMyPokedex()
        {
            IEnumerable<tblMyPokedex> myPokedex = _context.tblMyPokedex;

            _logger.LogInformation("Retrieved " + myPokedex.ToList().Count + " Pokemon from DBContext.");

            return myPokedex;
        }

        public tblMyPokedex GetMyPokemonById(int pokemonId)
        {
            tblMyPokedex myPokemon = _context.tblMyPokedex.FirstOrDefault(p => p.Id == pokemonId);

            _logger.LogInformation("Retrieved Pokemon from DBContext with Id: " + pokemonId);

            return myPokemon;
        }

        public IEnumerable<tlkpNationalDex> GetNationalDex()
        {
            IEnumerable<tlkpNationalDex> nationalDex = _context.tlkpNationalDex;

            _logger.LogInformation("Retrieved " + nationalDex.ToList().Count + " Pokemon from DBContext.");

            return nationalDex;
        }

        public tlkpNationalDex GetNationalDexPokemonById(int pokemonId)
        {
            tlkpNationalDex nationalDexPokemon = _context.tlkpNationalDex.FirstOrDefault(p => p.Id == pokemonId);

            _logger.LogInformation("Retrieved Pokemon from DBContext with Id: " + pokemonId);

            return nationalDexPokemon;
        }

        public tlkpPokeball GetPokeballById(int pokeballId)
        {
            tlkpPokeball pokeball = _context.tlkpPokeball.FirstOrDefault(p => p.Id == pokeballId);

            _logger.LogInformation("Retrieved Pokeball from DBContext with Id: " + pokeballId);

            return pokeball;
        }

        public tlkpType GetTypeById(int typeId)
        {
            tlkpType type = _context.tlkpType.FirstOrDefault(t => t.Id == typeId);

            _logger.LogInformation("Retrieved Type from DBContext with Id: " + typeId);

            return type;
        }

        public IEnumerable<tlkpNationalDex> Search(string searchString, int? selectedAbilityId, int? selectedCategoryId, int? selectedTypeId)
        {
            IEnumerable<tlkpNationalDex> nationalDexSearchResults = GetNationalDex()
                .Where(p => p.Name.Contains(searchString, StringComparison.CurrentCultureIgnoreCase)
                || (p.JapaneseName != null && p.JapaneseName.Contains(searchString, StringComparison.CurrentCultureIgnoreCase))
                || (p.Description != null && p.Description.Contains(searchString, StringComparison.CurrentCultureIgnoreCase)));

            if (selectedAbilityId.HasValue)
            {
                nationalDexSearchResults = nationalDexSearchResults.Where(p => p.AbilityId == selectedAbilityId.Value || p.HiddenAbilityId == selectedAbilityId);
            }

            if (selectedCategoryId.HasValue)
            {
                nationalDexSearchResults = nationalDexSearchResults.Where(p => p.CategoryId == selectedCategoryId.Value);
            }

            if (selectedTypeId.HasValue)
            {
                nationalDexSearchResults = nationalDexSearchResults.Where(p => p.TypeOneId == selectedTypeId.Value || p.TypeTwoId == selectedTypeId.Value);
            }

            _logger.LogInformation("Retrieved " + nationalDexSearchResults.ToList().Count + " Pokemon from DBContext matching Seach criteria.");

            return nationalDexSearchResults;
        }

        public IEnumerable<tblMyPokedex> Search(string searchString, int? selectedAbilityId, int? selectedCategoryId, int? selectedTypeId, int? selectedPokeballId)
        {
            IEnumerable<tblMyPokedex> myPokedexSearchResults = GetMyPokedex()
                .Where(p => p.Pokemon.Name.Contains(searchString, StringComparison.CurrentCultureIgnoreCase)
                || (p.Pokemon.JapaneseName != null && p.Pokemon.JapaneseName.Contains(searchString, StringComparison.CurrentCultureIgnoreCase))
                || (p.Pokemon.Description != null && p.Pokemon.Description.Contains(searchString, StringComparison.CurrentCultureIgnoreCase)));

            if (selectedAbilityId.HasValue)
            {
                myPokedexSearchResults = myPokedexSearchResults.Where(p => p.Pokemon.AbilityId == selectedAbilityId.Value || p.Pokemon.HiddenAbilityId == selectedAbilityId);
            }

            if (selectedCategoryId.HasValue)
            {
                myPokedexSearchResults = myPokedexSearchResults.Where(p => p.Pokemon.CategoryId == selectedCategoryId.Value);
            }

            if (selectedTypeId.HasValue)
            {
                myPokedexSearchResults = myPokedexSearchResults.Where(p => p.Pokemon.TypeOneId == selectedTypeId.Value || p.Pokemon.TypeTwoId == selectedTypeId.Value);
            }

            _logger.LogInformation("Retrieved " + myPokedexSearchResults.ToList().Count + " Pokemon from DBContext matching Seach criteria.");

            return myPokedexSearchResults;
        }
    }
}