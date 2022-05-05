using Microsoft.AspNetCore.Mvc;
using Pokedex.Data.Models;
using PokedexApp.Models;
using System;
using System.Collections.Generic;

namespace Pokedex.Tests
{
    /// <summary>
    /// The Mock Data Generator used for all mock data within test fixtures.
    /// </summary>
    public class DataGenerator
    {
        public static Guid DefaultGuid = Guid.Empty;

        private const string URL = "http://{0}.com";

        /// <summary>
        /// Get the view model from the given action result.
        /// </summary>
        /// <typeparam name="T">The View Model Type.</typeparam>
        /// <param name="result">The action result.</param>
        /// <returns>The View Model.</returns>
        public static T GetViewModel<T>(IActionResult result) where T : class
        {
            return (result as ViewResult)?.ViewData.Model as T;
        }

        public static List<tblMyPokedex> GenerateMyPokemon(int num)
        {
            var pokemon = new List<tblMyPokedex>();

            for (int i = 0; i < num; i++)
            {
                pokemon.Add(new tblMyPokedex()
                {
                    Date = DateTime.Today,
                    Id = DefaultGuid,
                    Level = i + 1,
                    Location = i + " Main Street",
                    Nickname = nameof(tblMyPokedex.Nickname) + i,
                    Pokeball = GeneratePokeballs(i + 1)[i],
                    PokeballId = i,
                    PokemonId = i,
                    Pokemon = GenerateNationalDexPokemon(i + 1)[i],
                    Sex = i % 2 == 0
                });
            }

            return pokemon;
        }

        public static List<tlkpAbility> GenerateAbilities(int num)
        {
            var abilities = new List<tlkpAbility>();

            for (int i = 0; i < num; i++)
            {
                abilities.Add(new tlkpAbility()
                {
                     Id = i,
                     Name = nameof(tlkpAbility.Name) + i
                });
            }

            return abilities;
        }

        public static List<tlkpCategory> GenerateCategories(int num)
        {
            var categories = new List<tlkpCategory>();

            for (int i = 0; i < num; i++)
            {
                categories.Add(new tlkpCategory()
                {
                    Id = i,
                    Name = nameof(tlkpCategory.Name) + i
                });
            }

            return categories;
        }

        public static List<tlkpNationalDex> GenerateNationalDexPokemon(int num)
        {
            var pokemon = new List<tlkpNationalDex>();

            for (int i = 0; i < num; i++)
            {
                pokemon.Add(new tlkpNationalDex()
                {
                    Ability = GenerateAbilities(i + 1)[i],
                    AbilityId = i,
                    Category = GenerateCategories(i + 1)[i],
                    CategoryId = i,
                    Description = "Desc" + i,
                    HeightInInches = i + 1,
                    HiddenAbility = GenerateAbilities(i + 2)[i+1],
                    HiddenAbilityId = i + 1,
                    Id = i,
                    ImageURL = string.Format(URL, i),
                    JapaneseName = "JapaneseName" + i,
                    Name = nameof(tlkpNationalDex.Name) + i,
                    TypeOne = GenerateTypes(i + 1)[i],
                    TypeOneId = i,
                    TypeTwo = GenerateTypes(i + 2)[i + 1],
                    TypeTwoId = i + 1,
                    WeightInPounds = i + 1,
                });
            }

            return pokemon;
        }

        public static List<tlkpPokeball> GeneratePokeballs(int num)
        {
            var pokeballs = new List<tlkpPokeball>();

            for (int i = 0; i < num; i++)
            {
                pokeballs.Add(new tlkpPokeball()
                {
                    Id = i,
                    ImageURL = string.Format(URL, i),
                    Name = nameof(tlkpPokeball.Name) + i
                });
            }

            return pokeballs;
        }

        public static PokemonFormViewModel GeneratePokemonForm()
        {
            return new PokemonFormViewModel();
        }

        public static List<tlkpType> GenerateTypes(int num)
        {
            var types = new List<tlkpType>();

            for (int i = 0; i < num; i++)
            {
                types.Add(new tlkpType()
                {
                    Id = i,
                    Name = nameof(tlkpType.Name) + i
                });
            }

            return types;
        }
    }
}