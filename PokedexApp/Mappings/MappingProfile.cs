using AutoMapper;
using Pokedex.Common;
using Pokedex.Data.Models;
using PokedexApp.Models;

namespace PokedexApp.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreatePokemonFormTblMyPokedexMappings();
            CreateTblMyPokedexDetailMappings();
            CreateTblMyPokedexListingMappings();
            CreateTlkpNationalDexDetailMappings();
        }

        /// <summary>
        /// Create mappings between PokemonFormViewModel => tblMyPokedex.
        /// </summary>
        public void CreatePokemonFormTblMyPokedexMappings()
        {
            CreateMap<PokemonFormViewModel, tblMyPokedex>()
                .ForMember(dest => dest.PokeballId, opt => opt.MapFrom(src => src.SelectedPokeballId))
                .ForMember(dest => dest.PokemonId, opt => opt.MapFrom(src => src.SelectedNationalDexPokemonId))
                .ForMember(dest => dest.Sex, opt => opt.MapFrom(src => src.SelectedSexId == 0 ? false : true));
        }

        /// <summary>
        /// Create mappings between tblMyPokedex <=> PokemonDetailViewModel.
        /// </summary>
        private void CreateTblMyPokedexDetailMappings()
        {
            CreateMap<tblMyPokedex, PokemonDetailViewModel>()
                .ForMember(dest => dest.ImageURL, opt => opt.MapFrom(src => src.Pokemon.ImageURL))
                .ForMember(dest => dest.MyPokemonId, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Pokemon.Name))
                .ForMember(dest => dest.NationalDexPokemonId, opt => opt.MapFrom(src => src.Pokemon.Id))
                .ForMember(dest => dest.PokeballImageURL, opt => opt.MapFrom(src => src.Pokeball.ImageURL))
                .ReverseMap();
        }

        /// <summary>
        /// Create mappings between Lists of tblMyPokedex => PokemonListingViewModel.
        /// </summary>
        private void CreateTblMyPokedexListingMappings()
        {
            CreateMap<tblMyPokedex, PokemonListingViewModel>()
                .ForMember(dest => dest.ImageURL, opt => opt.MapFrom(src => src.Pokemon.ImageURL))
                .ForMember(dest => dest.MyPokemonId, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Pokemon.Name))
                .ForMember(dest => dest.NationalDexPokemonId, opt => opt.MapFrom(src => src.Pokemon.Id));
        }

        /// <summary>
        /// Create mappings between tlkpNationalDex => PokemonDetailViewModel.
        /// </summary>
        private void CreateTlkpNationalDexDetailMappings()
        {
            CreateMap<tlkpNationalDex, PokemonDetailViewModel>()
                .ForMember(dest => dest.Ability, opt => opt.MapFrom(src => src.Ability.Name))
                .ForMember(dest => dest.Category, opt => opt.MapFrom(src => src.Category.Name))
                .ForMember(dest => dest.HiddenAbility, opt => opt.MapFrom(src => src.HiddenAbilityId.HasValue ? src.HiddenAbility.Name : Constants.NotApplicable))
                .ForMember(dest => dest.NationalDexPokemonId, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.TypeOne, opt => opt.MapFrom(src => src.TypeOne.Name))
                .ForMember(dest => dest.TypeTwo, opt => opt.MapFrom(src => src.TypeTwoId.HasValue ? src.TypeTwo.Name : Constants.NotApplicable));
        }
    }
}