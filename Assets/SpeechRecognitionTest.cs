using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.IO;
using HuggingFace.API;
using Newtonsoft.Json;
using System.Threading;

public class SpeechRecognitionTest : MonoBehaviour
{

    //[SerializeField] private Button startButton;
    //[SerializeField] private Button stopButton;
    [SerializeField] private TextMeshProUGUI text;

    public ControllerInput4 controller4;

    private AudioClip clip;
    private byte[] bytes;
    private bool recording;

    private void Start()
    {
        //startButton.onClick.AddListener(StartRecording);
        //stopButton.onClick.AddListener(StopRecording);

        controller4 = GetComponent<ControllerInput4>();
    }

    public void StartRecording()
    {
        text.color = Color.white;
        text.text = "Recording...";
        /*startButton.interactable = false;
        stopButton.interactable = true;*/
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
        var position = Microphone.GetPosition("Microphone Array (AMD Audio Device)");
        Microphone.End("Microphone Array (AMD Audio Device)");
        var samples = new float[position * clip.channels];
        clip.GetData(samples, 0);
        bytes = EncodeAsWAV(samples, clip.frequency, clip.channels);
        recording = false;
        SendRecording();
    }

    private void SendRecording()
    {
        string server_url = "https://speech.avatour.duckdns.org/synth_vr";
        /*string server_url = "https://assistant.avatour.duckdns.org/";*/


        HuggingFaceAPI.AutomaticSpeechRecognition(bytes, response => {
            text.color = Color.white;
            text.text = response;
            Debug.Log("Just finished outputting the text");
            TranscriptWrapper wrapper = new TranscriptWrapper();
            wrapper.transcript = response;
            Debug.Log("BBefore sending JSON");
            StartCoroutine(controller4.postRequest(server_url, JsonConvert.SerializeObject(wrapper, Formatting.Indented))); 
            Debug.Log("AAAfter sending JSON");
            
            Thread.Sleep(5000);
            StartCoroutine(controller4.postRequestLog("https://assistant.avatour.duckdns.org/log", JsonConvert.SerializeObject(wrapper, Formatting.Indented)));
            Thread.Sleep(2000);
            /*Debug.Log("killl" + controller4.logExport);
            string testText = controller4.logExport.transcript;
            
            Debug.Log("GGGlobal Variable loggingSRT: " + testText);
            Debug.Log("AAAfter receiving JSON");*/
            

        }, error => {
            text.color = Color.red;
            text.text = error;
        });
        text.color = Color.white;
    }

    private byte[] EncodeAsWAV(float[] samples, int frequency, int channels)
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

public class TranscriptWrapper
{
    public string transcript;

}

