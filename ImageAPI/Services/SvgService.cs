using System.Text.Json;

using ImageAPI.Models;

namespace ImageAPI.Handlers
{
    public class SvgService
    {
        private const string JsonFilePath = "DB/svgDimensions.json";

        public async Task SaveDimensionsAsync(SvgDimensionModel dimensions)
        {
            var svgRoot = await LoadSvgRootAsync();
            var existingDimension = svgRoot.SvgDimensions.FirstOrDefault(sd => sd.ListId == dimensions.ListId);
            if (existingDimension == null)
            {
                Console.WriteLine($"No existing SvgDimension found for ListId: {dimensions.ListId}. No changes made.");
                return;
            }

            var existingVersion = existingDimension.Versions.FirstOrDefault(v => Math.Abs(v.Id - dimensions.VersionId) < 0.0001);
            if (existingVersion != null)
            {
                existingVersion.Width = dimensions.Width;
                existingVersion.Height = dimensions.Height;
                Console.WriteLine($"Updated dimensions for ListId: {dimensions.ListId}, VersionId: {dimensions.VersionId}");
            }
            else
            {
                Console.WriteLine($"No existing version found for ListId: {dimensions.ListId}, VersionId: {dimensions.VersionId}. No changes made.");
                return;
            }

            await SaveSvgRootAsync(svgRoot);
        }

        private static async Task<Svg> LoadSvgRootAsync()
        {
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), JsonFilePath);
            if (File.Exists(filePath))
            {
                var json = await File.ReadAllTextAsync(filePath);
                return JsonSerializer.Deserialize<Svg>(json) ?? new Svg { SvgDimensions = new List<SvgDimensionModel>() };
            }

            return new Svg { SvgDimensions = new List<SvgDimensionModel>() };
        }

        private static async Task SaveSvgRootAsync(Svg svgRoot)
        {
            var updatedJson = JsonSerializer.Serialize(svgRoot, new JsonSerializerOptions { WriteIndented = true });
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), JsonFilePath);
            await File.WriteAllTextAsync(filePath, updatedJson);
        }
    }
}
