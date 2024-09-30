using System.Text.Json.Serialization;

namespace ImageAPI.Models
{
    public class SvgDimensionModel
    {
        [JsonPropertyName("ListId")]
        public int ListId { get; set; }

        [JsonPropertyName("VersionId")]
        public double VersionId { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public List<SvgVersion> Versions { get; set; } = new List<SvgVersion>();
    }

    public class SvgVersion
    {
        public double Id { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
    }

    public class Svg
    {
        public List<SvgDimensionModel> SvgDimensions { get; set; } = new List<SvgDimensionModel>();
    }
}


