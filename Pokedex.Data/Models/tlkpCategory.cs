using System;
using System.Collections.Generic;

namespace Pokedex.Data.Models
{
    public partial class tlkpCategory
    {
        public tlkpCategory()
        {
            tlkpNationalDex = new HashSet<tlkpNationalDex>();
        }

        public int Id { get; set; }
        public string Name { get; set; }

        public ICollection<tlkpNationalDex> tlkpNationalDex { get; set; }
    }
}
