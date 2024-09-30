using ImageAPI.Validators;

namespace ImageAPI.DTOs.Requests
{
    public class VersionActionRequestDTO
    {
        /// <summary>
        /// The ID of the SVG list associated with the version action.
        /// </summary>
        [VersionActionRequestValidator]
        public int ListId { get; set; }

        /// <summary>
        /// The action to perform on the specified SVG version (e.g., "newest").
        /// </summary>
        [VersionActionRequestValidator]
        public string Action { get; set; }
    }
}
