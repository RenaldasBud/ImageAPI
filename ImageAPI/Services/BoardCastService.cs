using System.Net.WebSockets;
using System.Text;
using System.Collections.Concurrent;

namespace ImageAPI.Handlers
{
    public class BroadcastService
    {
        private static readonly ConcurrentDictionary<string, WebSocket> _webSocketClients = new ConcurrentDictionary<string, WebSocket>();

        public void RegisterClient(string clientId, WebSocket webSocket)
        {
            _webSocketClients[clientId] = webSocket;
        }

        public void UnregisterClient(string clientId)
        {
            _webSocketClients.TryRemove(clientId, out _);
        }

        public async Task BroadcastMessageAsync(string message)
        {
            var buffer = Encoding.UTF8.GetBytes(message);
            var tasks = _webSocketClients.Values
                .Where(socket => socket.State == WebSocketState.Open)
                .Select(socket => socket.SendAsync(new ArraySegment<byte>(buffer), WebSocketMessageType.Text, true, CancellationToken.None));

            await Task.WhenAll(tasks);
        }
    }
}
