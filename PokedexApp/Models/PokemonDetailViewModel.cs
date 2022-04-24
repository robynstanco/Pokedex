using Pokedex.Common;
using System;
using System.ComponentModel.DataAnnotations;

namespace PokedexApp.Models
{
    /// <summary>
    /// The Pokémon Detail View Model.
    /// </summary>
    public class PokemonDetailViewModel
    {
        /// <summary>
        /// The ability.
        /// </summary>
        public string Ability { get; set; }

        /// <summary>
        /// The category.
        /// </summary>
        public string Category { get; set; }

        /// <summary>
        /// The capture date.
        /// </summary>
        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = Constants.DateFormat)]
        public DateTime? Date { get; set; }

        /// <summary>
        /// The description.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// The heigh in inches.
        /// </summary>
        public int HeightInInches { get; set; }

        /// <summary>
        /// The hidden ability.
        /// </summary>
        public string HiddenAbility { get; set; }

        /// <summary>
        /// The Image Url.
        /// </summary>
        public string ImageUrl { get; set; }

        /// <summary>
        /// True if is in edit mode.
        /// </summary>
        public bool IsEditMode { get; set; }

        /// <summary>
        /// The Japanese Name.
        /// </summary>
        public string JapaneseName { get; set; }

        /// <summary>
        /// The level.
        /// </summary>
        [Range(1, 100)]
        public int? Level { get; set; }

        /// <summary>
        /// The location.
        /// </summary>
        public string Location { get; set; }

        /// <summary>
        /// The Id.
        /// </summary>
        public Guid? MyPokemonId { get; set; }

        /// <summary>
        /// The Name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The National Dex Id.
        /// </summary>
        public int? NationalDexPokemonId { get; set; }

        /// <summary>
        /// The Nickname.
        /// </summary>
        public string Nickname { get; set; }

        /// <summary>
        /// The Pokéball Image Url.
        /// </summary>
        public string PokeballImageUrl { get; set; }

        /// <summary>
        /// The sex.
        /// </summary>
        public bool? Sex { get; set; }

        /// <summary>
        /// The first type.
        /// </summary>
        public string TypeOne { get; set; }

        /// <summary>
        /// The second type.
        /// </summary>
        public string TypeTwo { get; set; }

        /// <summary>
        /// The weight in pounds
        /// </summary>
        public int WeightInPounds { get; set; }
    }
}