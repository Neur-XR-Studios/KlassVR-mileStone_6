
using SimpleJSON;
using System;
using System.Collections;
using TMPro;
using Unity.VisualScripting;
using Unity.VisualScripting.Antlr3.Runtime.Tree;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using static TutorialAPI;
using static UnityEngine.Rendering.DebugUI;
using static UnityEngine.Tilemaps.Tilemap;

public class SpaceData
{
    public string experienceId;
    public string schoolId;
    public string sectionId;
    public string gradeId;
}
public class DeviceSynHandler : MonoBehaviour
{
    [System.Serializable]
    public class Data
    {
        public string schoolId;
        public bool isActive;
        public bool isSynced;
        public bool isCompleted;
    }
    [System.Serializable]
    public class Response
    {
        public Data data;
        public string message;
        // Add other fields if needed
    }
    public class DeviceStatus
    {
        //  public string deviceId;
        public bool isActive;
        public bool isSynced;
        public bool isCompleted;
    }
    public class SynchDevice
    {
        public string deviceId;
        public bool isCompleted;
    }
  


    private bool isSync;
    private string jsonData;
    float syncTimer = 0f;
    float syncInterval = 2f;
    public bool isActiveDevice;
    public bool isSyncedDevice;
    public bool isCompletedDevice;
    public static bool isOpenWelcomeScreen;
    public GameObject WelcomeScene;
    private API apiManager;
    private bool isStartGame;
    public  bool isOpenGame;
    public LoadingBar loadingBar;
    public static bool isGameOver;
    public bool isOpening;
    private bool isStartButtonPressed;
    public GamificationManager gamificationManager;
    private bool isStopCalling;
    private bool isFirstTime;
    private bool isLobby=true;
   // public TextMeshProUGUI text;
   
    // Start is called before the first frame update
    void Start()
    {
        isFirstTime=true;
        apiManager = FindAnyObjectByType<API>();
        isActiveDevice = false;
        ActiveDeviceStatus();
        isOpenWelcomeScreen = true;
        isGameOver = false;

    }

    // Update is called once per frame
    void FixedUpdate()
    {
        syncTimer += Time.fixedDeltaTime; // Increment the timer with fixed delta time

        if (syncTimer >= syncInterval)
        {
            syncTimer = 0f; // 

            ActiveDeviceStatus();
            if (isStartGame)
            {
               
                StartCoroutine(WebGetRequest("https://44.200.7.3/v1/device_sync/live_tracking"));
            }
               
        }
    }
    public void ActiveDeviceStatus()
    {

        DeviceStatus status = new DeviceStatus
        {
            //  deviceId = SystemInfo.deviceUniqueIdentifier,
            isActive = true,
            isSynced = isSync,
            isCompleted = isGameOver
        };
        jsonData = JsonUtility.ToJson(status);
        StartCoroutine(SendPatchRequest("https://44.200.7.3/v1/device_sync/device_connecting", jsonData));

    }
    IEnumerator SendPatchRequest(string api, string jsonPayload)
    {
        string url = api;
        UnityWebRequest request = new UnityWebRequest(url, "PATCH");
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonPayload);
        request.uploadHandler = (UploadHandler)new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");
        //unComment
       request.SetRequestHeader("device-id", SystemInfo.deviceUniqueIdentifier);
       // request.SetRequestHeader("device-id", "aj4y");
        //  request.SetRequestHeader("Authorization", $"Bearer {bearerToken}");

        // Use the custom CertificateHandler to bypass certificate validation (for development purposes only)
        request.certificateHandler = new BypassCertificateHandler();

        // Send the request and await a response
        yield return request.SendWebRequest();
       
        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("Error: " + request.error);
        }
        else
        {
            Debug.Log("Success: " + request.downloadHandler.text);
            string value = request.downloadHandler.text;


           
            Response response = JsonUtility.FromJson<Response>(value);
            string message = response.message;
            if(message== "Experience not yet started" && isLobby==false)
            {
                LoadScene();
            }
            isActiveDevice = response.data.isActive;
            isSyncedDevice = response.data.isSynced;
            isCompletedDevice = response.data.isCompleted;

        }
        if (isCompletedDevice)
            yield return null; 
        // Dispose of the certificate handler
        if (request.certificateHandler != null)
        {
            request.certificateHandler.Dispose();
        }
        if(isCompletedDevice)
        {
            Debug.Log("over");
           yield return null;
        }
        if (isActiveDevice && isOpenWelcomeScreen )
        {
            isLobby = false;
            isOpenWelcomeScreen = false;
            WelcomeScene.SetActive(true);
            if (apiManager.isLoadedResponse == false)
            {
                apiManager.CallingClassVrExperiance();
                // isOpenWelcomeScreen = true;
            }

        }
    }
    public void SynDevice()
    {


        isSync = true;
        isStartGame = true;


    }


    public IEnumerator WebGetRequest(string api)
    {

        var apireq = api;
        string jsonData;
        var req = new UnityWebRequest(apireq, "GET");
        //  byte[] jsonToSend = new System.Text.UTF8Encoding().GetBytes(SystemInfo.deviceUniqueIdentifier);
        // req.uploadHandler = (UploadHandler)new UploadHandlerRaw(jsonToSend);
        req.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
        req.SetRequestHeader("Content-Type", "application/json");
        //uncomment 
         req.SetRequestHeader("device-id", SystemInfo.deviceUniqueIdentifier);
        //req.SetRequestHeader("device-id", "aj4y");
        //  req.SetRequestHeader("device-id", "axy");
        //Send the request then wait here until it returns
        req.certificateHandler = new BypassCertificateHandler();

        req.downloadHandler = new DownloadHandlerBuffer();
        yield return req.Send();
        if (req.isNetworkError) // error in request
        {
            Debug.Log("Error While Sending: " + req.error);
        }
        else
        {


            jsonData = req.downloadHandler.text;
          
            var spaceData = JSONNode.Parse(jsonData);
            //uncomment
       /*     if (spaceData["code"] != null && spaceData["code"].AsInt == 401)
            {
                string errorMessage = spaceData["message"];
                Debug.Log("Error: " + errorMessage);

            }
*/
            if (spaceData!=null ||spaceData.Count>0)
            {
                string Start = spaceData[0]["isStart"];
                string Stop = spaceData[0]["isStop"];
                string sID = spaceData[0]["schoolId"];
            
             
                bool isStart = bool.Parse(Start);
                bool isStop = bool.Parse(Stop);
                isStartButtonPressed = isStart;
                if(isFirstTime&& !string.IsNullOrEmpty(spaceData[0]["gradeId"]))
                {
                    string experianceID = spaceData[0]["experienceId"];
                    string schoolId = spaceData[0]["schoolId"];
                    string sectionId = spaceData[0]["sectionId"];
                    string gradeId = spaceData[0]["gradeId"];
                    SpaceData spaceDataObject = new SpaceData
                    {
                        experienceId = experianceID,
                        schoolId = schoolId,
                        sectionId = sectionId,
                        gradeId = gradeId
                    };
                    gamificationManager.StudentPerformance(experianceID, schoolId, sectionId, gradeId);
               //     text.text = ("experienceId " + experianceID + "\nschoolId" + schoolId + "\nsectionId" + sectionId + "\ngradeId" + gradeId);
                    isFirstTime = false;
                }
                
                if (isStart == true && isOpenGame != true)
                {
                    isOpening = true;
                    loadingBar.SynchDevice();
                    isOpenGame = true;
                  


                }
                if (isStop && isOpening)
                {
                    if(!string.IsNullOrEmpty(spaceData[0]["experienceId"]))
                    {
                        if(!isStopCalling)
                        {
                            isStopCalling=true;
                          /*  string experianceID = spaceData[0]["experienceId"];
                            string schoolId = spaceData[0]["schoolId"];
                            string sectionId = spaceData[0]["sectionId"];
                            string gradeId = spaceData[0]["gradeId"];
                            gamificationManager.StudentPerformance(experianceID, schoolId, sectionId, gradeId);*/
                        }
                      
                       /* string currentSceneName = SceneManager.GetActiveScene().name;
                        SceneManager.LoadScene(currentSceneName);*/
                    }
                    
                    
                }
                if (isStop && !isStopCalling)
                {
                   // LoadScene();


                }

            }


        }
    }
    public void ApplicationStatus()
    {
        Debug.Log("reg the account");
    }
    public void LoadScene()
    {
        isGameOver = true;
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(currentSceneIndex);
    }
    
}
