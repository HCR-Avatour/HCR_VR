using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using System.Threading.Tasks;
using System.Net.WebSockets;
using UnityEditor.PackageManager;
using Unity.VisualScripting;
using System.Net.Http;
using System.Collections;
using UnityEngine.Networking;
using HuggingFace.API;
using Newtonsoft.Json;

using TMPro;
using UnityEngine.UI;
using System.IO;

public class ControllerInput4 : MonoBehaviour
{
    private InputDevice? leftController, rightController;
    SpeechRecognitionTest SRT = new SpeechRecognitionTest();

    private int modeTrigger = 1;
    private int controlTrigger = 0;
    private int flipflop = 1;
    private int ModeCounter = 0;
    private int countStart;
    public LogJson logExport = new LogJson();
    [SerializeField] private TextMeshProUGUI text;

    private readonly string server_url = "http://avatour.duckdns.org:5000/";
    JoystickDataWrapper wrapper = new JoystickDataWrapper();

    private void Start()
    {
        updateInputDevices();
        InputTracking.nodeAdded += InputTracking_nodeAdded;
        text = GameObject.Find("SpeechText (TMP)").GetComponent<TextMeshProUGUI>();
        SRT = GetComponent<SpeechRecognitionTest>();
        /*Kinda obtuse but who knows, it might work to regulate the frequency of POSTs sent lol*/
        /*This should start instantly and call the postForMe function 20 times per second which shouldn't be many at all.*/
        /*InvokeRepeating("postForMe", 0, 0.05f);*/
    }

    private void postForMe()
    {
        /*StartCoroutine(postRequest("http:///www.yoururl.com", "your json"));*/
        StartCoroutine(postRequest(server_url, JsonUtility.ToJson(wrapper)));
    }

    public IEnumerator postRequest(string url, string json)
    {
        print("JJSON: " + json);
        print("Content:" + wrapper.content.leftJoystick);
        var uwr = new UnityWebRequest(url, "POST");
        byte[] jsonToSend = new System.Text.UTF8Encoding().GetBytes(json);
        uwr.uploadHandler = (UploadHandler)new UploadHandlerRaw(jsonToSend);
        uwr.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
        uwr.SetRequestHeader("Content-Type", "application/json");

        //Send the request then wait here until it returns
        yield return uwr.SendWebRequest();

        if (uwr.isNetworkError)
        {
            Debug.Log("Error While Sending: " + uwr.error);
        }
        else
        {
            Debug.Log("RRReceived1: " + uwr.downloadHandler.text);
        }
    }

    public IEnumerator postFileRequest(string url, byte[] bytes)
    {
        print("Content:" + wrapper.content.leftJoystick);
        var uwr = new UnityWebRequest(url, "POST");
        byte[] jsonToSend = bytes;
        uwr.uploadHandler = (UploadHandler)new UploadHandlerRaw(jsonToSend);
        uwr.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
        uwr.SetRequestHeader("Content-Type", "application/json");

        //Send the request then wait here until it returns
        yield return uwr.SendWebRequest();

        if (uwr.isNetworkError)
        {
            Debug.Log("Error While Sending File: " + uwr.error);
        }
        else
        {
            Debug.Log("RRReceived1 File: " + uwr.downloadHandler.text);
        }
    }

    public IEnumerator postRequestLog(string url, string json)
    {
        print("JJSON: " + json);
        print("Content:" + wrapper.content.leftJoystick);
        var uwr = new UnityWebRequest(url, "POST");
        byte[] jsonToSend = new System.Text.UTF8Encoding().GetBytes(json);
        uwr.uploadHandler = (UploadHandler)new UploadHandlerRaw(jsonToSend);
        uwr.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
        uwr.SetRequestHeader("Content-Type", "application/json");

        //Send the request then wait here until it returns
        yield return uwr.SendWebRequest();

        if (uwr.isNetworkError)
        {
            Debug.Log("Error While Sending: " + uwr.error);
        }
        else
        {
            Debug.Log("RRReceived1: " + uwr.downloadHandler.text);
            logExport = JsonUtility.FromJson<LogJson>(uwr.downloadHandler.text);
            /*logExport.transcript = JsonConvert.SerializeObject(uwr.downloadHandler.text, Formatting.Indented);*/
            text.text = logExport.transcript;
            Debug.Log("killl" + logExport.transcript +"-----" +logExport.audioUrl + "-----" +logExport.loading);

            Debug.Log("GGGlobal Variable loggingSRT: " + text.text);
            Debug.Log("AAAfter receiving JSON");

            Debug.Log("GGGlobal logging: " + logExport.transcript);
        }
    }

    // check for new input devices when new XRNode is added
    private void InputTracking_nodeAdded(XRNodeState obj)
    {
        updateInputDevices();
    }

    // find any devices supporting the desired feature usage
    void updateInputDevices()
    {
        var gameControllers = new List<InputDevice>();
        InputDevices.GetDevicesWithRole(InputDeviceRole.LeftHanded, gameControllers);
        leftController = gameControllers.Count == 0 ? null : gameControllers[0];

        gameControllers.Clear();
        InputDevices.GetDevicesWithRole(InputDeviceRole.RightHanded, gameControllers);
        rightController = gameControllers.Count == 0 ? null : gameControllers[0];
    }

    /*async void Update()*/
    async void FixedUpdate()
    {

        if (countStart == 5)
        {

            /*if (flipflop == 2)
            {*/

            if (leftController?.isValid == false || rightController?.isValid == false) updateInputDevices();

            // Just the left trigger
            if (leftController is not null)
            {
                if (leftController.Value.TryGetFeatureValue(CommonUsages.triggerButton, out bool triggerPressed) && triggerPressed)
                {
                    /*Debug.Log("LEFT: Trigger pressed");*/
                    wrapper.content.rightJoystick.x = 0.0f;
                    wrapper.content.rightJoystick.y = 0.0f;
                    wrapper.content.leftJoystick.x = 0.0f;
                    wrapper.content.leftJoystick.y = 0.0f;
                    modeTrigger = (modeTrigger == 1) ? 0 : 1;
                    /*content.Mode = modeTrigger;*/
                    wrapper.content.Mode = modeTrigger;
                    wrapper.content.Control = controlTrigger;
                    /*client.ClientConnect2(content);*/
                }

                if (leftController.Value.TryGetFeatureValue(CommonUsages.gripButton, out bool gripStartPressed) && gripStartPressed)
                {
                    Debug.Log("GGGgripStartPressed, so let's start recording");
                    SRT.StartRecording();
                }
            }

            // Just the right trigger
            if (rightController is not null)
            {
                if (rightController.Value.TryGetFeatureValue(CommonUsages.triggerButton, out bool triggerPressed) && triggerPressed)
                {
                    /*Debug.Log("LEFT: Trigger pressed");*/
                    wrapper.content.rightJoystick.x = 0.0f;
                    wrapper.content.rightJoystick.y = 0.0f;
                    wrapper.content.leftJoystick.x = 0.0f;
                    wrapper.content.leftJoystick.y = 0.0f;
                    controlTrigger = (controlTrigger == 1) ? 0 : 1;
                    /*content.Mode = modeTrigger;*/
                    wrapper.content.Mode = modeTrigger;
                    wrapper.content.Control = controlTrigger;
                    /*client.ClientConnect2(content);*/
                }
                if (rightController.Value.TryGetFeatureValue(CommonUsages.gripButton, out bool gripStopPressed) && gripStopPressed)
                {
                    Debug.Log("GGGgripStopPressed, so let's start recording");
                    SRT.StopRecording();
                }
            }

            //check if both controllers are in use
            if ((leftController is not null) && (rightController is not null))
            {
                // Get joystick input
                if ((leftController.Value.TryGetFeatureValue(CommonUsages.primary2DAxis, out Vector2 leftJoystickInput)) && (rightController.Value.TryGetFeatureValue(CommonUsages.primary2DAxis, out Vector2 rightJoystickInput)))
                {
                    // Check joystick directions
                    if ((leftJoystickInput != Vector2.zero) && (rightJoystickInput != Vector2.zero))
                    {
                        /*Debug.Log("LEFT: Joystick input: " + leftJoystickInput.x + ","+ leftJoystickInput.y + "\nRIGHT: Joystick input: " + rightJoystickInput.x + "," + rightJoystickInput.y);*/
                        // You can use joystickInput.x and joystickInput.y to determine directions

                        wrapper.content.rightJoystick.x = rightJoystickInput.x;
                        wrapper.content.rightJoystick.y = rightJoystickInput.y;
                        wrapper.content.leftJoystick.x = leftJoystickInput.x;
                        wrapper.content.leftJoystick.y = leftJoystickInput.y;
                        /*content.Mode = modeTrigger;*/
                        wrapper.content.Mode = modeTrigger;
                        wrapper.content.Control = controlTrigger;
                        /*client.ClientConnect2(content);*/
                    }
                    else if (leftJoystickInput != Vector2.zero)
                    {
                        /*JoystickContent content = new JoystickContent();*/
                        wrapper.content.rightJoystick.x = 0.0f;
                        wrapper.content.rightJoystick.y = 0.0f;
                        wrapper.content.leftJoystick.x = leftJoystickInput.x;
                        wrapper.content.leftJoystick.y = leftJoystickInput.y;
                        /*content.Mode = modeTrigger;*/
                        wrapper.content.Mode = modeTrigger;
                        wrapper.content.Control = controlTrigger;
                        /*client.ClientConnect2(content);*/
                    }
                    else if (rightJoystickInput != Vector2.zero)
                    {
                        /*JoystickContent content = new JoystickContent();*/
                        wrapper.content.leftJoystick.x = 0.0f;
                        wrapper.content.leftJoystick.y = 0.0f;
                        wrapper.content.rightJoystick.x = rightJoystickInput.x;
                        wrapper.content.rightJoystick.y = rightJoystickInput.y;
                        /*content.Mode = modeTrigger;*/
                        wrapper.content.Mode = modeTrigger;
                        wrapper.content.Control = controlTrigger;
                        /*client.ClientConnect2(content);*/
                    }
                }
            }
            // Just the left controller is on
            else if (leftController is not null)
            {
                // Check if the trigger button is pressed
                if (leftController.Value.TryGetFeatureValue(CommonUsages.triggerButton, out bool triggerPressed) && triggerPressed)
                {
                    /*Debug.Log("LEFT: Trigger pressed");*/
                    wrapper.content.rightJoystick.x = 0.0f;
                    wrapper.content.rightJoystick.y = 0.0f;
                    wrapper.content.leftJoystick.x = 0.0f;
                    wrapper.content.leftJoystick.y = 0.0f;
                    modeTrigger = (modeTrigger == 1) ? 0 : 1;
                    /*content.Mode = modeTrigger;*/
                    wrapper.content.Mode = modeTrigger;
                    wrapper.content.Control = controlTrigger;
                    /*client.ClientConnect2(content);*/
                }

                // Get left joystick input
                if (leftController.Value.TryGetFeatureValue(CommonUsages.primary2DAxis, out Vector2 joystickInput))
                {
                    // Check joystick directions
                    if (joystickInput != Vector2.zero)
                    {
                        /*Debug.Log("LEFT: Joystick input: " + joystickInput);*/
                        // You can use joystickInput.x and joystickInput.y to determine directions
                        wrapper.content.rightJoystick.x = 0.0f;
                        wrapper.content.rightJoystick.y = 0.0f;
                        wrapper.content.leftJoystick.x = joystickInput.x;
                        wrapper.content.leftJoystick.y = joystickInput.y;
                        /*content.Mode = modeTrigger;*/
                        wrapper.content.Mode = modeTrigger;
                        wrapper.content.Control = controlTrigger;
                        /*client.ClientConnect2(content);*/
                    }
                }
            }
            // Just the right controller is on
            else if (rightController is not null)
            {
                // Get right joystick input
                if (rightController.Value.TryGetFeatureValue(CommonUsages.primary2DAxis, out Vector2 joystickInput))
                {
                    // Check joystick directions
                    if (joystickInput != Vector2.zero)
                    {
                        /*Debug.Log("RIGHT: Joystick input: " + joystickInput);*/
                        // You can use joystickInput.x and joystickInput.y to determine directions
                        wrapper.content.leftJoystick.x = 0.0f;
                        wrapper.content.leftJoystick.y = 0.0f;
                        wrapper.content.rightJoystick.x = joystickInput.x;
                        wrapper.content.rightJoystick.y = joystickInput.y;
                        /*content.Mode = modeTrigger;*/
                        wrapper.content.Mode = modeTrigger;
                        wrapper.content.Control = controlTrigger;
                        /*client.ClientConnect2(content);*/
                    }
                }
            }
            /*flipflop = 0;*/
            /*ModeCounter += 1;*/
            StartCoroutine(postRequest(server_url, JsonConvert.SerializeObject(wrapper, Formatting.Indented)));
            /*}*/
            /*else
            {
                flipflop += 1;
            }*/
        }
        else
        {
            countStart += 1;
            print(countStart);
        }
    }
}

public class JoystickDataWrapper
{
    public JoystickContentRenewNew content { get; set; }

    public JoystickDataWrapper()
    {
        this.content = new JoystickContentRenewNew();
    }
    public class JoystickContentRenewNew
    {
        public int Mode { get; set; }
        public int Control { get; set; }
        public Joystick leftJoystick { get; set; }
        public Joystick rightJoystick { get; set; }


        public JoystickContentRenewNew()
        {
            this.leftJoystick = new Joystick();
            this.rightJoystick = new Joystick();
        }
        public class Joystick
        {
            public float x { get; set; }
            public float y { get; set; }
        }

    }

}

public class SpeechRecognitionTest2 : MonoBehaviour
{
    /*[SerializeField] private Button startButton;
    [SerializeField] private Button stopButton;*/
    /*[SerializeField] private TextMeshProUGUI text;*/
    private string text;
    private AudioClip clip;
    private byte[] bytes;
    private bool recording;

    /*public SpeechRecognitionTest2()
    { 

    }*/
    public void StartRecording()
    {
        /*text.color = Color.white;
        text.text = "Recording...";*/
        text = "RRRecording...";
        Debug.Log(text);
        /*startButton.interactable = false;
        stopButton.interactable = true;*/
        /*clip = Microphone.Start(null, false, 10, 44100);*/
        clip = Microphone.Start("Microphone Array (AMD Audio Device)", false, 10, 48000);
        recording = true;
    }

    private void Update()
    {
        if (recording && Microphone.GetPosition("Microphone Array (AMD Audio Device)") >= clip.samples)
        {
            StopRecording();
        }
    }

    public void StopRecording()
    {
        Debug.Log("SSStop recording");
        var position = Microphone.GetPosition("Microphone Array (AMD Audio Device)");
        Microphone.End("Microphone Array (AMD Audio Device)");
        var samples = new float[position * clip.channels];
        clip.GetData(samples, 0);
        bytes = EncodeAsWAV(samples, clip.frequency, clip.channels);
        recording = false;
        SendRecording();
    }

    public void SendRecording()
    {
        HuggingFaceAPI.AutomaticSpeechRecognition(bytes, response => {
            /*text.color = Color.white;
            text.text = response;*/
            text = response;
        }, error => {
            /*text.color = Color.red;
            text.text = error;*/
            text = error;
        });
    }

    public byte[] EncodeAsWAV(float[] samples, int frequency, int channels)
    {
        using (var memoryStream = new MemoryStream(44 + samples.Length * 2))
        {
            using (var writer = new BinaryWriter(memoryStream))
            {
                writer.Write("RIFF".ToCharArray());
                writer.Write(36 + samples.Length * 2);
                writer.Write("WAVE".ToCharArray());
                writer.Write("fmt ".ToCharArray());
                writer.Write(16);
                writer.Write((ushort)1);
                writer.Write((ushort)channels);
                writer.Write(frequency);
                writer.Write(frequency * channels * 2);
                writer.Write((ushort)(channels * 2));
                writer.Write((ushort)16);
                writer.Write("data".ToCharArray());
                writer.Write(samples.Length * 2);

                foreach (var sample in samples)
                {
                    writer.Write((short)(sample * short.MaxValue));
                }
            }
            return memoryStream.ToArray();
        }
    }

}

public class LogJson
{
    public string transcript;
    public string audioUrl;
    public bool loading;
}