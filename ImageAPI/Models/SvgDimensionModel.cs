using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace SvgApi.Models
{
    public class SvgDimensionModel
    {
        // Use PascalCase for public properties
        [JsonPropertyName("ListId")]
        public int ListId { get; set; }  // Public property for serialization

        [JsonPropertyName("VersionId")]
        public double VersionId { get; set; } // Public property for serialization

        public int Width { get; set; }
        public int Height { get; set; }

        // Versions should remain the same
        public List<SvgVersion> Versions { get; set; } = new List<SvgVersion>();
    }

    public class SvgVersion
    {
        public double Id { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
    }

    public class SvgRoot
    {
        public List<SvgDimensionModel> SvgDimensions { get; set; } = new List<SvgDimensionModel>();
    }
}
