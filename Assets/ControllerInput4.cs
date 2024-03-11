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

public class ControllerInput4 : MonoBehaviour
{
    private InputDevice? leftController, rightController;

    private int modeTrigger = 1;
    private int controlTrigger = 0;
    private int flipflop = 1;
    private int ModeCounter = 0;
    private int countStart;

    private readonly string server_url = "http://avatour.duckdns.org:5000/";
    JoystickDataWrapper wrapper = new JoystickDataWrapper();

    private void Start()
    {
        updateInputDevices();
        InputTracking.nodeAdded += InputTracking_nodeAdded;

        /*Kinda obtuse but who knows, it might work to regulate the frequency of POSTs sent lol*/
        /*This should start instantly and call the postForMe function 20 times per second which shouldn't be many at all.*/
        /*InvokeRepeating("postForMe", 0, 0.05f);*/
    }

    private void postForMe()
    {
        /*StartCoroutine(postRequest("http:///www.yoururl.com", "your json"));*/
        StartCoroutine(postRequest(server_url, JsonUtility.ToJson(wrapper)));
    }

    IEnumerator postRequest(string url, string json)
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
            Debug.Log("Received: " + uwr.downloadHandler.text);
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


