using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.IO;
using System.Collections.Generic;
using System;
using UnityEngine.Events;
using SimpleJSON;
public enum Language
{
    Spanish,
    Arabic,
    English,
    Malayalam,
    Tamil
}

public class GoogleTextToSpeech : MonoBehaviour
{
   
    public  enum GendeR
    {
        MALE,
        FEMALE
    }
   

    [SerializeField]
    private GendeR selectedGender = GendeR.FEMALE; // Default to female
    [SerializeField]
    private Language selectedLanguage = Language.Spanish;

    public string textToSynthesize = "hi dinesh i hate you. we are playing 360 video here";

    private string languageCode = "ar-XA";
    private string targetLanguage = "ar";


    // private string apiKey = "AIzaSyARPp4C_7l6yRrm1zYEwyD8lgQQripZKMg";
    private string apiKeyTextToSpeech = "AIzaSyBZiQ5-LPnKIFVcQGmJma7Yb37iswv6aQY";
    private string apiKeyTranslation = "AIzaSyDX9tewMp8PV3ITfnzVC9uCOtzOm-GLZng";

    private string voiceName;
    private string ssmlGender = "FEMALE";
    private string audioEncoding = "MP3";
    private string apiUrl = "https://texttospeech.googleapis.com/v1/text:synthesize?key=";
    private string apiUrlTranslation = "https://translation.googleapis.com/language/translate/v2";

    public AudioSource audioSource;
    private bool isFirstTime;
    [HideInInspector]
    public float audioClipLength;
    public GameObject male;
    public GameObject feMale;
    private GameStatus gameStatus;
    public bool IsAudioReady { get; private set; }
    private ServiceScript serviceScript;
    private string textToTranslate;
    private string language;
  

    void OnEnable()
    {
        serviceScript=FindAnyObjectByType<ServiceScript>();
         isFirstTime =true;
     //  SetVoiceNameAndGender(selectedGender);
        
    }
    public void InitialVideoStatus(GameStatus currentstatus)
    {
        gameStatus = currentstatus;
    }
    public void AssistantStartTalking()
    {
        StartTalking(PlayerPrefs.GetString("Speak"));
    }
    public void StartTalking(string text)
    {
        // textToSynthesize = text;
        //  StartCoroutine(SendTextToSpeechRequest());
       // LanguageSelector();
        StartCoroutine(SendTranslationRequest(text));
    }
    public void LanguageSelector(Language selectedLanguage)
    {
        switch (selectedLanguage)
        {
            case Language.Spanish:
                languageCode = "es-ES";
                targetLanguage = "es";
                break;
            case Language.Arabic:
                languageCode = "ar-XA";
                targetLanguage = "ar";
                break;
            case Language.English:
                languageCode = "en-US";
                targetLanguage = "en";
                break;
            case Language.Tamil: 
                languageCode = "ta-IN";
                targetLanguage = "ta";
                break;
            case Language.Malayalam:
                languageCode = "ml-IN";
                targetLanguage = "ml";
                break;
            default:
                print("Incorrect intelligence level.");
                break;
        }
    }
    public void SetVoiceNameAndGender(GendeR gender)
    {
        // voiceName = GetVoiceName( );
       // gender = GendeR.FEMALE;
        ssmlGender = GetSsmlGender(gender);
    }
    string GetSsmlGender(GendeR gender)
    {
        if (gender==GendeR.MALE)
        {
            GetVoiceName(GendeR.MALE);
            male.SetActive(true);
            Destroy(feMale);
           // feMale.SetActive(false);
        }
        else
        {
            feMale.SetActive(true);
            Destroy(male);
            //male.SetActive(false);
            GetVoiceName(GendeR.FEMALE);
        }
        return gender == GendeR.FEMALE ? "FEMALE" : "MALE";
    }
    string GetVoiceName(GendeR gender)
    {
        return gender == GendeR.FEMALE ? "en-US-Neural2-G" : "en-US-Neural2-D"; 
    }
    [System.Serializable]
    public class TextToSpeechResponse
    {
        public string audioContent;
    }

    IEnumerator SendTranslationRequest(string text)
    {
        textToTranslate = text;
        // Create the request payload
        string payload = $"{{\"q\": \"{textToTranslate}\", \"target\": \"{targetLanguage}\"}}";

        // Create UnityWebRequest
        UnityWebRequest request = new UnityWebRequest(apiUrlTranslation + "?key=" + apiKeyTranslation, "POST");
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(payload);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");
        request.SetRequestHeader("charset", "utf-8");

        // Send the request
        yield return request.SendWebRequest();

        // Check for errors
        if (request.result == UnityWebRequest.Result.ConnectionError ||
            request.result == UnityWebRequest.Result.ProtocolError)
        {
            Debug.LogError("Error: " + request.error);
        }
        else
        {
            // Print response
            Debug.Log("Translation Response: " + request.downloadHandler.text);
            string jsonData = request.downloadHandler.text;
            var parseData = JSONNode.Parse(jsonData);
            JSONNode translation = parseData["data"]["translations"][0]["translatedText"];

            StartCoroutine(SendTextToSpeechRequest(translation));
        }
    }
    public void StopAudio()
    {
        if (audioSource.isPlaying)
        {

            audioSource.Stop();
        }
    }
IEnumerator SendTextToSpeechRequest(string text)
    {
        textToSynthesize = text;
        string jsonPayload = $"{{\"input\": {{\"text\": \"{textToSynthesize}\"}}, \"voice\": {{\"languageCode\": \"{languageCode}\", \"name\": \"{voiceName}\", \"ssmlGender\": \"{ssmlGender}\"}}, \"audioConfig\": {{\"audioEncoding\": \"{audioEncoding}\"}}}}";

        UnityWebRequest request = new UnityWebRequest(apiUrl + apiKeyTextToSpeech, "POST");
        byte[] jsonToSend = System.Text.Encoding.UTF8.GetBytes(jsonPayload);
        request.uploadHandler = new UploadHandlerRaw(jsonToSend);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
        {
            Debug.LogError("Error: " + request.error);
            Debug.LogError("Response Code: " + request.responseCode);
            Debug.LogError("Response: " + request.downloadHandler.text);
        }
        else
        {
            string jsonResponse = request.downloadHandler.text;

            TextToSpeechResponse response = JsonUtility.FromJson<TextToSpeechResponse>(jsonResponse);

            if (response != null && !string.IsNullOrEmpty(response.audioContent))
            {
                DecodeAndPlayAudio(response.audioContent);
            }
            else
            {
                Debug.LogError("Invalid response or empty audio content.");
            }
        }
    }


    void DecodeAndPlayAudio(string base64AudioContent)
    {
        try
        {
            byte[] audioData = Convert.FromBase64String(base64AudioContent);
            string fileName = "temp_audio_" + DateTime.Now.Ticks + ".wav"; // You may want to change the file ext 
            string fullPath = Path.Combine(Application.persistentDataPath, fileName);
            File.WriteAllBytes(fullPath, audioData);

            if (File.Exists(fullPath))
            {
                StartCoroutine(LoadAndPlayAudio(fullPath));
            }
            else
            {
                Debug.LogError("Audio file not found in persistentDataPath: " + fullPath);
            }
        }
        catch (Exception e)
        {
            Debug.LogError("Error decoding audio: " + e.Message);
        }
    }

    IEnumerator LoadAndPlayAudio(string path)
    {
        using (WWW www = new WWW("file://" + path))
        {
            yield return www;

            if (string.IsNullOrEmpty(www.error))
            {
                AudioClip audioClip = www.GetAudioClip(false, false, AudioType.MPEG);

                // Play the loaded audio clip
                audioSource.clip = audioClip;
                audioSource.Play();
                IsAudioReady = true;
                // Wait for the audio to finish playing
                audioClipLength = audioClip.length;
                yield return new WaitForSeconds(audioClip.length);
                IsAudioReady = false;
                MyUtilityClass utilityInstance = new MyUtilityClass(() =>
                {

                    Debug.Log("Performing the custom action!");
                });
                
         
                if(isFirstTime)
                {
                    serviceScript.AssignTaskBasedOnCondition(gameStatus);
                    isFirstTime = false;
                }
                
                File.Delete(path);
            }
            else
            {
                Debug.LogError("Error loading audio file: " + www.error);
            }
        }
    }


    [System.Serializable]
    public class TranslationData
    {
        public List<TranslationItem> translations;
    }

    [System.Serializable]
    public class TranslationItem
    {
        public string translatedText;
        // You can include additional fields if needed
    }

    [System.Serializable]
    public class TranslateTextResponseList
    {
        public TranslationData data;
    }

    [System.Serializable]
    public class Data
    {
        public Translation[] translations;
    }

    [System.Serializable]
    public class Translation
    {
        public string translatedText;
    }
}
