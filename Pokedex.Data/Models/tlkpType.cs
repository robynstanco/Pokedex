using System;
using System.Collections.Generic;

namespace Pokedex.Data.Models
{
    public partial class tlkpType
    {
        public tlkpType()
        {
            tlkpNationalDexTypeOne = new HashSet<tlkpNationalDex>();
            tlkpNationalDexTypeTwo = new HashSet<tlkpNationalDex>();
        }

        public int Id { get; set; }
        public string Name { get; set; }

        public ICollection<tlkpNationalDex> tlkpNationalDexTypeOne { get; set; }
        public ICollection<tlkpNationalDex> tlkpNationalDexTypeTwo { get; set; }
    }
}
