using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Concurrent;
using System.IO;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Text.Json;
using SvgApi.Models;
using System.Text.Json.Serialization;

namespace SvgApi.Handlers
{
    public class WebSocketHandler
    {
        private static readonly ConcurrentDictionary<string, WebSocket> _webSocketClients = new ConcurrentDictionary<string, WebSocket>();
        private const string JsonFilePath = "DB/svgDimensions.json"; // Adjust the path to match your setup

        public static async Task HandleWebSocketAsync(WebSocket webSocket, CancellationToken cancellationToken)
        {
            var buffer = new byte[1024 * 4];
            WebSocketReceiveResult result = null;

            try
            {
                while (webSocket.State == WebSocketState.Open)
                {
                    result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), cancellationToken);
                    var message = Encoding.UTF8.GetString(buffer, 0, result.Count);
                    await ProcessMessageAsync(message, webSocket);

                    if (result.MessageType == WebSocketMessageType.Text)
                    {
                        await webSocket.SendAsync(new ArraySegment<byte>(buffer, 0, result.Count), result.MessageType, result.EndOfMessage, cancellationToken);
                    }
                }
            }
            catch (WebSocketException ex)
            {
                Console.WriteLine($"WebSocket error: {ex.Message}");
            }
            finally
            {
                if (webSocket.State == WebSocketState.Open)
                {
                    await webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closing", CancellationToken.None);
                }
            }
        }

        private static async Task ProcessMessageAsync(string message, WebSocket webSocket)
        {
            Console.WriteLine($"Received: {message}");
            try
            {
                var svgDimension = JsonSerializer.Deserialize<SvgDimensionModel>(message);
                if (svgDimension != null)
                {
                    await SaveDimensionsAsync(svgDimension);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error processing WebSocket message: {ex.Message}");
            }
        }

        private async Task ReceiveMessagesAsync(WebSocket socket, string socketId)
        {
            var buffer = new byte[1024 * 4];

            while (socket.State == WebSocketState.Open)
            {
                var result = await socket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);

                if (result.MessageType == WebSocketMessageType.Text)
                {
                    var receivedMessage = Encoding.UTF8.GetString(buffer, 0, result.Count);
                    Console.WriteLine($"Received WebSocket message: {receivedMessage}");

                    await ProcessWebSocketMessage(receivedMessage);
                    await BroadcastMessageAsync(receivedMessage);
                }
                else if (result.MessageType == WebSocketMessageType.Close)
                {
                    break;
                }
            }

            await socket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closing", CancellationToken.None);
        }

        private async Task ProcessWebSocketMessage(string message)
        {
            try
            {
                var svgDimension = JsonSerializer.Deserialize<SvgDimensionModel>(message);
                if (svgDimension != null)
                {
                    await SaveDimensionsAsync(svgDimension);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error processing WebSocket message: {ex.Message}");
            }
        }

        private async Task BroadcastMessageAsync(string message)
        {
            var buffer = Encoding.UTF8.GetBytes(message);

            foreach (var socket in _webSocketClients.Values)
            {
                if (socket.State == WebSocketState.Open)
                {
                    await socket.SendAsync(new ArraySegment<byte>(buffer, 0, buffer.Length), WebSocketMessageType.Text, true, CancellationToken.None);
                }
            }
        }

        //private static async Task SaveDimensionsAsync(SvgDimensionModel dimensions)
        //{
        //    var filePath = Path.Combine(Directory.GetCurrentDirectory(), "DB", "svgDimensions.json");

        //    // Read existing JSON data
        //    SvgRoot svgRoot;
        //    if (File.Exists(filePath))
        //    {
        //        var json = await File.ReadAllTextAsync(filePath);
        //        svgRoot = JsonSerializer.Deserialize<SvgRoot>(json);
        //    }
        //    else
        //    {
        //        svgRoot = new SvgRoot { SvgDimensions = new List<SvgDimensionModel>() };
        //    }

        //    // Log the received dimensions for debugging
        //    Console.WriteLine($"Received dimensions: ListId={dimensions.ListId}, VersionId={dimensions.VersionId}, Width={dimensions.Width}, Height={dimensions.Height}");

        //    // Find the corresponding SvgDimension for the given ListId
        //    var svgDimension = svgRoot.SvgDimensions.FirstOrDefault(sd => sd.ListId == dimensions.ListId);

        //    // Check if svgDimension is found
        //    if (svgDimension == null)
        //    {
        //        Console.WriteLine($"No existing SvgDimension found for ListId: {dimensions.ListId}. No changes made.");
        //        return;
        //    }

        //    Console.WriteLine($"Found existing SvgDimension for ListId: {dimensions.ListId}");

        //    // Log existing versions
        //    Console.WriteLine("Existing Versions:");
        //    foreach (var version in svgDimension.Versions)
        //    {
        //        Console.WriteLine($"VersionId: {version.Id}");
        //    }

        //    // Find the existing version for the given VersionId
        //    var existingVersion = svgDimension.Versions.FirstOrDefault(v => Math.Abs(v.Id - dimensions.VersionId) < 0.0001);

        //    // Log the VersionId you are checking against
        //    Console.WriteLine($"Checking for VersionId: {dimensions.VersionId} in existing versions.");

        //    if (existingVersion != null)
        //    {
        //        existingVersion.Width = dimensions.Width;
        //        existingVersion.Height = dimensions.Height;

        //        Console.WriteLine($"Updated dimensions for ListId: {dimensions.ListId}, VersionId: {dimensions.VersionId}");
        //    }
        //    else
        //    {
        //        Console.WriteLine($"No existing version found for ListId: {dimensions.ListId}, VersionId: {dimensions.VersionId}. No changes made.");
        //        return;
        //    }

        //    // Save the updated data back to the JSON file
        //    var updatedJson = JsonSerializer.Serialize(svgRoot, new JsonSerializerOptions { WriteIndented = true });
        //    await File.WriteAllTextAsync(filePath, updatedJson);

        //    Console.WriteLine("Dimensions saved successfully.");
        //}
        private static async Task SaveDimensionsAsync(SvgDimensionModel dimensions)
        {
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "DB", "svgDimensions.json");

            // Read existing JSON data
            SvgRoot svgRoot;
            if (File.Exists(filePath))
            {
                var json = await File.ReadAllTextAsync(filePath);
                svgRoot = JsonSerializer.Deserialize<SvgRoot>(json);
            }
            else
            {
                svgRoot = new SvgRoot { SvgDimensions = new List<SvgDimensionModel>() };
            }

            // Find the corresponding SvgDimension for the given ListId
            var svgDimension = svgRoot.SvgDimensions.FirstOrDefault(sd => sd.ListId == dimensions.ListId);

            // Check if svgDimension is found
            if (svgDimension == null)
            {
                Console.WriteLine($"No existing SvgDimension found for ListId: {dimensions.ListId}. No changes made.");
                return;
            }

            // Find the existing version for the given VersionId
            var existingVersion = svgDimension.Versions.FirstOrDefault(v => Math.Abs(v.Id - dimensions.VersionId) < 0.0001);

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

            // Remove unwanted properties from the main SvgDimensionModel
            foreach (var item in svgRoot.SvgDimensions)
            {
                item.Width = 0;
                item.Height = 0;
            }

            // Save the updated data back to the JSON file
            var updatedJson = JsonSerializer.Serialize(svgRoot, new JsonSerializerOptions { WriteIndented = true, DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault });
            await File.WriteAllTextAsync(filePath, updatedJson);

            Console.WriteLine("Dimensions saved successfully.");
        }

    }
}

