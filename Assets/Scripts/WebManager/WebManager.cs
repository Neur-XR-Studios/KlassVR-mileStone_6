using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using Vuplex.WebView;
using static UnityEngine.Rendering.DebugUI;

public class WebManager : MonoBehaviour
{
    private GamificationManager gamificationManager;
    public GameObject webView;
    public GameObject[] disableitems;
    public CanvasWebViewPrefab _webViewPrefab;
    private string url;
    private float simulationTimer;
    private ServiceScript service;
    public CanvasWebViewLoader canvasWebViewLoader;
    private string simulationScript;
    private GoogleTextToSpeech googleTextToSpeech;
    public TextMeshProUGUI timer;
    private Coroutine timerCoroutine;
    public GameObject buttonNext;
    private bool oneMinutePassed;
    public UnityEvent startingevent;
    public UnityEvent endevent;
    public GameObject simulation;
    public GameObject temp;
    private bool firstTime;
    // Start is called before the first frame update
    void Start()
    {
        googleTextToSpeech=FindObjectOfType<GoogleTextToSpeech>();
        service =FindObjectOfType<ServiceScript>();
        gamificationManager = FindObjectOfType<GamificationManager>();
          

    }
    public float ConvertTimeToSeconds(string displayTime)
    {
        if (string.IsNullOrEmpty(displayTime))
        {
            Debug.LogError("Input time string is null or empty.");
            return 0f;
        }

        // Split the time string into minutes and seconds
        string[] timeComponents = displayTime.Split(':');

        // Check if the time string has the correct format
        if (timeComponents.Length != 2)
        {
            Debug.LogError("Invalid time format. Expected format is MM:ss.");
            return 0f;
        }

        // Parse minutes and seconds from the split string
        if (!int.TryParse(timeComponents[0], out int minutes) || !int.TryParse(timeComponents[1], out int seconds))
        {
            Debug.LogError("Failed to parse minutes or seconds.");
            return 0f;
        }

        // Calculate total seconds
        float totalSeconds = minutes * 60f + seconds;

        return totalSeconds;
    }
    public void StopWebViewAudio()
    {
        if (_webViewPrefab != null && _webViewPrefab.WebView != null && _webViewPrefab.WebView.IsInitialized)
        {
            // Execute JavaScript to stop audio playback within the WebView
            string jsCommand = "var audioElements = document.getElementsByTagName('audio');" +
                               "for(var i = 0; i < audioElements.length; i++) {" +
                               "  audioElements[i].pause();" +
                               "}";

            _webViewPrefab.WebView.ExecuteJavaScript(jsCommand);
        }
    }

    public void AssignSimulationAsync(string time, string newUrl, string script)
    {
        simulationScript = script;
        url = newUrl;
        simulationTimer = ConvertTimeToSeconds(time);
        // StartLoading();
        //  InitializeAndLoadWebView("https://example.com");


        StartSimulation();
    }
    public void StartSimulation()
    {
        if(!string.IsNullOrEmpty(url))
        {
            if(firstTime )
            {
                canvasWebViewLoader.SetupWebView(url);
                simulation.transform.position = temp.transform.position;
                startingevent.Invoke();
                timerCoroutine = StartCoroutine(WebViewDuration(simulationTimer));
                ToggleItem(false);
                if (!string.IsNullOrEmpty(simulationScript))
                {
                    googleTextToSpeech.StartTalking(simulationScript);
                }


            }
            else
            {
                webView.SetActive(true);
                canvasWebViewLoader.SetupWebView(url);
            }
            firstTime = true;
           
        }
        else
        {
            CheckTheStatus();
        }
       
    }
   
    public void ToggleItem(bool value)
    {
      
        foreach (GameObject item in disableitems)
        {
            item.SetActive(value);
        }
       
       

    }
   
    /*private async void StartLoading()
    {

        await _webViewPrefab.WaitUntilInitialized();
        _webViewPrefab.WebView.LoadUrl("https://vuplex.com");

        *//* await _webViewPrefab.WaitUntilInitialized();
         _webViewPrefab.WebView.LoadUrl(url, new Dictionary<string, string>
         {
             ["Authorization"] = "Basic YWxhZGRpbjpvcGVuc2VzYW1l",
             ["Cookie"] = "foo=bar"
         });*//*
    }*/
    
    IEnumerator WebViewDuration(float duration)
    {
       
        float timeRemaining = duration;
        while (timeRemaining > 0)
        {
            // Calculate minutes and seconds
            int minutes = Mathf.FloorToInt(timeRemaining / 60);
            int seconds = Mathf.FloorToInt(timeRemaining % 60);

            // Update the timer text to display in minutes and seconds format
            timer.text = string.Format("{0}:{1:00}", minutes, seconds);

            // aterter reach one min
            if (timeRemaining <= duration - 60f && !oneMinutePassed)
            {
                oneMinutePassed = true;
               buttonNext.SetActive(true);
            }

            yield return new WaitForSeconds(1f); // Wait for 1 second
            timeRemaining -= 1f; // Decrease remaining time by 1 second
        }

        // After the countdown is complete, check the status
        CheckTheStatus();
    }
    public void CheckTheStatus()

    {
        endevent.Invoke();
        if (timerCoroutine!=null)
        {
           StopCoroutine(timerCoroutine);
        }
        StopWebViewAudio();
       // webView.SetActive(false );
        Destroy(webView);
        service.AssignTaskBasedOnCondition(GameStatus.Mcq);
        gamificationManager.AssignGames();
    }
    // Update is called once per frame
  
}
