using System;
using System.Collections.Generic;

namespace Pokedex.Data.Models
{
    public partial class tlkpPokeball
    {
        public tlkpPokeball()
        {
            tblMyPokedex = new HashSet<tblMyPokedex>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public string ImageURL { get; set; }

        public ICollection<tblMyPokedex> tblMyPokedex { get; set; }
    }
}
