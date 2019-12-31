using Pokedex.Data.Models;
using System.Collections.Generic;

namespace Pokedex.Tests
{
    public class DataGenerator
    {
        public static IEnumerable<tblMyPokedex> GeneratePokemon(int num)
        {
            List<tblMyPokedex> pokemon = new List<tblMyPokedex>();

            for (int i = 0; i < num; i++)
            {
                pokemon.Add(new tblMyPokedex()
                {
                    Nickname = "N" + i
                });
            }

            return pokemon;
        }
    }
}