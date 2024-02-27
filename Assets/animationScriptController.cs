using System.Collections;
using System.Collections.Generic;
using UnityEditor.PackageManager;
using UnityEngine;
using UnityEngine.XR;

public class animationScriptController : MonoBehaviour
{
    Animator animator;

    // Start is called before the first frame update
    private void Start()
    {
        animator = GetComponent<Animator>();
        Debug.Log(animator);
        /*updateInputDevices();
        InputTracking.nodeAdded += InputTracking_nodeAdded;*/
    }

    // check for new input devices when new XRNode is added
    /*private void InputTracking_nodeAdded(XRNodeState obj)
    {
        updateInputDevices();
    }*/


    // find any devices supporting the desired feature usage
    /*void updateInputDevices()
    {

        var gameControllers = new List<InputDevice>();
        InputDevices.GetDevicesWithRole(InputDeviceRole.LeftHanded, gameControllers);
        leftController = gameControllers.Count == 0 ? null : gameControllers[0];

        gameControllers.Clear();
        InputDevices.GetDevicesWithRole(InputDeviceRole.RightHanded, gameControllers);
        rightController = gameControllers.Count == 0 ? null : gameControllers[0];
    }*/

    // Update is called once per frame
    public async void MoveAvatar(int toWalkOrNot)
    {

        /*if (leftController?.isValid == false || rightController?.isValid == false) updateInputDevices();

        // Check if the trigger button is pressed
        if (leftController is not null)
        {
            if (leftController.Value.TryGetFeatureValue(CommonUsages.triggerButton, out bool triggerPressed) && triggerPressed)
            {
                Debug.Log("LLLLLEFT: Trigger pressed");
                animator.SetBool("isWalking", true);

            }
        }

        if (rightController is not null)
        {
            if (rightController.Value.TryGetFeatureValue(CommonUsages.triggerButton, out bool triggerPressed) && triggerPressed)
            {
                Debug.Log("RRRRRIGHT: Trigger pressed");
                animator.SetBool("isWalking", false);

            }
        }*/

        if (toWalkOrNot == 0)
        {
            animator.SetBool("isWalking", false);
            /*animator.SetTrigger("isWalking");*/
        }
        else if (toWalkOrNot == 1)
        {
            animator.SetBool("isWalking", true);
        }
    }
}
