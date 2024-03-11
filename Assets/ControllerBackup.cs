/*using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using System.Threading.Tasks;
using System.Net.WebSockets;
using UnityEditor.PackageManager;
using Unity.VisualScripting;
using System.Net.Http;

public class ControllerInput3 : MonoBehaviour
{
    private InputDevice? leftController, rightController;
    private HTTPClient2 client = new HTTPClient2();
    private int modeTrigger = 1;
    private int flipflop = 1;
    private int ModeCounter = 0;
    private int countStart;
    JoystickContent2 content = new JoystickContent2();
    private void Start()
    {
        updateInputDevices();
        InputTracking.nodeAdded += InputTracking_nodeAdded;
        *//*InvokeRepeating("client.ClientConnect(content)", 0, 0.1f);*//*
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

        if (countStart == 500)
        {

            if (flipflop == 29)
            {

                if (leftController?.isValid == false || rightController?.isValid == false) updateInputDevices();

                // Just the left trigger
                if (leftController is not null)
                {
                    if (leftController.Value.TryGetFeatureValue(CommonUsages.triggerButton, out bool triggerPressed) && triggerPressed)
                    {
                        *//*Debug.Log("LEFT: Trigger pressed");*//*
                        content.right_joystick_x = 0.0f;
                        content.right_joystick_y = 0.0f;
                        content.left_joystick_x = 0.0f;
                        content.left_joystick_y = 0.0f;
                        modeTrigger = (modeTrigger == 1) ? 0 : 1;
                        *//*content.mode_trigger = modeTrigger;*//*
                        content.mode_trigger = ModeCounter;
                        client.ClientConnect2(content);
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
                            *//*Debug.Log("LEFT: Joystick input: " + leftJoystickInput.x + ","+ leftJoystickInput.y + "\nRIGHT: Joystick input: " + rightJoystickInput.x + "," + rightJoystickInput.y);*//*
                            // You can use joystickInput.x and joystickInput.y to determine directions

                            content.right_joystick_x = rightJoystickInput.x;
                            content.right_joystick_y = rightJoystickInput.y;
                            content.left_joystick_x = leftJoystickInput.x;
                            content.left_joystick_y = leftJoystickInput.y;
                            *//*content.mode_trigger = modeTrigger;*//*
                            content.mode_trigger = ModeCounter;
                            client.ClientConnect2(content);
                        }
                        else if (leftJoystickInput != Vector2.zero)
                        {
                            *//*JoystickContent content = new JoystickContent();*//*
                            content.right_joystick_x = 0.0f;
                            content.right_joystick_y = 0.0f;
                            content.left_joystick_x = leftJoystickInput.x;
                            content.left_joystick_y = leftJoystickInput.y;
                            *//*content.mode_trigger = modeTrigger;*//*
                            content.mode_trigger = ModeCounter;
                            client.ClientConnect2(content);
                        }
                        else if (rightJoystickInput != Vector2.zero)
                        {
                            *//*JoystickContent content = new JoystickContent();*//*
                            content.left_joystick_x = 0.0f;
                            content.left_joystick_y = 0.0f;
                            content.right_joystick_x = rightJoystickInput.x;
                            content.right_joystick_y = rightJoystickInput.y;
                            *//*content.mode_trigger = modeTrigger;*//*
                            content.mode_trigger = ModeCounter;
                            client.ClientConnect2(content);
                        }
                    }
                }
                // Just the left controller is on
                else if (leftController is not null)
                {
                    // Check if the trigger button is pressed
                    if (leftController.Value.TryGetFeatureValue(CommonUsages.triggerButton, out bool triggerPressed) && triggerPressed)
                    {
                        *//*Debug.Log("LEFT: Trigger pressed");*//*
                        content.right_joystick_x = 0.0f;
                        content.right_joystick_y = 0.0f;
                        content.left_joystick_x = 0.0f;
                        content.left_joystick_y = 0.0f;
                        modeTrigger = (modeTrigger == 1) ? 0 : 1;
                        *//*content.mode_trigger = modeTrigger;*//*
                        content.mode_trigger = ModeCounter;
                        client.ClientConnect2(content);
                    }

                    // Get left joystick input
                    if (leftController.Value.TryGetFeatureValue(CommonUsages.primary2DAxis, out Vector2 joystickInput))
                    {
                        // Check joystick directions
                        if (joystickInput != Vector2.zero)
                        {
                            *//*Debug.Log("LEFT: Joystick input: " + joystickInput);*//*
                            // You can use joystickInput.x and joystickInput.y to determine directions
                            content.right_joystick_x = 0.0f;
                            content.right_joystick_y = 0.0f;
                            content.left_joystick_x = joystickInput.x;
                            content.left_joystick_y = joystickInput.y;
                            *//*content.mode_trigger = modeTrigger;*//*
                            content.mode_trigger = ModeCounter;
                            client.ClientConnect2(content);
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
                            *//*Debug.Log("RIGHT: Joystick input: " + joystickInput);*//*
                            // You can use joystickInput.x and joystickInput.y to determine directions
                            content.left_joystick_x = 0.0f;
                            content.left_joystick_y = 0.0f;
                            content.right_joystick_x = joystickInput.x;
                            content.right_joystick_y = joystickInput.y;
                            *//*content.mode_trigger = modeTrigger;*//*
                            content.mode_trigger = ModeCounter;
                            client.ClientConnect2(content);
                        }
                    }
                }
                flipflop = 0;
                ModeCounter += 1;
            }
            else
            {
                flipflop += 1;
            }
        }
        else
        {
            countStart += 1;
            print(countStart);
        }
    }
}*/