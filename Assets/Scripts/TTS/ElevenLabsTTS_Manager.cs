using System.Collections;
using UnityEngine.Networking;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using TMPro;

public class ElevenLabsTTS_Manager : MonoBehaviour
{
    // The API endpoint URL
    private string[] voices = { "21m00Tcm4TlvDq8ikWAM", "29vD33N1CtxCmqQRPOHJ", "2EiwWnXFnvU5JabPnv8n", "5Q0t7uMcjvnagumLfvZi", "AZnzlk1XvdvUeBnXmlld", "CYw3kZ02Hs0563khs1Fj", "D38z5RcWu1voky8WS1ja", "EXAVITQu4vr4xnSDxMaL", "ErXwobaYiN019PkySvjV", "GBv7mTt0atIp3Br8iCZE", "IKne3meq5aSn9XLyUdCD", "JBFqnCBsd6RMkjVDRZzb", "LcfcDJNUP1GQjkzn1xUU", "MF3mGyEYCl7XYWbV9V6O", "N2lVS1w4EtoT3dr4eOWO", "ODq5zmih8GrVes37Dizd", "SOYHLrjzK2X1ezoPC6cr", "TX3LPaxmHKxFdv7VOQHJ", "ThT5KcBeYPX3keUQqHPh", "TxGEqnHWrfWFTfGW9XjX", "VR6AewLTigWG4xSOukaG", "XB0fDUnXU5powFXDhCwa", "XrExE9yKIg1WjnnlVkGX", "Yko7PKHZNXotIFUBG7I9", "ZQe5CZNOzWyzPSCn5a3c", "Zlb1dXrM653N07WRdFW3", "bVMeCyTHy58xNoL34h3p", "flq6f7yk4E4fJM5XTYuZ", "g5CIjZEefAph4nQFvHAz", "jBpfuIE2acCO8z3wKNLl", "jsCqWAovK2LkecY7zXl4", "knrPHWnBmmDHMoiMeP3l", "oWAxZDx7w5VEj9dCyTzz", "onwK4e9ZLuTAKqWW03F9", "pFZP5JQG7iQjIQuC4Bku", "pMsXgVXv3BLzUgSXRplE", "pNInz6obpgDQGcFmaJgB", "piTKgcLEGmPE4e6mEKli", "pqHfZKP75CvOlQylNhV4", "t0jbNlBVZ17f02VDIeMI", "wViXBPUzp2ZZixB1xQuM", "yoZ06aMxZJJ28mfd3POQ", "z9fAnlkpzviPz146aGWa", "zcAOhNBS3c14rBihAFp1", "zrHiDhphv9ZnVXBqCLjz"};
    private string apiUrl = "https://api.elevenlabs.io/v1/text-to-speech/";
    private string ApiKey = "f0e2a240bc6900199fb0cbc185a8bdae";

    private AudioSource Audio;/*
    public TMP_InputField Inputfield;*/
    private string InputText;
    private string Model_Id;
    private string Voice_Id;
    public Model model;
    public Voice voice;


   /* //Testing purpose
    public TMP_Dropdown dropdown;
    private string[] testvoices = { "Rachel", "Drew", "Clyde", "Paul", "Domi", "Dave", "Fin", "Bella",
        "Antoni", "Thomas", "Charlie", "George", "Emily", "Elli", "Callum", "Patrick", "Harry",
        "Liam", "Dorothy", "Josh", "Arnold", "Charlotte", "Matilda", "Matthew", "James", "Joseph", 
        "Jeremy", "Michael", "Ethan", "Gigi", "Freya", "Santa_Claus", "Grace", "Daniel", "Lily",
        "Serena", "Adam", "Nicole", "Bill", "Jessie", "Ryan", "Sam", "Glinda", "Giovanni", "Mimi"};*/


    // Start is called before the first frame update

    #region enums

    public enum Model
    {
        eleven_multilingual_v2,
        eleven_multilingual_v1,
        eleven_monolingual_v1,
       
        eleven_turbo_v2
    }

    public enum Voice
    {
        Rachel,
        Drew,
        Clyde,
        Paul,
        Domi,
        Dave,
        Fin,
        Bella,
        Antoni,
        Thomas,
        Charlie,
        George,
        Emily,
        Elli,
        Callum,
        Patrick,
        Harry,
        Liam,
        Dorothy,
        Josh,
        Arnold,
        Charlotte,
        Matilda,
        Matthew,
        James,
        Joseph,
        Jeremy,
        Michael,
        Ethan,
        Gigi,
        Freya,
        Santa_Claus,
        Grace,
        Daniel,
        Lily,
        Serena,
        Adam,
        Nicole,
        Bill,
        Jessie,
        Ryan,
        Sam,
        Glinda,
        Giovanni,
        Mimi
    }

    #endregion

    void Start()
    {        
        Model_Id = model.ToString();
        Voice_Id = voices[(int)voice];
        Debug.Log($"Model: { Model_Id}");
        Debug.Log($"Voice: {Voice_Id}");
        Audio = gameObject.GetComponent<AudioSource>();
        /*StartELTTS
                //Testing purpose
                Initialization();*/

        StartELTTS("hai. arul how are you");
    }

    // Use this function for developement
    public void StartELTTS(string input)
    {
        //Input transcription text
        InputText = input;
        StartCoroutine(SendPostRequest());
    }    

    IEnumerator SendPostRequest()
    {
        string api = apiUrl + Voice_Id;

        // Dynamically include the variable in the JSON request payload
        string jsonData = $@"{{
            ""model_id"": ""{Model_Id}"",
            ""text"": ""{InputText}"",
            ""voice_settings"": {{
                ""similarity_boost"": 1,
                ""stability"": 1,
                ""style"": 1,
                ""use_speaker_boost"": true
            }}
        }}";

        // Create a UnityWebRequest
        UnityWebRequest request = new UnityWebRequest(api, "POST");
        byte[] jsonToSend = System.Text.Encoding.UTF8.GetBytes(jsonData);
        request.uploadHandler = new UploadHandlerRaw(jsonToSend);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");
        request.SetRequestHeader("xi-api-key", ApiKey);

        // Send the request and wait for the response
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.ConnectionError ||
            request.result == UnityWebRequest.Result.ProtocolError)
        {
            Debug.LogError("Error: " + request.error);
            Debug.LogError("Response Code: " + request.responseCode);
            Debug.LogError("Response: " + request.downloadHandler.text);
        }
        else
        {
            // Get the response as a byte array
            byte[] audioData = request.downloadHandler.data;

            // Save the byte array to a file (adjust the path as needed)
            string filePath = Application.persistentDataPath + "/output.mp3";
            File.WriteAllBytes(filePath, audioData);

            Debug.Log("Audio saved to: " + filePath);

            playAudio();
        }
    }    

    private void playAudio()
    {
        // Replace this with the actual file path
        string filePath = Application.persistentDataPath + "/output.mp3";

        // Check if the file exists
        if (System.IO.File.Exists(filePath))
        {
            // Use WWW to load the file
            WWW www = new WWW("file://" + filePath);

            // Wait until the file is loaded
            while (!www.isDone) { }

            // Assign the audio clip to the AudioSource
            Audio.clip = www.GetAudioClip();

            // Play the audio
            Audio.Play();
        }
        else
        {
            Debug.LogError("File not found: " + filePath);
        }
    }



   /* //only for testing purpose
    public void StartTTS()
    {
        //Inputs
        Model_Id = model.ToString();
        Voice_Id = voices[dropdown.value];
        Debug.Log($"Model: { Model_Id}");
        Debug.Log($"Voice: {Voice_Id}");
        InputText = Inputfield.text;
        StartCoroutine(SendPostRequest());
    }

    private void Initialization()
    {
        // Clear existing options
        dropdown.ClearOptions();



        // Create a list to hold Dropdown.OptionData objects
        List<TMP_Dropdown.OptionData> dropdownOptions = new List<TMP_Dropdown.OptionData>();

        // Iterate through the string array and add options to the list
        foreach (string option in testvoices)
        {
            TMP_Dropdown.OptionData dropdownOption = new TMP_Dropdown.OptionData(option);
            dropdownOptions.Add(dropdownOption);
        }

        // Add the list of options to the dropdown
        dropdown.AddOptions(dropdownOptions);
    }*/
}


/*File path for reference

C:/Users/AiWiz-PC-001/AppData/LocalLow/Ai-Wiz/STT_Testing_V_0_1_0/output.mp3
*/

/*Voice lists with name for reference

21m00Tcm4TlvDq8ikWAM
Rachel

29vD33N1CtxCmqQRPOHJ
Drew

2EiwWnXFnvU5JabPnv8n
Clyde

5Q0t7uMcjvnagumLfvZi
Paul

AZnzlk1XvdvUeBnXmlld
Domi

CYw3kZ02Hs0563khs1Fj
Dave

D38z5RcWu1voky8WS1ja
Fin

EXAVITQu4vr4xnSDxMaL
Bella

ErXwobaYiN019PkySvjV
Antoni

GBv7mTt0atIp3Br8iCZE
Thomas

IKne3meq5aSn9XLyUdCD
Charlie

JBFqnCBsd6RMkjVDRZzb
George

LcfcDJNUP1GQjkzn1xUU
Emily

MF3mGyEYCl7XYWbV9V6O
Elli

N2lVS1w4EtoT3dr4eOWO
Callum

ODq5zmih8GrVes37Dizd
Patrick

SOYHLrjzK2X1ezoPC6cr
Harry

TX3LPaxmHKxFdv7VOQHJ
Liam

ThT5KcBeYPX3keUQqHPh
Dorothy

TxGEqnHWrfWFTfGW9XjX
Josh

VR6AewLTigWG4xSOukaG
Arnold

XB0fDUnXU5powFXDhCwa
Charlotte

XrExE9yKIg1WjnnlVkGX
Matilda

Yko7PKHZNXotIFUBG7I9
Matthew

ZQe5CZNOzWyzPSCn5a3c
James

Zlb1dXrM653N07WRdFW3
Joseph

bVMeCyTHy58xNoL34h3p
Jeremy

flq6f7yk4E4fJM5XTYuZ
Michael

g5CIjZEefAph4nQFvHAz
Ethan

jBpfuIE2acCO8z3wKNLl
Gigi

jsCqWAovK2LkecY7zXl4
Freya

knrPHWnBmmDHMoiMeP3l
Santa Claus

oWAxZDx7w5VEj9dCyTzz
Grace

onwK4e9ZLuTAKqWW03F9
Daniel

pFZP5JQG7iQjIQuC4Bku
Lily

pMsXgVXv3BLzUgSXRplE
Serena

pNInz6obpgDQGcFmaJgB
Adam

piTKgcLEGmPE4e6mEKli
Nicole

pqHfZKP75CvOlQylNhV4
Bill

t0jbNlBVZ17f02VDIeMI
Jessie

wViXBPUzp2ZZixB1xQuM
Ryan

yoZ06aMxZJJ28mfd3POQ
Sam

z9fAnlkpzviPz146aGWa
Glinda

zcAOhNBS3c14rBihAFp1
Giovanni

zrHiDhphv9ZnVXBqCLjz
Mimi
*/