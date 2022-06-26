namespace Pokedex.Common
{
    /// <summary>
    /// General Constants used in multiple files/projects. Only abstracted here if occurence of string is > 2.
    /// Note: not applicable for test layers. 
    /// </summary>
    public class Constants
    {
        public const string Abilities = nameof(Abilities);
        public const string Ability = nameof(Ability);
        public const string Added = nameof(Added);
        public const string Bulbapedia = nameof(Bulbapedia);
        public const string Capture = nameof(Capture);
        public const string CaptureDate = "Capture Date";
        public const string Categories = nameof(Categories);
        public const string Category = nameof(Category);
        public const string Controller = nameof(Controller);
        public const string DateFormat = "{0:MM/dd/yyyy}";
        public const string DBContext = nameof(DBContext);
        public const string Deleted = nameof(Deleted);
        public const string Detail = nameof(Detail);
        public const string Edit = nameof(Edit);
        public const string Error = nameof(Error);
        public const string From = "from";
        public const string In = "in";
        public const string InformationalMessageMappingWithCount = Mapping + " {0} {1} {2}.";
        public const string InvalidRequest = "Invalid Request";
        public const string Level = nameof(Level);
        public const string LocationCaught = "Location Caught";
        public const string Mapping = nameof(Mapping);
        public const string NationalDex = nameof(NationalDex);
        public const string Nickname = nameof(Nickname);
        public const string NotApplicable = "N/A";
        public const int    PageSize = 28;
        public const string Pokeball = "Pokéball";
        public const string Pokeballs = "Pokéballs";
        public const string Pokedex = "Pokédex";
        public const string PokedexNoAccent = "Pokedex";
        public const string Pokemon = "Pokémon";
        public const string Retrieved = nameof(Retrieved);
        public const string Search = nameof(Search);
        public const string Sex = nameof(Sex);
        public const string Success = nameof(Success);
        public const string Title = nameof(Title);
        public const string To = "to";
        public const string Type = nameof(Type);
        public const string Types = nameof(Types);
        public const string UnexpectedError = "An unexpected error occured!";
        public const string Updated = nameof(Updated);
        public const string WithId = " with Id: ";
    }
}