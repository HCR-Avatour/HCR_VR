using System;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

public class WebSocketClientClass : MonoBehaviour
{
    private readonly string serverUrl = "ws://127.0.0.1:5000/";
    public ClientWebSocket ws;

    private async void Start()
    {
        ws = new ClientWebSocket();
        await ConnectWebSocket();
    }

    public async Task ConnectWebSocket()
    {
        try
        {
            await ws.ConnectAsync(new Uri(serverUrl), CancellationToken.None);
            Debug.Log("WebSocket connected");

            //ReceiveMessages();
        }
        catch (Exception ex)
        {
            Debug.LogError($"Error connecting to WebSocket server: {ex.Message}");
        }
    }

    private async void ReceiveMessages()
    {
        byte[] buffer = new byte[1024];
        while (ws.State == WebSocketState.Open)
        {
            WebSocketReceiveResult result = await ws.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
            string message = Encoding.UTF8.GetString(buffer, 0, result.Count);
            Debug.Log($"Received message: {message}");
        }
    }

    public async Task SendMessage(string message)
    {
        if (ws != null && ws.State == WebSocketState.Open)
        {
            byte[] buffer = Encoding.UTF8.GetBytes(message);
            await ws.SendAsync(new ArraySegment<byte>(buffer), WebSocketMessageType.Text, true, CancellationToken.None);
        }
        else
        {
            Debug.LogWarning("WebSocket is not connected");
        }
    }

    /*private async void OnApplicationQuit()
    {
        await DisconnectWebSocket();
    }

    private async Task DisconnectWebSocket()
    {
        if (ws != null && ws.State == WebSocketState.Open)
        {
            await ws.CloseAsync(WebSocketCloseStatus.NormalClosure, "", CancellationToken.None);
            Debug.Log("WebSocket closed");
        }
    }*/
}
