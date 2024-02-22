using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Http;
using UnityEditor.PackageManager;
using UnityEngine;
using UnityEngine.Assertions;
using Newtonsoft.Json;

public class HTTPClient : MonoBehaviour
{
    // Instantiate one HttpClient for your application's lifetime
    private static readonly HttpClient client = new();
    /*private readonly string server_url = "http://146.169.188.207:5000/";*/
    private readonly string server_url = "http://172.30.36.188:5000/";


    /*private Dictionary<string, string> msg_values = new() { };*/

    public async void ClientConnect(JoystickContent msg_content)
    {
        //Debug.Log("string:"+server_url);
        Uri siteUri = new Uri(server_url);
        //Debug.Log(siteUri);

        //msg_values.Clear();
        //msg_values.Add("content", msg);

        // Create content for POST
        //var content = new FormUrlEncodedContent(msg_values);
        //Debug.Log(content.ToString());


        /*var payload = new Content
        {
            mode_trigger = (submessages[0] != null) ? submessages[0] : "",
            left_joystick = submessages[1],
            right_joystick = submessages[2],
        };*/


        Dictionary<string, object> payload = new Dictionary<string, object>
        {
            { "content", new Dictionary<string, object>
                {
                    { "Mode", msg_content.mode_trigger },
                    { "leftJoystick", new Dictionary<string, float>
                        {
                            { "x", msg_content.left_joystick_x },
                            { "y", msg_content.left_joystick_y },
                        }
                    },
                    { "rightJoystick", new Dictionary<string, float>
                        {
                            { "x", msg_content.right_joystick_x },
                            { "y", msg_content.right_joystick_y },
                        }
                    },
                } 
            },
        };

        // Convert the dictionary to a JSON string
        string json = JsonConvert.SerializeObject(payload, Formatting.Indented);

        // Output the JSON string
        Debug.Log("json:" + json);

        /*var stringPayload = JsonConvert.SerializeObject(payload);*/
        /*var httpContent = new StringContent(stringPayload);*/
        var httpContent = new StringContent(json);
        Debug.Log(httpContent.ToString());

        // Send the POST request
        var response = await client.PostAsync(siteUri, httpContent);
        Assert.IsTrue(response.IsSuccessStatusCode);
        Debug.Log("RESPONSE: " + response.Content);

        // Read the response
        // var responseString = await response.Content.ReadAsStringAsync();

    }

}

public class JoystickContent
{
    public int mode_trigger { get; set; }
    public float left_joystick_x { get; set; } 
    public float left_joystick_y { get; set; } 
    public float right_joystick_x { get; set; }
    public float right_joystick_y { get; set; }
}
