using System.Net.WebSockets;
using System.Text.Json;

using ImageAPI.Models;

namespace ImageAPI.Handlers
{
    public class MessageProcessor
    {
        private readonly BroadcastService _broadcastService;
        private readonly SvgService _svgService;

        public MessageProcessor(BroadcastService broadcastService, SvgService svgService)
        {
            _broadcastService = broadcastService;
            _svgService = svgService;
        }

        public async Task ProcessMessageAsync(string message, WebSocket webSocket)
        {
            Console.WriteLine($"Received: {message}");
            var svgDimension = DeserializeSvgDimension(message);
            if (svgDimension != null)
            {
                await _svgService.SaveDimensionsAsync(svgDimension);
                await _broadcastService.BroadcastMessageAsync(message);
            }
        }

        private static SvgDimensionModel? DeserializeSvgDimension(string message)
        {
            try
            {
                return JsonSerializer.Deserialize<SvgDimensionModel>(message);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error deserializing message: {ex.Message}");
                return null;
            }
        }
    }
}
