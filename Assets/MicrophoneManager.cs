using UnityEngine;

public class MicrophoneManager : MonoBehaviour
{
    void Start()
    {
        // Get an array of available microphone devices
        string[] microphoneDevices = Microphone.devices;

        // Check if there are any microphone devices available
        if (microphoneDevices.Length == 0)
        {
            Debug.Log("No microphone devices found.");
        }
        else
        {
            Debug.Log("AAAvailable microphone devices:");
            // Iterate through the array and log information about each microphone device
            foreach (string deviceName in microphoneDevices)
            {
                int minFreq, maxFreq;
                Microphone.GetDeviceCaps(deviceName, out minFreq, out maxFreq);
                Debug.Log("Device Name: _" + deviceName + "_");
                Debug.Log("Minimum Frequency: " + minFreq);
                Debug.Log("Maximum Frequency: " + maxFreq);
                Debug.Log("Is Currently Recording: " + Microphone.IsRecording(deviceName));
                Debug.Log("Get Position: " + Microphone.GetPosition("Microphone Array (AMD Audio Device)"));
                /*Debug.Log("Get Position: " + Microphone.GetPosition(""));*/
            }
        }
    }
}


