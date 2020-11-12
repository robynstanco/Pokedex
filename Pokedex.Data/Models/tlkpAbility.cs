using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

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

        [JsonIgnore]
        public virtual ICollection<tlkpNationalDex> tlkpNationalDexAbility { get; set; }

        [JsonIgnore]
        public virtual ICollection<tlkpNationalDex> tlkpNationalDexHiddenAbility { get; set; }
    }
}
