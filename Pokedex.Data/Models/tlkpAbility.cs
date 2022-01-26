using System.Collections.Generic;

namespace Pokedex.Data.Models
{
    public partial class tlkpAbility
    {
        public tlkpAbility()
        {
            tlkpNationalDexAbility = new HashSet<tlkpNationalDex>();
            tlkpNationalDexHiddenAbility = new HashSet<tlkpNationalDex>();
        }

        public int Id { get; set; }
        public string Name { get; set; }

        public virtual ICollection<tlkpNationalDex> tlkpNationalDexAbility { get; set; }
        public virtual ICollection<tlkpNationalDex> tlkpNationalDexHiddenAbility { get; set; }
    }
}