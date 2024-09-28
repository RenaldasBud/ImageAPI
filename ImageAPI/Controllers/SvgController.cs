
using Microsoft.AspNetCore.Mvc;

using Newtonsoft.Json;

using System.Collections.Generic;
using System.Linq;

namespace ImageAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SvgController : ControllerBase
    {
        private const string JsonFilePath = "DB/svgDimensions.json"; // Adjust the path as needed

        [HttpGet]
        public ActionResult<List<SvgData>> GetAllSvgs()
        {
            var svgData = GetSvgDataFromFile();
            return Ok(svgData);
        }

        [HttpPost("store-dimensions")]
        public ActionResult<SvgData> StoreDimensions([FromBody] StoreDimensionsRequest request)
        {
            var svgData = GetSvgDataFromFile();
            var svg = svgData.FirstOrDefault(x => x.ListId == request.ListId);
            if (svg == null)
                return NotFound();

            // Find the specified version to update
            var versionToUpdate = svg.Versions.FirstOrDefault(v => v.Id == request.VersionId);
            if (versionToUpdate == null)
                return NotFound();

            // Update the dimensions
            versionToUpdate.Width = request.Width;
            versionToUpdate.Height = request.Height;

            // Move the updated version to the front of the array
            svg.Versions.Remove(versionToUpdate);
            svg.Versions.Insert(0, versionToUpdate);

            // Save the updated svg data back to the file
            SaveSvgDataToFile(svgData);

            return Ok(svg);
        }

        // Request model for storing dimensions
        public class StoreDimensionsRequest
        {
            public int ListId { get; set; }
            public double VersionId { get; set; } // Using double to match the Version Id type
            public int Width { get; set; }
            public int Height { get; set; }
        }

        [HttpGet("{listId}")]
        public ActionResult<SvgData> GetSvg(int listId)
        {
            var svgData = GetSvgDataFromFile();
            var svg = svgData.FirstOrDefault(x => x.ListId == listId);
            if (svg == null)
                return NotFound();

            return Ok(svg);
        }
        [HttpPost("version")]
        public ActionResult<SvgData> HandleVersion([FromBody] VersionActionRequest request)
        {
            var svgData = GetSvgDataFromFile();
            var svg = svgData.FirstOrDefault(x => x.ListId == request.ListId);
            if (svg == null)
                return NotFound();

            if (request.Action == "toggle")
            {
                // Increment the index and wrap around
                // (This logic should be handled on the frontend)
            }
            else if (request.Action == "newest")
            {
                // Return the first version without modifying any ids or positions
                var firstVersion = svg.Versions.FirstOrDefault(); // Get the first version
                return Ok(new SvgData
                {
                    ListId = svg.ListId,
                    Versions = new List<Version> { firstVersion } // Return only the first version
                });
            }
            else
            {
                return BadRequest("Invalid action");
            }
            return Ok();
            // Save the updated svg data back to the file if necessary (not needed for 'newest' action)
        }

        private List<SvgData> GetSvgDataFromFile()
        {
            var json = System.IO.File.ReadAllText(JsonFilePath);
            var data = JsonConvert.DeserializeObject<SvgRoot>(json);
            return data.SvgDimensions;
        }

        //private void SaveSvgDataToFile(List<SvgData> svgData)
        //{
        //    var json = JsonConvert.SerializeObject(new SvgRoot { SvgDimensions = svgData }, Formatting.Indented);
        //    System.IO.File.WriteAllText(JsonFilePath, json);
        //}
        private void SaveSvgDataToFile(List<SvgData> svgData)
        {
            try
            {
                var json = JsonConvert.SerializeObject(new SvgRoot { SvgDimensions = svgData }, Formatting.Indented);
                System.IO.File.WriteAllText(JsonFilePath, json);
                Console.WriteLine("Good");
            }
            catch (Exception ex)
            {
                // Log the exception or handle it as necessary
                Console.WriteLine($"Error saving SVG data: {ex.Message}");
            }
        }
    }

    public class SvgRoot
    {
        public List<SvgData> SvgDimensions { get; set; }
    }

    public class SvgData
    {
        public int ListId { get; set; }
        public List<Version> Versions { get; set; }
    }

    public class Version
    {
        public double Id { get; set; } // Support decimal ids
        public int Width { get; set; }
        public int Height { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
    }

    public class VersionActionRequest
    {
        public int ListId { get; set; }
        public string Action { get; set; }
    }
}
