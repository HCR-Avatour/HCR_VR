using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using System.Threading.Tasks;
using System.Net.WebSockets;
using UnityEditor.PackageManager;

public class ControllerInput : MonoBehaviour {
    private InputDevice? leftController, rightController;
    private HTTPClient client = new HTTPClient();
    //private WebSocketClientClass webSocketClient;

    private void Start()
    {
        //webSocketClient = GetComponent<WebSocketClientClass>();
        updateInputDevices();
        InputTracking.nodeAdded += InputTracking_nodeAdded;
       /* await socket.ConnectWebSocket();*/
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

    async void Update()
    {
        /*if (webSocketClient == null)
        {
            print("Reconnecting to websocket...");
            webSocketClient = GetComponent<WebSocketClientClass>();
            print("Reconnected successfully!");
        }*/

        if (leftController?.isValid == false || rightController?.isValid == false) updateInputDevices();

        // Check if the trigger button is pressed
        if (leftController is not null)
        {
            if (leftController.Value.TryGetFeatureValue(CommonUsages.triggerButton, out bool triggerPressed) && triggerPressed)
            {
                Debug.Log("LEFT: Trigger pressed");
                client.ClientConnect("LEFT: Trigger pressed");

                /*if (webSocketClient != null)
                {
                    print("inside if webSocketClient trigger");
                    await webSocketClient.SendMessage("LEFT trigger pressed");
                }*/
            }

            // Get joystick input
            if (leftController.Value.TryGetFeatureValue(CommonUsages.primary2DAxis, out Vector2 joystickInput))
            {
                // Check joystick directions
                if (joystickInput != Vector2.zero)
                {
                    Debug.Log("LEFT: Joystick input: " + joystickInput);
                    // You can use joystickInput.x and joystickInput.y to determine directions
                    client.ClientConnect(joystickInput.ToString());

                    /*if (webSocketClient != null)
                    {
                        print("inside if webSocketClient joystick");
                        await webSocketClient.SendMessage(joystickInput.ToString());
                    }*/
                }
            }
        }
    }
}



