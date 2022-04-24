namespace PokedexApp.Models
{
    /// <summary>
    /// The Error View Model.
    /// </summary>
    public class ErrorViewModel
    {
        /// <summary>
        /// The error message.
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// The error request Id.
        /// </summary>
        public string RequestId { get; set; }

        /// <summary>
        /// True if the request Id is filled.
        /// </summary>
        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
    }
}