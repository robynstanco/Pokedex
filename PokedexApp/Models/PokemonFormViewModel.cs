using Microsoft.AspNetCore.Mvc.Rendering;
using Pokedex.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace PokedexApp.Models
{
    public class PokemonFormViewModel
    {
        [Required]
        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = Constants.DateFormat)]
        public DateTime? Date { get; set; }

        public string DatePlaceholder
        {
            get
            {
                return "MM/DD/YYYY";
            }
        }

        [Required]
        public int Level { get; set; }

        public string Location { get; set; }

        public IEnumerable<SelectListItem> NationalDexOptions { get; set; }
        [Required]
        public int SelectedNationalDexPokemonId { get; set; }

        public string Nickname { get; set; }

        public IEnumerable<SelectListItem> PokeballOptions { get; set; }
        [Required]
        public int SelectedPokeballId { get; set; }

        public IEnumerable<SelectListItem> SexOptions { get; set; }
        [Required]
        public int SelectedSexId { get; set; }
    }
}