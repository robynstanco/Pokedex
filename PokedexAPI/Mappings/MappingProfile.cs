using AutoMapper;
using Pokedex.Data.Models;
using PokedexAPI.Models.Output;

namespace PokedexAPI.Mappings
{
    /// <summary>
    /// The AutoMapper Profile.
    /// </summary>
    public class MappingProfile : Profile
    {
        /// <summary>
        /// The Mapping Profile Constructor.
        /// </summary>
        public MappingProfile()
        {
            CreateLookupResultMappings();
        }

        /// <summary>
        /// Creates the mapping for tlkp* => LookupResult.
        /// </summary>
        private void CreateLookupResultMappings()
        {
            CreateMap<tlkpAbility, LookupResult>();
            CreateMap<tlkpCategory, LookupResult>();
            CreateMap<tlkpPokeball, LookupResult>();
            CreateMap<tlkpType, LookupResult>();
        }
    }
}
