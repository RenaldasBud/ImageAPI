using ImageAPI.DTOs.Requests;
using ImageAPI.Models;

using Microsoft.AspNetCore.Mvc;

using Newtonsoft.Json;

using Version = ImageAPI.Models.Version;

namespace ImageAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SvgController : ControllerBase
    {
        private const string JsonFilePath = "DB/svgDimensions.json";

        /// <summary>
        /// Retrieve all SVG data.
        /// </summary>
        /// <response code="200">List of SVG data</response>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public ActionResult<List<SvgData>> GetAllSvgs()
        {
            var svgData = GetSvgDataFromFile();
            return Ok(svgData);
        }

        /// <summary>
        /// Store dimensions for a specific SVG version.
        /// </summary>
        /// <param name="request">Request containing dimensions and version information</param>
        /// <response code="200">Successfully updated SVG data</response>
        /// <response code="404">SVG or version not found</response>
        [HttpPost("store-dimensions")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<SvgData> StoreDimensions([FromBody] StoreDimensionsRequestDTO request)
        {
            var svgData = GetSvgDataFromFile();
            var svg = svgData.FirstOrDefault(x => x.ListId == request.ListId);
            if (svg == null)
                return NotFound();

            var versionToUpdate = svg.Versions.FirstOrDefault(v => v.Id == request.VersionId);
            if (versionToUpdate == null)
                return NotFound();

            versionToUpdate.Width = request.Width;
            versionToUpdate.Height = request.Height;

            svg.Versions.Remove(versionToUpdate);
            svg.Versions.Insert(0, versionToUpdate);

            SaveSvgDataToFile(svgData);

            return Ok(svg);
        }

        /// <summary>
        /// Retrieve a specific SVG by its list ID.
        /// </summary>
        /// <param name="listId">The ID of the SVG list to retrieve</param>
        /// <response code="200">SVG data for the specified ID</response>
        /// <response code="404">SVG not found</response>
        [HttpGet("{listId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<SvgData> GetSvg(int listId)
        {
            var svgData = GetSvgDataFromFile();
            var svg = svgData.FirstOrDefault(x => x.ListId == listId);
            if (svg == null)
                return NotFound();

            return Ok(svg);
        }

        /// <summary>
        /// Handle version actions for a specific SVG.
        /// </summary>
        /// <param name="request">Request containing version action information</param>
        /// <response code="200">Successfully retrieved SVG version data</response>
        /// <response code="404">SVG not found</response>
        /// <response code="400">Invalid action provided</response>
        [HttpPost("version")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult<SvgData> HandleVersion([FromBody] VersionActionRequestDTO request)
        {
            var svgData = GetSvgDataFromFile();
            var svg = svgData.FirstOrDefault(x => x.ListId == request.ListId);
            if (svg == null)
                return NotFound();
            if (request.Action == "newest")
            {
                var firstVersion = svg.Versions.FirstOrDefault();
                return Ok(new SvgData
                {
                    ListId = svg.ListId,
                    Versions = new List<Version> { firstVersion }
                });
            }
            else
            {
                return BadRequest("Invalid action");
            }
        }

        private List<SvgData> GetSvgDataFromFile()
        {
            var json = System.IO.File.ReadAllText(JsonFilePath);
            var data = JsonConvert.DeserializeObject<SvgRoot>(json);
            return data.SvgDimensions;
        }

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
                Console.WriteLine($"Error saving SVG data: {ex.Message}");
            }
        }
    }
}
