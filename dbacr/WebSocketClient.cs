using System.Net.WebSockets;
using System.Text;

namespace Dbacr;

public class WebSocketClient : IDisposable
{
    private ClientWebSocket? _client;
   
    public event Action<WebSocketClient>? OnConnected;
    public event Action<string?>? OnDisconnected;
    public event Action<WebSocketClient, string>? OnMessage;
    public event Action<WebSocketClient, string>? OnMessageSent;

    
    public WebSocketState State => _client?.State ?? WebSocketState.None;

    /// <summary>
    /// async method to connect to a websocket server
    /// </summary>      
    public async Task<WebSocketState> ConnectAsync(string url)
    {
        if (_client is { State: WebSocketState.Open })
            return _client.State;

        _client = new ClientWebSocket();

        await _client.ConnectAsync(new Uri(url), CancellationToken.None);

        if (_client.State == WebSocketState.Open)
        {
            OnConnected?.Invoke(this);
            await ReadMessageAsync();
        }
        if (_client.State == WebSocketState.Closed)
            OnDisconnected?.Invoke(_client.CloseStatusDescription);

        return _client.State;
    }

    /// <summary>
    /// async method loop to read messages from the websocket server
    /// </summary>
    private async Task ReadMessageAsync()
        => await Task.Factory.StartNew(async () =>
           {
               while (_client is { State: WebSocketState.Open })
                   await ReadAsync();
           });

    /// <summary>
    /// async method to send a message to the websocket server
    /// </summary> 
    public async Task SendAsync(string data)
    {
        if (string.IsNullOrEmpty(data) || _client is null || _client.State != WebSocketState.Open)
            return;

        var buffer = Encoding.UTF8.GetBytes(data);
        var segment = new ArraySegment<byte>(buffer);
        await _client.SendAsync(segment, WebSocketMessageType.Text, true, CancellationToken.None);

        OnMessageSent?.Invoke(this, data);
    }

    /// <summary>
    /// async method to receive a message from the websocket server
    /// </summary>
    private async Task ReadAsync()
    {
        if (_client is null || _client.State != WebSocketState.Open) return;

        var buffer = new ArraySegment<byte>(new byte[1024]);
        var result = await _client.ReceiveAsync(buffer, CancellationToken.None);

        if (result.MessageType == WebSocketMessageType.Close)
        {
            await CloseAsync("close received from the server");
            return;
        }

        while (!result.EndOfMessage)
            result = await _client.ReceiveAsync(buffer, CancellationToken.None);

        if (result.MessageType == WebSocketMessageType.Text && buffer.Array != null)
        {
            var data = Encoding.UTF8.GetString(buffer.Array, 0, result.Count);

            if (!string.IsNullOrEmpty(data))
                OnMessage?.Invoke(this, data);
        }
    }

    /// <summary>
    /// async method to close the websocket connection
    /// </summary>
    public async Task CloseAsync(string? description = default)
    {
        if (_client is null || _client.State != WebSocketState.Open)
            return;

        await _client.CloseAsync(WebSocketCloseStatus.NormalClosure, description, CancellationToken.None);

        OnDisconnected?.Invoke(description);
        Dispose();
    }


    /// <summary>
    /// dispose the websocket client
    /// </summary>
    public void Dispose()
    {
        GC.SuppressFinalize(this);
        _client?.Dispose();
        _client = null;
    }
}