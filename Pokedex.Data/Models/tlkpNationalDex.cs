using System.Collections.Generic;

namespace Pokedex.Data.Models
{
    public partial class tlkpNationalDex
    {
        public tlkpNationalDex()
        {
            tblMyPokedex = new HashSet<tblMyPokedex>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public string JapaneseName { get; set; }
        public string Description { get; set; }
        public int? CategoryId { get; set; }
        public int? TypeOneId { get; set; }
        public int? TypeTwoId { get; set; }
        public int? AbilityId { get; set; }
        public int? HiddenAbilityId { get; set; }
        public int HeightInInches { get; set; }
        public int WeightInPounds { get; set; }
        public string ImageURL { get; set; }

        public virtual tlkpAbility Ability { get; set; }
        public virtual tlkpCategory Category { get; set; }
        public virtual tlkpAbility HiddenAbility { get; set; }
        public virtual tlkpType TypeOne { get; set; }
        public virtual tlkpType TypeTwo { get; set; }
        public virtual ICollection<tblMyPokedex> tblMyPokedex { get; set; }
    }
}