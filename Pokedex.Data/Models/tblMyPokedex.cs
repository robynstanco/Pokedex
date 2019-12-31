using System;
using System.Collections.Generic;

namespace Pokedex.Data.Models
{
    public partial class tblMyPokedex
    {
        public int Id { get; set; }
        public int PokemonId { get; set; }
        public string Nickname { get; set; }
        public int? Level { get; set; }
        public bool? Sex { get; set; }
        public DateTime? Date { get; set; }
        public string Location { get; set; }
        public int? PokeballId { get; set; }

        public tlkpPokeball Pokeball { get; set; }
        public tlkpNationalDex Pokemon { get; set; }
    }
}
