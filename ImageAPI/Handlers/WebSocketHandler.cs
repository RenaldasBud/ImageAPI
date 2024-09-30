using System.Net.WebSockets;
using System.Text;

namespace ImageAPI.Handlers
{
    public class WebSocketHandler
    {
        private readonly MessageProcessor _messageProcessor;

        public WebSocketHandler(MessageProcessor messageProcessor)
        {
            _messageProcessor = messageProcessor;
        }

        public async Task HandleWebSocketAsync(WebSocket webSocket, CancellationToken cancellationToken)
        {
            try
            {
                while (webSocket.State == WebSocketState.Open)
                {
                    var receivedMessage = await ReceiveMessageAsync(webSocket, cancellationToken);
                    if (receivedMessage != null)
                    {
                        await _messageProcessor.ProcessMessageAsync(receivedMessage, webSocket);
                    }
                }
            }
            catch (WebSocketException ex)
            {
                Console.WriteLine($"WebSocket error: {ex.Message}");
            }
            finally
            {
                await CloseSocketAsync(webSocket);
            }
        }

        private async Task<string?> ReceiveMessageAsync(WebSocket webSocket, CancellationToken cancellationToken)
        {
            var buffer = new byte[1024 * 4];
            var result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), cancellationToken);
            return result.MessageType == WebSocketMessageType.Text
                ? Encoding.UTF8.GetString(buffer, 0, result.Count)
                : null;
        }

        private async Task CloseSocketAsync(WebSocket webSocket)
        {
            if (webSocket.State == WebSocketState.Open)
            {
                await webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closing", CancellationToken.None);
            }
        }
    }
}
