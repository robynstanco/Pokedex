using PokedexApp.Models.Misc;
using System;

namespace PokedexApp.Models
{
    public class PokemonViewModel
    {
        public string Ability { get; set; }
        public AttributeOptions AttributeOptions { get; set; } // used only for Pokemon entry form
        public string Category { get; set; }
        public DateTime? Date { get; set; }
        public string Description { get; set; }
        public int HeightInInches { get; set; }
        public string HiddenAbility { get; set; }
        public int Id { get; set; }
        public string ImageURL { get; set; }
        public string JapaneseName { get; set; }
        public int? Level { get; set; }
        public string Location { get; set; }
        public string Name { get; set; }
        public string Nickname { get; set; }
        public string Pokeball { get; set; }
        public bool? Sex { get; set; }
        public string TypeOne { get; set; }
        public string TypeTwo { get; set; }
        public int WeightInPounds { get; set; }
    }
}