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

        //check if both controllers are in use
        if ((leftController is not null) && (rightController is not null))
        {
            // Get joystick input
            if ((leftController.Value.TryGetFeatureValue(CommonUsages.primary2DAxis, out Vector2 leftJoystickInput)) && (rightController.Value.TryGetFeatureValue(CommonUsages.primary2DAxis, out Vector2 rightJoystickInput)))
            {
                // Check joystick directions
                if ((leftJoystickInput != Vector2.zero) && (rightJoystickInput != Vector2.zero))
                {
                    /*Debug.Log("LEFT: Joystick input: " + leftJoystickInput + "\nRIGHT: Joystick input: " + rightJoystickInput);*/
                    Debug.Log("LEFT: Joystick input: " + leftJoystickInput.x + ","+ leftJoystickInput.y + "\nRIGHT: Joystick input: " + rightJoystickInput.x + "," + rightJoystickInput.y);
                    // You can use joystickInput.x and joystickInput.y to determine directions
                    JoystickContent content = new JoystickContent();
                    content.right_joystick_x = rightJoystickInput.x;
                    content.right_joystick_y = rightJoystickInput.y;
                    content.left_joystick_x = leftJoystickInput.x;
                    content.left_joystick_y = leftJoystickInput.y;
                    content.mode_trigger = 0;
                    client.ClientConnect(content);
                }
            }
        }

        // Check if the trigger button is pressed
        if (leftController is not null)
        {
            if (leftController.Value.TryGetFeatureValue(CommonUsages.triggerButton, out bool triggerPressed) && triggerPressed)
            {
                Debug.Log("LEFT: Trigger pressed");
                /*client.ClientConnect("LEFT: Trigger pressed");*/
                JoystickContent content = new JoystickContent();
                content.right_joystick_x = 0.0f;
                content.right_joystick_y = 0.0f;
                content.right_joystick_x = 0.0f;
                content.right_joystick_x = 0.0f;
                content.mode_trigger = 1;
                client.ClientConnect(content);

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
                    /*client.ClientConnect(joystickInput.ToString());*/
                    /*client.ClientConnect(joystickInput.x.ToString());
                    client.ClientConnect(joystickInput.y.ToString());*/

                    JoystickContent content = new JoystickContent();
                    content.right_joystick_x = 0.0f;
                    content.right_joystick_y = 0.0f;
                    content.left_joystick_x = joystickInput.x;
                    content.left_joystick_y = joystickInput.y;
                    content.mode_trigger = 0;
                    client.ClientConnect(content);

                    /*if (webSocketClient != null)
                    {
                        print("inside if webSocketClient joystick");
                        await webSocketClient.SendMessage(joystickInput.ToString());
                    }*/
                }
            }
        }

        if (rightController is not null)
        {
            /*if (rightController.Value.TryGetFeatureValue(CommonUsages.triggerButton, out bool triggerPressed) && triggerPressed)
            {
                Debug.Log("RIGHT: Trigger pressed");
                client.ClientConnect("RIGHT: Trigger pressed");

                *//*if (webSocketClient != null)
                {
                    print("inside if webSocketClient trigger");
                    await webSocketClient.SendMessage("LEFT trigger pressed");
                }*//*
            }*/

            // Get joystick input
            if (rightController.Value.TryGetFeatureValue(CommonUsages.primary2DAxis, out Vector2 joystickInput))
            {
                // Check joystick directions
                if (joystickInput != Vector2.zero)
                {
                    Debug.Log("RIGHT: Joystick input: " + joystickInput);
                    // You can use joystickInput.x and joystickInput.y to determine directions

                    /*client.ClientConnect(joystickInput.ToString());*/
                    /* client.ClientConnect(joystickInput.x.ToString());
                     client.ClientConnect(joystickInput.y.ToString());*/

                    JoystickContent content = new JoystickContent();
                    content.left_joystick_x = 0.0f;
                    content.left_joystick_y = 0.0f;
                    content.right_joystick_x = joystickInput.x;
                    content.right_joystick_y = joystickInput.y;
                    content.mode_trigger = 0;
                    client.ClientConnect(content);

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



