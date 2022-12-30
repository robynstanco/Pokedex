using System.CodeDom.Compiler;
using AutoMapper;
using Pokedex.Common;
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
            CreateGenericPokemonResultMappings();
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

        /// <summary>
        /// Creates the mapping for tlkp* => GenericPokemonResult.
        /// </summary>
        private void CreateGenericPokemonResultMappings()
        {
            CreateMap<tlkpNationalDex, GenericPokemonResult>()
                .ForMember(dest => dest.Ability, opt => opt.MapFrom(src => src.Ability.Name))
                .ForMember(dest => dest.Category, opt => opt.MapFrom(src => src.Category.Name))
                .ForMember(dest => dest.HiddenAbility, opt => opt.MapFrom(src => src.HiddenAbilityId.HasValue ? src.HiddenAbility.Name : Constants.NotApplicable))
                .ForMember(dest => dest.NationalDexPokemonId, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.TypeOne, opt => opt.MapFrom(src => src.TypeOne.Name))
                .ForMember(dest => dest.TypeTwo, opt => opt.MapFrom(src => src.TypeTwoId.HasValue ? src.TypeTwo.Name : Constants.NotApplicable));
        }
    }
}
