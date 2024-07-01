using UnityEngine;
using System.Collections;
using WebSocketSharp;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Net;

public class WebSocketManager : MonoBehaviour
{
    public string url = "http://192.168.0.88:3000/v1/devices"; // Replace with your server URL.
    private WebSocket ws;
    private bool isWebSocketReady = false;

    void Start()
    {
        // Optionally include this line if you're using a wss:// connection.
        // It adds a certificate validator for SSL connections (not safe for production).
        ServicePointManager.ServerCertificateValidationCallback = Validator;

        // Start the WebSocket connection
        StartCoroutine(StartWebSocketConnection());
    }
  //  http://192.168.0.88:3000/v1/devices
    IEnumerator StartWebSocketConnection()
    {
        ws = new WebSocket(url);

        ws.OnOpen += (sender, e) =>
        {
            Debug.Log("WebSocket connection opened.");
            isWebSocketReady = true;
        };

        ws.OnMessage += (sender, e) =>
        {
            Debug.Log("Message received from the server: " + e.Data);
        };

        ws.OnError += (sender, e) =>
        {
            Debug.LogError("WebSocket encountered an error: " + e.Message);
            if (e.Exception != null)
            {
                Debug.LogError("WebSocket exception details: " + e.Exception.ToString());
            }
        };

        ws.OnClose += (sender, e) =>
        {
            Debug.Log("WebSocket connection closed: " + e.Reason);
            isWebSocketReady = false;
        };

        // Connect asynchronously to avoid blocking the main thread
        ws.ConnectAsync();

        float timer = 0f;
        float timeout = 10f; // Set a timeout for the connection attempt (10 seconds)

        // Wait for the connection to either open or timeout
        while (!isWebSocketReady && timer < timeout)
        {
            timer += Time.deltaTime;
            yield return null;
        }

        if (!isWebSocketReady)
        {
            Debug.LogError("Connection attempt timed out. Please check the WebSocket URL and your network connection.");
        }
    }

    private void Update()
    {
        // Check if the space bar is pressed and the WebSocket connection is open
        if (Input.GetKeyDown(KeyCode.Space) && isWebSocketReady)
        {
            Debug.Log("Sending hello to the server.");
            ws.Send("Hello"); // Send a message "Hello" to the WebSocket server
        }
    }

    private void OnDestroy()
    {
        // Close the WebSocket connection when the Unity object is destroyed
        if (ws != null)
        {
            ws.CloseAsync();
        }
    }

    // Validator for the SSL certificate (not safe for production)
    private bool Validator(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
    {
        // This returns true to allow any certificate (not secure).
        // In production, you should validate the certificate properly.
        return true; // Not recommended for production.
    }
}
