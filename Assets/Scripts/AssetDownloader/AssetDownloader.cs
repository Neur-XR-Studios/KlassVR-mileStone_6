
using System.Collections;
using UnityEngine;

using UnityEngine.Events;

using TriLibCore.Samples;
using static ElevenLabsTTS_Manager;
using TMPro;
using UnityEngine.SceneManagement;
using static UnityEngine.Rendering.DebugUI;
using System;
using SimpleJSON;
using static UnityEngine.GraphicsBuffer;
using System.Collections.Generic;

public class AssetDownloader : MonoBehaviour
{
  
    public Transform instantiationPoint;
    private GoogleTextToSpeech googleTextToSpeech;
    private AnnotationManager annotationManager;
    public  RuntimeImportBehaviourHelper[] runtimeImportBehaviourHelper;

    //timer

    public float timeRemaining = 10;
    public bool timerIsRunning = false;
    public TextMeshProUGUI timerText;
    public  ServiceScript service;
    private bool isMCQ=true;

    private int count = 3; // How many times to call the function
    private float interval; // Time between function calls
    private float nextThreshold; // Next time to call the function
    private int timesCalled = 0; //
    private int modelcounter;
    public  API api;
    private int modelCount;
    private JSONArray modelDetails;
    private GameObject model;
    private List<float> modelTimer=new List<float>();
    private List<float> thresholds ;
    private float currentTimerIndex;
    private WebManager webManager;
    public TextMeshProUGUI text;
    private void Start()
    {
        
      
          webManager=FindObjectOfType<WebManager>();
          annotationManager =FindAnyObjectByType<AnnotationManager>();
          googleTextToSpeech =FindAnyObjectByType<GoogleTextToSpeech>();
    }
    public void TimerStart()
    {
        timeRemaining = 0;
        foreach (float time in modelTimer)
        {
            timeRemaining += time;
        }

        if (timeRemaining == 0)
        {
            timeRemaining = 120f; // Fallback default time
        }

        count = modelTimer.Count; // Set count based on the number of intervals in the modelTimer list
        timesCalled = 0;
        timerIsRunning = true;
        if(modelTimer.Count>=2)
        {
            currentTimerIndex = timeRemaining - modelTimer[0];
        }
        
    }

    public void DownloadingAssetBackgrount()
    {
        if(api.IsmodelDownload())
        {
            //ChangeModelPosition();
            annotationManager.initialization();
            StartCoroutine(AssinOperation());
            timerIsRunning = true;
        }
       
    }
    public void FetchingTimer(List<float> timer)
    {
        modelTimer=timer;
    }
    public void StartNextOperation()
    {
        
    }
    private IEnumerator AssinOperation()
    {
        googleTextToSpeech.StartTalking(PlayerPrefs.GetString("ModelScript"));
        yield return new WaitUntil(() => googleTextToSpeech.IsAudioReady);
      
       
       
       
    }
    public void AssignMCQStatus(bool value)
    {
        isMCQ = value;
    }
    private void Perform()
    {
        webManager.StartSimulation();
       //  service.AssignTaskBasedOnCondition(GameStatus.Mcq);

        /* if (!string.IsNullOrWhiteSpace(api.youtubeURL))
         {
            // service.AssignTaskBasedOnCondition(GameStatus.threesixtyImage);
         }
         else if(api.is360Image)
         {
             //SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
           //  service.AssignTaskBasedOnCondition(GameStatus.threesixtyImage);
         }
         else
         {
            // service.AssignTaskBasedOnCondition(GameStatus.threesixtyImage);
             Debug.Log("display final result");
         }*/


    }
    public void AssignModelDetails(JSONArray modelData)
    {
        modelDetails = modelData;
    }
    void FixedUpdate()
    {
        if (timerIsRunning)
        {
            if (timeRemaining > 0)
            {
                timeRemaining -= Time.deltaTime;

                // Check if we've crossed the next threshold to call the function
                if (timesCalled < modelTimer.Count && timeRemaining <= currentTimerIndex)
                {
                    int counter;
                    CallFunction(); // Your function to call
                    timesCalled++;
                    counter = timesCalled + 1;
                    if (modelTimer.Count >= counter)
                    {
                        currentTimerIndex = timeRemaining - modelTimer[timesCalled];
                        
                    }
                  
                }

                DisplayTime(timeRemaining); // Assume this method correctly displays the remaining time
            }
            else
            {
                CallFunction();
                Debug.Log("Time has run out!");
                timeRemaining = 0;
                timerIsRunning = false;
                Perform(); // Perform whatever needs to be done when time runs out
            }
        }
    }

    void DisplayTime(float timeToDisplay)
    {
        timeToDisplay += 1;

        float minutes = Mathf.FloorToInt(timeToDisplay / 60);
        float seconds = Mathf.FloorToInt(timeToDisplay % 60);

        timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }
    void CallFunction()
    {
        text.text = "";
        
        // We are deleting the previous model, that's why we increment "modelCount" after the "if" loop. If "allUrl" is less than 3, then we need to keep models until the timer ends.


        if (modelCount < api.allUrl.Count)
            runtimeImportBehaviourHelper[modelCount].DisableObject();

        // The variable "modelCount" is used to check the total number of models. Then, the "DisableObject()" method of the runtime import behavior helper corresponding to the model count is invoked.
        modelCount++;
       

        //The "apiUrlCount" variable is used to understand the number of available models from the API. Here, we check if the model count is less than the total number of URLs retrieved from the API.

        if (modelCount < api.allUrl.Count)
        {
            api.MultipleModelCall();
            runtimeImportBehaviourHelper[modelCount].customCallBack(PlayerPrefs.GetString("ModelScript"));
          
            annotationManager.MultipleModelCall(modelCount);
          
        }
    
       


    }
  
    public void  ChangeModelPosition()
     {
       
       GameObject collectionPopint = GameObject.Find("CollectionPointCloneScene");
       GameObject VrModelParent = collectionPopint.transform.GetChild(0).gameObject;
       Transform parent = VrModelParent.transform.GetChild(0);
       Transform child = parent.transform.GetChild(0);

        /*  VrModelParent.transform.position = instantiationPoint.transform.position;
          VrModelParent.transform.rotation = instantiationPoint.transform.rotation;*/


        //---------------
        model = VrModelParent.gameObject;
       // model.AddComponent<ScaleController>();
        Vector3 parentBottomPosition = GetBottomPosition(VrModelParent);

        // Find the bottom position of the target
        Vector3 targetBottomPosition = GetBottomPosition(instantiationPoint.transform.gameObject);
       
        // Calculate the offset to align their bottom positions
        Vector3 offset = parentBottomPosition - targetBottomPosition;

        // Apply the offset to the parentObject's position
        model.transform.position -= offset;
        // child.transform.localPosition = Vector3.zero;

        StartCoroutine(ChildZero(child.transform));
     }
    IEnumerator ChildZero(Transform child )
    {
        yield return new WaitForSeconds(3f);
      //  child.transform.localPosition = Vector3.zero;
    }
    Vector3 GetBottomPosition(GameObject obj)
    {
        // Get the bounds of the GameObject
        Bounds bounds;
        Renderer renderer = obj.GetComponent<Renderer>();

        /*if (renderer != null)
        {
            bounds = renderer.bounds;
        }
        else
        {*/
            Collider collider = obj.GetComponent<Collider>();
            if (collider != null)
            {
                bounds = collider.bounds;
            }
            else
            {
                Debug.LogError("GameObject has neither renderer nor collider.");
                return Vector3.zero;
            }
       /* }*/

        // Calculate the bottom position
        Vector3 bottomPosition = bounds.center - Vector3.up * bounds.extents.y;
      
        return bottomPosition;
    }

}
