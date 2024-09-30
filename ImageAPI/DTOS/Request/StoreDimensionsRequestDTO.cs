using ImageAPI.Validators;

namespace ImageAPI.DTOs.Requests
{
    public class StoreDimensionsRequestDTO
    {
        /// <summary>
        /// The ID of the SVG list to which the dimensions belong.
        /// </summary>
        [StoreDimensionsRequestValidator]
        public int ListId { get; set; }

        /// <summary>
        /// The ID of the specific version of the SVG to update.
        /// </summary>
        [StoreDimensionsRequestValidator]
        public double VersionId { get; set; }

        /// <summary>
        /// The width to set for the specified SVG version.
        /// </summary>
        [StoreDimensionsRequestValidator]
        public int Width { get; set; }

        /// <summary>
        /// The height to set for the specified SVG version.
        /// </summary>
        [StoreDimensionsRequestValidator]
        public int Height { get; set; }
    }
}
