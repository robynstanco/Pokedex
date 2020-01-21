using Pokedex.Common;
using System;
using System.ComponentModel.DataAnnotations;

namespace PokedexApp.Models
{
    public class PokemonDetailViewModel
    {
        public string Ability { get; set; }

        public string Category { get; set; }

        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = Constants.DateFormat)]
        public DateTime? Date { get; set; }

        public string Description { get; set; }

        public int HeightInInches { get; set; }

        public string HiddenAbility { get; set; }

        public string ImageURL { get; set; }

        public string JapaneseName { get; set; }

        public int? Level { get; set; }

        public string Location { get; set; }

        public Guid? MyPokemonId { get; set; }

        public string Name { get; set; }

        public int? NationalDexPokemonId { get; set; }

        public string Nickname { get; set; }

        public string PokeballImageURL { get; set; }

        public bool? Sex { get; set; }

        public string TypeOne { get; set; }

        public string TypeTwo { get; set; }

        public int WeightInPounds { get; set; }
    }
}