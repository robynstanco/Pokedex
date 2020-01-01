using Pokedex.Data.Models;
using System;
using System.Collections.Generic;

namespace Pokedex.Tests
{
    public class DataGenerator
    {
        private const string URL = "http://{0}.com";

        public static List<tblMyPokedex> GeneratePokemon(int num)
        {
            List<tblMyPokedex> pokemon = new List<tblMyPokedex>();

            for (int i = 0; i < num; i++)
            {
                pokemon.Add(new tblMyPokedex()
                {
                    Date = DateTime.Today,
                    Id = i,
                    Level = i + 1,
                    Location = i + " Main Street",
                    Nickname = nameof(tblMyPokedex.Nickname) + i,
                    PokeballId = i,
                    PokemonId = i,
                    Sex = i % 2 == 0
                });
            }

            return pokemon;
        }

        public static List<tlkpAbility> GenerateAbilities(int num)
        {
            List<tlkpAbility> abilities = new List<tlkpAbility>();

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
            List<tlkpCategory> categories = new List<tlkpCategory>();

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

        public static List<tlkpPokeball> GeneratePokeballs(int num)
        {
            List<tlkpPokeball> pokeballs = new List<tlkpPokeball>();

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
    }
}