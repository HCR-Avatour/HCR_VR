using UnityEngine;

public class getRicohStream : MonoBehaviour
{

    static WebCamTexture ricohStream;
    string camName = "OBS Virtual Camera"; // Name of your camera. 
    public Material camMaterial;  // Skybox material

    void Start()
    {
        WebCamDevice[] devices = WebCamTexture.devices;
        for (int i = 0; i < devices.Length; i++)
        {
            print("Webcam available: " + devices[i].name);
        }

        if (ricohStream == null)
            ricohStream = new WebCamTexture(camName, 1920, 960); // Resolution you want

        if (!ricohStream.isPlaying)
            ricohStream.Play();

        if (camMaterial != null)
            camMaterial.mainTexture = ricohStream;

    }

}