using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Http;
using UnityEditor.PackageManager;
using UnityEngine;
using UnityEngine.Assertions;

public class HTTPClient : MonoBehaviour
{
    // Instantiate one HttpClient for your application's lifetime
    private static readonly HttpClient client = new();
    /*private readonly string server_url = "http://146.169.188.207:5000/";*/
    private readonly string server_url = "http://127.0.0.1:5000/";

    private Dictionary<string, string> msg_values = new() { };

    public async void ClientConnect(string msg)
    {
        //Debug.Log("string:"+server_url);
        Uri siteUri = new Uri(server_url);
        //Debug.Log(siteUri);

        msg_values.Clear();
        msg_values.Add("client_MSG_"+ msg_values.Count, msg);

        // Create content for POST
        var content = new FormUrlEncodedContent(msg_values);
        Debug.Log(content.ToString());


        // Send the POST request
        var response = await client.PostAsync(siteUri, content);
        Assert.IsTrue(response.IsSuccessStatusCode);
        Debug.Log("RESPONSE: " + response.Content);

        // Read the response
        // var responseString = await response.Content.ReadAsStringAsync();

    }

}
