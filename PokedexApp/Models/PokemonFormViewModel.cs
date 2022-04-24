using Microsoft.AspNetCore.Mvc.Rendering;
using Pokedex.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace PokedexApp.Models
{
    /// <summary>
    /// The Pokémon Form View Model.
    /// </summary>
    public class PokemonFormViewModel
    {
        /// <summary>
        /// The reguired date of capture.
        /// </summary>
        [Required]
        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = Constants.DateFormat)]
        public DateTime? Date { get; set; }

        /// <summary>
        /// The date placeholder.
        /// </summary>
        public string DatePlaceholder
        {
            get
            {
                return "MM/DD/YYYY";
            }
        }

        /// <summary>
        /// The required level.
        /// </summary>
        [Required]
        [Range(1,100)]
        public int Level { get; set; }
        
        /// <summary>
        /// The optional location.
        /// </summary>
        public string Location { get; set; }

        /// <summary>
        /// The National Dex options.
        /// </summary>
        public IEnumerable<SelectListItem> NationalDexOptions { get; set; }

        /// <summary>
        /// The required selected National Dex Pokémon Id.
        /// </summary>
        [Required]
        public int SelectedNationalDexPokemonId { get; set; }

        /// <summary>
        /// The optional Nickname.
        /// </summary>
        public string Nickname { get; set; }

        /// <summary>
        /// The Pokéball options.
        /// </summary>
        public IEnumerable<SelectListItem> PokeballOptions { get; set; }

        /// <summary>
        /// The required selected Pokéball Id.
        /// </summary>
        [Required]
        public int SelectedPokeballId { get; set; }

        /// <summary>
        /// The sex options.
        /// </summary>
        public IEnumerable<SelectListItem> SexOptions { get; set; }

        /// <summary>
        /// The required selected sex Id.
        /// </summary>
        [Required]
        public int SelectedSexId { get; set; }
    }
}