using System.Collections;
using System.Collections.Generic;
using TriLibCore.Dae.Schema;
using UnityEngine;


public class ClassVrManager : MonoBehaviour
{
    private VideoManager videoManager;
    private GoogleTextToSpeech googleTextToSpeech;
    private List<GameObject> selectedModels = new List<GameObject>();
    public GameObject[] modelsDownloader;
    private DownloadManager downloadManager;
    public LoadingBar loadingBar;
    private RuntimeImportBehaviour runtimeImportBehaviour;
    private GamificationManager gamificationManager;
    void Start()
    {
        gamificationManager=FindAnyObjectByType<GamificationManager>();
       
        downloadManager =FindObjectOfType<DownloadManager>();
        videoManager=FindAnyObjectByType<VideoManager>();
        googleTextToSpeech=FindAnyObjectByType<GoogleTextToSpeech>();
       
    }
    public void AssignValues(List<string> typeOfGame)
    {
        gamificationManager.AssignGames(typeOfGame);
    }
    public void PreferredLanguage(string language)
    {
        if(language == "arabic")
        {
            googleTextToSpeech.LanguageSelector(Language.Arabic);
        }
        else if(language == "spanish")
        {
            googleTextToSpeech.LanguageSelector(Language.Spanish);
        }
        else
        {
            googleTextToSpeech.LanguageSelector(Language.English);
        }


            
    }
   
    public void UpdateVideoType(string type)
    {
      //  videoManager.ChangeVideoType(type);
    }
    public void AddDownloadManager(int modelDetailsCount)
    {
        selectedModels.Clear();

        // Add the GameObjects to the list based on modelDetailsCount
        for (int i = 0; i < modelDetailsCount; i++)
        {
            selectedModels.Add(modelsDownloader[i]);
            modelsDownloader[i].SetActive(true);
            runtimeImportBehaviour = modelsDownloader[i].gameObject.GetComponent<RuntimeImportBehaviour>();
        }
        downloadManager.EnbleCallback();
       // LoadingBarSimulator();
    }
    public void LoadingBarSimulator()
    {
        loadingBar.SimulateLoading(runtimeImportBehaviour);
    }
    void Update()
    {
        
    }
}
