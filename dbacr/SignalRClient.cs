using Dbacr.Extension;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Dbacr;
public class SignalRClient : IAsyncDisposable
{
    private HubConnection? _hubConnection;
    private readonly HubConnectionBuilder _hubConnectionBuilder;

    /// <summary>
    /// state of the connection to the SignalR server
    /// </summary>
    public HubConnectionState State => _hubConnection?.State ?? HubConnectionState.Disconnected;
    /// <summary>
    /// event to be called when the connection to the SignalR server is established
    /// </summary>
    public Action<HubConnection?>? OnConnected;
    /// <summary>
    /// event to be called when the connection to the SignalR server is reconnecting
    /// </summary>
    public Action<Exception?>? OnReconnecting;
    /// <summary>
    /// event to be called when the client has an error 
    /// </summary>
    public Action<Exception>? OnError;
    /// <summary>
    /// event to be called when the connection to the SignalR server is disconnected
    /// </summary>
    public Action<string?>? OnDisconnected;
    /// <summary>
    /// event to be called when the connection to the SignalR server is reconnected
    /// </summary>
    public Action<string?>? OnReconnected;

    /// <summary>
    /// represent a SignalR client 
    /// </summary>
    public SignalRClient()
       => _hubConnectionBuilder = new HubConnectionBuilder();


    /// <summary>
    /// Add Autoreconnect to the signalr client
    /// </summary>
    /// <returns></returns>
    public SignalRClient WithAutoReconnect()
    {
        _hubConnectionBuilder.WithAutomaticReconnect();
        return this;
    }

    /// <summary>
    /// Add Autoreconnect to the signalr client with timespan
    /// </summary>
    public SignalRClient WithAutoReconnect(params TimeSpan[] span)
    {
        _hubConnectionBuilder.WithAutomaticReconnect(span);
        return this;
    }

    /// <summary>
    /// Add json protocol to the signalr client
    /// </summary>
    /// <returns></returns>
    public SignalRClient WithJsonProtocol()
    {
        _hubConnectionBuilder.AddJsonProtocol();
        return this;
    }

    /// <summary>
    /// Add Message Pack protocol to the signalr client
    /// </summary>
    /// <returns></returns>
    public SignalRClient WithMessagePackProtocol()
    {
        _hubConnectionBuilder.AddMessagePackProtocol();
        return this;
    }

    /// <summary>
    /// Add logging to the signalr client
    /// </summary>
    /// <returns></returns>
    public SignalRClient WithLogging(Action<ILoggingBuilder> logger)
    {
        _hubConnectionBuilder.ConfigureLogging(logger);
        return this;
    }

    /// <summary>
    /// Add Url to the signalr client
    /// </summary>
    public SignalRClient WithUrl(string url)
    {
        _hubConnectionBuilder.WithUrl(url);
        return this;
    }

    /// <summary>
    /// Add Url to the signalr client
    /// </summary>
    public SignalRClient WithUrl(Uri url)
    {
        _hubConnectionBuilder.WithUrl(url);
        return this;
    }

    /// <summary>
    /// Build the client
    /// </summary>
    public SignalRClient Build()
    {
        _hubConnection = _hubConnectionBuilder.Build();
        _hubConnection.Closed += HubConnection_Closed;
        _hubConnection.Reconnecting += HubConnection_Reconnecting;
        _hubConnection.Reconnected += HubConnection_Reconnected;
        return this;
    }

    /// <summary>
    /// async method to connect to a signalr server
    /// </summary>
    public async Task<HubConnectionState> ConnectAsync()
    {
        if (_hubConnection is { State: HubConnectionState.Connected })
            return _hubConnection.State;

        int retryCount = 0;

    repeat:
        try
        {
            await _hubConnection!.StartAsync();
        }
        catch (Exception ex)
        {
            OnError?.Invoke(ex);

            if (retryCount++ <= 2)
                goto repeat;
        }

        if (_hubConnection!.State != HubConnectionState.Connected && retryCount++ <= 2)
            goto repeat;

        return _hubConnection.State;
    }

    /// <summary>
    /// event handler for when the hub connection is reconnected
    /// </summary>
    private Task HubConnection_Reconnected(string? arg)
    {
        OnReconnected?.Invoke(arg);

        return Task.CompletedTask;
    }

    /// <summary>
    /// event handler for when the hub connection is reconnecting
    /// </summary>
    private Task HubConnection_Reconnecting(Exception? arg)
    {
        if (arg is null)
            OnReconnecting?.Invoke(arg);
        else
            OnError?.Invoke(arg);

        return Task.CompletedTask;
    }

    /// <summary>
    /// event handler for when the hub connection is closed
    /// </summary>
    private Task HubConnection_Closed(Exception? arg)
    {
        if (arg is null)
            OnDisconnected?.Invoke("disconnected from server");
        else
            OnError?.Invoke(arg);

        return Task.CompletedTask;
    }

    /// <summary>
    /// async method to disconnect from a signalr server
    /// </summary>
    public async Task DisconnectAsync()
    {
        if (_hubConnection is null)
            return;

        await _hubConnection.StopAsync();
        await DisposeAsync();

    }
    /// <summary>
    /// async method to invoke a method on the signalr server
    /// </summary>
    public async Task<T?> InvokeAsync<T>(string method , int retry = 5)
    {
        if (_hubConnection is null || _hubConnection.State != HubConnectionState.Connected)
            return default;
        try
        {
            return await _hubConnection.InvokeAsync<T>(method).RetryAsync(retry);
        }
        catch (Exception e)
        {
            OnError?.Invoke(e);
            return default;
        }
    }

    /// <summary>
    /// async method to invoke a method on the signalr server
    /// </summary>
    public async Task<T?> InvokeAsync<T>(string method, object? args, int retry = 5)
    {
        if (_hubConnection is null || _hubConnection.State != HubConnectionState.Connected)
            return default;
        try
        {
            return await _hubConnection.InvokeAsync<T>(method, args).RetryAsync(retry);
        }
        catch (Exception e)
        {
            OnError?.Invoke(e);
            return default;
        }
    }

    /// <summary>
    /// async method to invoke a method on the signalr server
    /// </summary>
    public async Task<T?> InvokeAsync<T>(string method, object? arg, object? arg1, int retry = 5)
    {
        if (_hubConnection is null || _hubConnection.State != HubConnectionState.Connected)
            return default;
        try
        {
            return await _hubConnection.InvokeAsync<T>(method, arg, arg1).RetryAsync(5);
        }
        catch (Exception e)
        {
            OnError?.Invoke(e);
            return default;
        }
    }

    /// <summary>
    /// async method to invoke a method on the signalr server
    /// </summary>
    public async Task<T?> InvokeAsync<T>(string method, object? arg, object? arg1, object? arg2, int retry = 5)
    {
        if (_hubConnection is null || _hubConnection.State != HubConnectionState.Connected)
            return default;
        try
        {
            return await _hubConnection.InvokeAsync<T>(method, arg, arg1, arg2).RetryAsync(5);
        }
        catch (Exception e)
        {
            OnError?.Invoke(e);
            return default;
        }
    }

    /// <summary>
    /// async method to invoke a method on the signalr server
    /// </summary>
    public async Task<T?> InvokeAsync<T>(string method, object? arg, object? arg1, object? arg2, object? arg3, int retry = 5)
    {
        if (_hubConnection is null || _hubConnection.State != HubConnectionState.Connected)
            return default;
        try
        {
            return await _hubConnection.InvokeAsync<T>(method, arg, arg1, arg2, arg3).RetryAsync(retry);
        }
        catch (Exception e)
        {
            OnError?.Invoke(e);
            return default;
        }
    }

    /// <summary>
    /// async method to invoke a method on the signalr server
    /// </summary>
    public async Task<T?> InvokeAsync<T>(string method, object? arg, object? arg1, object? arg2, object? arg3, object? arg4, int retry = 5)
    {
        if (_hubConnection is null || _hubConnection.State != HubConnectionState.Connected)
            return default;
        try
        {
            return await _hubConnection.InvokeAsync<T>(method, arg, arg1, arg2, arg3, arg4).RetryAsync(retry);
        }
        catch (Exception e)
        {
            OnError?.Invoke(e);
            return default;
        }
    }

    /// <summary>
    /// async method to invoke a method on the signalr server
    /// </summary>
    public async Task<T?> InvokeAsync<T>(string method, object? arg, object? arg1, object? arg2, object? arg3, object? arg4, object? arg5, int retry = 5)
    {
        if (_hubConnection is null || _hubConnection.State != HubConnectionState.Connected)
            return default;
        try
        {
            return await _hubConnection.InvokeAsync<T>(method, arg, arg1, arg2, arg3, arg4, arg5).RetryAsync(retry);
        }
        catch (Exception e)
        {
            OnError?.Invoke(e);
            return default;
        }
    }

    /// <summary>
    /// async method to invoke a method on the signalr server
    /// </summary>
    public async Task<T?> InvokeAsync<T>(string method, object? arg, object? arg1, object? arg2, object? arg3, object? arg4, object? arg5, object? arg6, int retry = 5)
    {
        if (_hubConnection is null || _hubConnection.State != HubConnectionState.Connected)
            return default;
        try
        {
            return await _hubConnection.InvokeAsync<T>(method, arg, arg1, arg2, arg3, arg4, arg5, arg6).RetryAsync(retry);
        }
        catch (Exception e)
        {
            OnError?.Invoke(e);
            return default;
        }
    }

    /// <summary>
    /// async method to invoke a method on the signalr server
    /// </summary>
    public async Task<T?> InvokeAsync<T>(string method, object? arg, object? arg1, object? arg2, object? arg3, object? arg4, object? arg5, object? arg6, object? arg7, int retry = 5)
    {
        if (_hubConnection is null || _hubConnection.State != HubConnectionState.Connected)
            return default;
        try
        {
            return await _hubConnection.InvokeAsync<T>(method, arg, arg1, arg2, arg3, arg4, arg5, arg6, arg7).RetryAsync(retry);
        }
        catch (Exception e)
        {
            OnError?.Invoke(e);
            return default;
        }
    }

    /// <summary>
    /// async method to invoke a method on the signalr server
    /// </summary>
    public async Task<T?> InvokeAsync<T>(string method, object? arg, object? arg1, object? arg2, object? arg3, object? arg4, object? arg5, object? arg6, object? arg7, object? arg8, int retry = 5)
    {
        if (_hubConnection is null || _hubConnection.State != HubConnectionState.Connected)
            return default;
        try
        {
            return await _hubConnection.InvokeAsync<T>(method, arg, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8).RetryAsync(retry);
        }
        catch (Exception e)
        {
            OnError?.Invoke(e);
            return default;
        }
    }

    /// <summary>
    /// async method to send a message to the signalr server
    /// </summary>
    public async Task SendAsync(string method, int retry = 5)
    {
        if (string.IsNullOrEmpty(method) || _hubConnection is null || _hubConnection.State != HubConnectionState.Connected)
            return;

        try
        {
            await _hubConnection.SendAsync(method).RetryAsync(retry);
        }
        catch (Exception e)
        {
            OnError?.Invoke(e);
        }
    }

    /// <summary>
    /// async method to send a message to the signalr server
    /// </summary>
    public async Task SendAsync(string method, object? arg, int retry = 5)
    {
        if (string.IsNullOrEmpty(method) || _hubConnection is null || _hubConnection.State != HubConnectionState.Connected)
            return;

        try
        {
            await _hubConnection.SendAsync(method, arg).RetryAsync(retry);
        }
        catch (Exception e)
        {
            OnError?.Invoke(e);
        }
    }

    /// <summary>
    /// async method to send a message to the signalr server
    /// </summary>
    public async Task SendAsync(string method, object? arg, object? arg1, int retry = 5)
    {
        if (string.IsNullOrEmpty(method) || _hubConnection is null || _hubConnection.State != HubConnectionState.Connected)
            return;

        try
        {
            await _hubConnection.SendAsync(method, arg, arg1).RetryAsync(retry);
        }
        catch (Exception e)
        {
            OnError?.Invoke(e);
        }
    }

    /// <summary>
    /// async method to send a message to the signalr server
    /// </summary>
    public async Task SendAsync(string method, object? arg, object? arg1, object? arg2, int retry = 5)
    {
        if (string.IsNullOrEmpty(method) || _hubConnection is null || _hubConnection.State != HubConnectionState.Connected)
            return;

        try
        {
            await _hubConnection.SendAsync(method, arg, arg1, arg2).RetryAsync(retry);
        }
        catch (Exception e)
        {
            OnError?.Invoke(e);
        }
    }

    /// <summary>
    /// async method to send a message to the signalr server
    /// </summary>
    public async Task SendAsync(string method, object? arg, object? arg1, object? arg2, object? arg3, int retry = 5)
    {
        if (string.IsNullOrEmpty(method) || _hubConnection is null || _hubConnection.State != HubConnectionState.Connected)
            return;

        try
        {
            await _hubConnection.SendAsync(method, arg, arg1, arg2, arg3).RetryAsync(retry);
        }
        catch (Exception e)
        {
            OnError?.Invoke(e);
        }
    }

    /// <summary>
    /// async method to send a message to the signalr server
    /// </summary>
    public async Task SendAsync(string method, object? arg, object? arg1, object? arg2, object? arg3, object? arg4, int retry = 5)
    {
        if (string.IsNullOrEmpty(method) || _hubConnection is null || _hubConnection.State != HubConnectionState.Connected)
            return;

        try
        {
            await _hubConnection.SendAsync(method, arg, arg1, arg2, arg3, arg4).RetryAsync(retry);
        }
        catch (Exception e)
        {
            OnError?.Invoke(e);
        }
    }

    /// <summary>
    /// async method to send a message to the signalr server
    /// </summary>
    public async Task SendAsync(string method, object? arg, object? arg1, object? arg2, object? arg3, object? arg4, object? arg5, int retry = 5)
    {
        if (string.IsNullOrEmpty(method) || _hubConnection is null || _hubConnection.State != HubConnectionState.Connected)
            return;

        try
        {
            await _hubConnection.SendAsync(method, arg, arg1, arg2, arg3, arg4, arg5).RetryAsync(retry);
        }
        catch (Exception e)
        {
            OnError?.Invoke(e);
        }
    }

    /// <summary>
    /// async method to send a message to the signalr server
    /// </summary>
    public async Task SendAsync(string method, object? arg, object? arg1, object? arg2, object? arg3, object? arg4, object? arg5, object? arg6, int retry = 5)
    {
        if (string.IsNullOrEmpty(method) || _hubConnection is null || _hubConnection.State != HubConnectionState.Connected)
            return;

        try
        {
            await _hubConnection.SendAsync(method, arg, arg1, arg2, arg3, arg4, arg5, arg6).RetryAsync(retry);
        }
        catch (Exception e)
        {
            OnError?.Invoke(e);
        }
    }

    /// <summary>
    /// async method to send a message to the signalr server
    /// </summary>
    public async Task SendAsync(string method, object? arg, object? arg1, object? arg2, object? arg3, object? arg4, object? arg5, object? arg6, object? arg7, int retry = 5)
    {
        if (string.IsNullOrEmpty(method) || _hubConnection is null || _hubConnection.State != HubConnectionState.Connected)
            return;

        try
        {
            await _hubConnection.SendAsync(method, arg, arg1, arg2, arg3, arg4, arg5, arg6, arg7).RetryAsync(retry);
        }
        catch (Exception e)
        {
            OnError?.Invoke(e);
        }
    }

    /// <summary>
    /// async method to send a message to the signalr server
    /// </summary>
    public async Task SendAsync(string method, object? arg, object? arg1, object? arg2, object? arg3, object? arg4, object? arg5, object? arg6, object? arg7, object? arg8, int retry = 5)
    {
        if (string.IsNullOrEmpty(method) || _hubConnection is null || _hubConnection.State != HubConnectionState.Connected)
            return;

        try
        {
            await _hubConnection.SendAsync(method, arg, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8).RetryAsync(retry);
        }
        catch (Exception e)
        {
            OnError?.Invoke(e);
        }
    }

    /// <summary>
    /// method to register a callback on the signalr server
    /// </summary>
    public void On(string method, Action callback)
    {
        if (string.IsNullOrEmpty(method) || _hubConnection is null)
            return;

        _hubConnection.On(method, callback);
    }

    /// <summary>
    /// method to register a callback on the signalr server
    /// </summary>
    public void On<T>(string method, Action<T?> callback)
    {
        if (string.IsNullOrEmpty(method) || _hubConnection is null)
            return;

        _hubConnection.On(method, callback);
    }

    /// <summary>
    /// method to register a callback on the signalr server
    /// </summary>
    public void On<T1, T2>(string method, Action<T1?, T2?> callback)
    {
        if (string.IsNullOrEmpty(method) || _hubConnection is null)
            return;

        _hubConnection.On(method, callback);
    }

    /// <summary>
    /// method to register a callback on the signalr server
    /// </summary>
    public void On<T1, T2, T3>(string method, Action<T1?, T2?, T3?> callback)
    {
        if (string.IsNullOrEmpty(method) || _hubConnection is null)
            return;

        _hubConnection.On(method, callback);
    }

    /// <summary>
    /// method to register a callback on the signalr server
    /// </summary>
    public void On<T1, T2, T3, T4>(string method, Action<T1?, T2?, T3?, T4?> callback)
    {
        if (string.IsNullOrEmpty(method) || _hubConnection is null)
            return;

        _hubConnection.On(method, callback);
    }

    /// <summary>
    /// method to register a callback on the signalr server
    /// </summary>
    public void On<T1, T2, T3, T4, T5>(string method, Action<T1?, T2?, T3?, T4?, T5?> callback)
    {
        if (string.IsNullOrEmpty(method) || _hubConnection is null)
            return;

        _hubConnection.On(method, callback);
    }

    /// <summary>
    /// method to register a callback on the signalr server
    /// </summary>
    public void On<T1, T2, T3, T4, T5, T6>(string method, Action<T1?, T2?, T3?, T4?, T5?, T6?> callback)
    {
        if (string.IsNullOrEmpty(method) || _hubConnection is null)
            return;

        _hubConnection.On(method, callback);
    }

    /// <summary>
    /// method to register a callback on the signalr server
    /// </summary>
    public void On<T1, T2, T3, T4, T5, T6, T7>(string method, Action<T1?, T2?, T3?, T4?, T5?, T6?, T7?> callback)
    {
        if (string.IsNullOrEmpty(method) || _hubConnection is null)
            return;

        _hubConnection.On(method, callback);
    }

    /// <summary>
    /// method to dispose the signalr client
    /// </summary>
    public async ValueTask DisposeAsync()
    {
        if (_hubConnection is null)
            return;

        await _hubConnection.DisposeAsync();
    }
}
