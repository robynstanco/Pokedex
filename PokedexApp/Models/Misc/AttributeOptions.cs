using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;

namespace PokedexApp.Models.Misc
{
    public class AttributeOptions
    {
        public IEnumerable<SelectListItem> AbilityOptions { get; set; }
        public int? SelectedAbilityId { get; set; }
        public int? SelectedHiddenAbilityId { get; set; }

        public IEnumerable<SelectListItem> CategoryOptions { get; set; }
        public int? SelectedCategoryId { get; set; }

        public IEnumerable<SelectListItem> PokeballOptions { get; set; }
        public int? SelectedPokeballId { get; set; }

        public IEnumerable<SelectListItem> TypeOptions { get; set; }
        public int? SelectedTypeOneId { get; set; }
        public int? SelectedTypeTwoId { get; set; }
    }
}