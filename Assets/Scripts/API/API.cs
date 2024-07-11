
using GLTFast.Schema;
using SimpleJSON;
using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;

using UnityEngine.Video;


public class API : MonoBehaviour
{
    [System.Serializable]
    public class ApiResponse
    {
        public int code;
        public string message;
        // Include other fields as necessary
    }
    [HideInInspector]
    public string assessmentsString;

    public UnityEvent immediatActionApiDownload;
    private ServiceScript serviceScript;
    private JSONNode modelDetail;
    private string teacherCharacterGender;
    private bool isModelDownload;
    private bool isPlayingVideo;
    private JSONArray modelDetails;
    private int currentIndex = 0;
    public List<string> allModelCoordinates;
    [HideInInspector]
    public Dictionary<string, string> modelIdDatas;
    public List<string> allUrl;
    private ClassVrManager VrManager;
    private int modelDetailsCount;
    private string language;
    List<string> typeOfGames = new List<string>();
    private JSONArray allModelDetails;
    private List<int> modelCoordinateCount = new List<int>();
    private int annotationIncrimenter = 0;
    public GameObject DeviceID;
    public bool isLoadedResponse;
    private VideoManager videoManager;
    public VideoPlayer youtubevideoPlayer;
    public bool is360Image;
   
    public string youtubeURL;
    private AddingImage addingImage;
    private List<float> modelDisplayTime = new List<float>();
    private AssetDownloader assetDownloader;
    private bool isThreeSixty;
    private ThreesixtyImageManager threesixtyImageManager;
    private WebManager webManager;
    private ChangeClassRoom room;
    private void Start()
    {
        room=FindAnyObjectByType<ChangeClassRoom>();
        webManager = FindAnyObjectByType<WebManager>();
        threesixtyImageManager =FindAnyObjectByType<ThreesixtyImageManager>();
        assetDownloader =FindAnyObjectByType<AssetDownloader>();
          VrManager = FindAnyObjectByType<ClassVrManager>();
        addingImage= FindAnyObjectByType<AddingImage>();
        serviceScript = FindAnyObjectByType<ServiceScript>();
        videoManager = FindAnyObjectByType<VideoManager>();
        CallingClassVrExperiance();


    }

    public void CallingClassVrExperiance()
    {
        StartCoroutine(WebGetRequest("https://44.200.7.3/v1/experience"));

    }
    public bool IsmodelDownload()
    {
        return isModelDownload;
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

        //uncomment device id
         req.SetRequestHeader("device-id", SystemInfo.deviceUniqueIdentifier);
       // req.SetRequestHeader("device-id","aj4y");
       
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
            try
            {

                // Attempt to parse the response JSON as an object
                var responseData = JsonUtility.FromJson<ApiResponse>(req.downloadHandler.text);
                Debug.Log("Response Data: " + responseData);
                DeviceID.SetActive(true);
                DeviceIDManager deviceIDManager = DeviceID.GetComponent<DeviceIDManager>();
                deviceIDManager.OneTimeDeviceConfiguration();
            }
            catch (Exception)
            {
                if (DeviceSynHandler.isOpenWelcomeScreen == false && isLoadedResponse != true)
                {
                    isLoadedResponse = true;
                    DeviceIDManager deviceIDManager = DeviceID.GetComponent<DeviceIDManager>();
                    deviceIDManager.ToggleFunction(true);
                    deviceIDManager.gameObject.SetActive(false);
                    // LoadingScene.SetActive(true);
                    var spaceData = JSONNode.Parse(jsonData);
                    JSONNode session = spaceData[0]["session"];
                    string sessionId = session["_id"];
                    string sessionTimeAndDate = session["sessionTimeAndDate"];

                    JSONArray assessments = spaceData[0]["assessments"].AsArray;
                    if (assessments != null && assessments.Count > 0)
                    {
                        foreach (JSONNode assessment in assessments)
                        {
                            string typeOfGame = assessment["typeOfGame"];
                            typeOfGames.Add(typeOfGame);
                        }

                        if (assessments != null && assessments.Count > 0)
                        {
                            assessmentsString = assessments.ToString();
                        }
                    }


                    JSONArray contents = spaceData[0]["content"].AsArray;


                    foreach (JSONNode content in contents)
                    {
                        string script = content["script"];
                        language = content["language"];
                        //  YouTube details


                        string youTubeUrl = content["youTubeUrl"];
                        youTubeUrl = AddingURL(youTubeUrl);
                     
                        bool youTubeVideoAudio = content["youTubeVideoAudio"].AsBool;
                        string youTubeVideoScript = content["youTubeVideoScript"];
                        string youTubeStartTimer = content["youTubeStartTimer"];
                        string youTubeEndTimer = content["youTubeEndTimer"];
                        string classEnvironment = content["classEnvironment"].Value;
                        room.AssignClassRoom(classEnvironment);

                         JSONArray modelDetails = content["modelDetails"].AsArray;
                        if (modelDetails != null)
                        {

                        }
                        modelDetailsCount = modelDetails.Count;
                        VrManager.AddDownloadManager(modelDetailsCount);
                        List<string> allAnnotations = new List<string>();
                        JSONArray simulationDetails = content["simulationDetails"].AsArray;
                        if(simulationDetails!=null && simulationDetails.Count > 0)
                        {
                            foreach (JSONNode simulationDetail in simulationDetails)
                            {
                                string simulationScript = simulationDetail["script"];
                                string simulationURL = simulationDetail["simulationId"]["simulationURL"];
                                string displayTime = simulationDetail["displayTime"];
                                webManager.AssignSimulationAsync(displayTime,simulationURL, simulationScript);
                                Debug.Log("Simulation URL: " + simulationURL);
                                Debug.Log("Display Time: " + displayTime);
                            }
                        }
                      
                        JSONArray imageDetails = content["imageDetails"].AsArray;
                        

                        // Check if imageDetails is not null and has elements
                        if (imageDetails != null && imageDetails.Count > 0)
                        {
                            isThreeSixty=true;

                            // Access the imageURL field from the first element of imageDetails
                            string imageTime  = imageDetails[0]["displayTime"];
                            string imageUrl = imageDetails[0]["ImageId"]["imageURL"];
                            string imageScript = imageDetails[0]["script"];
                            Debug.Log("Image URL: " + imageUrl);
                            if(!string.IsNullOrEmpty(imageUrl))
                            {
                                is360Image = true;
                                addingImage.LoadAndAssignImage(imageUrl, imageScript,imageTime);
                            }
                        }
                        // Iterate over the modelDetails array
                        foreach (JSONNode modelDetail in modelDetails)
                        {
                            string modelTimer = modelDetail["displayTime"];
                            if (modelTimer != null)
                            {
                                string[] timeParts = modelTimer.Split(':');

                                int minutes = int.Parse(timeParts[0]);
                                int seconds = int.Parse(timeParts[1]);


                                int totalSeconds = minutes * 60 + seconds;
                                float totalSecondsFloat = (float)totalSeconds;
                                modelDisplayTime.Add(totalSecondsFloat);
                                PlayerPrefs.SetFloat("totslTime", totalSecondsFloat);

                            }
                            //   float floatTimer = float.Parse(modelTimer);
                            // PlayerPrefs.SetFloat(floatTimer);
                            if (modelDetail["modelId"] != null)
                            {
                                string url = modelDetail["modelId"]["modelUrl"];
                                allUrl.Add(url);
                            }
                            // Check if modelCoordinates exists and is not null
                            if (modelDetail["modelCoordinates"] != null)
                            {
                                // Now check if annotations exist within modelCoordinates
                                JSONNode annotationsNode = modelDetail["modelCoordinates"];
                                if (annotationsNode != null && annotationsNode.AsArray.Count > 0) // Making sure annotations is not null and has elements
                                {
                                    string annotationsJson = annotationsNode.ToString();
                                    modelCoordinateCount.Add(annotationIncrimenter);
                                    allAnnotations.Add(annotationsJson);

                                }
                            }
                            annotationIncrimenter++;
                        }
                        assetDownloader.FetchingTimer(modelDisplayTime);


                        if (modelDetails != null && modelDetails.Count > 0)
                        {
                           
                            isModelDownload = true;
                            threesixtyImageManager.ModelStatus(isModelDownload);
                            allModelDetails = modelDetails;
                            ProcessNextModelDetail();

                        }
                        else
                        {
                            isModelDownload = false;

                        }

                        JSONNode videoDetail = content["videoDetails"][0];
                        if (videoDetail != null && videoDetail.Count > 0)
                        {
                            isPlayingVideo = true;
                            UpdateVideoPreferences(videoDetail);
                        }
                        else if(!string.IsNullOrEmpty(youTubeUrl))
                        {
                            isPlayingVideo = true;
                           
                            
                          //  youTubeUrl = youtubevideoPlayer.url;
                            UpdateYoutubePreferance(youTubeVideoAudio, youTubeVideoScript, youTubeStartTimer, youTubeEndTimer, youTubeUrl);
                         
                        }
                        else
                        {
                            isPlayingVideo = false;

                        }

                        teacherCharacterGender = content["teacherCharacterGender"];
                        PlayerPrefs.SetString("Speak", script);
                        PlayerPrefs.SetString("ImageScript", content["imageScript"]); // Check if this is correct
                                                                                      //  StartCoroutine(WaitUntil(allAnnotations));
                        if (!isPlayingVideo && !isModelDownload)
                        {
                            serviceScript.AssignTaskBasedOnCondition(GameStatus.SkipVideoAndModel);
                        }
                        else
                        {
                            if (isModelDownload)
                            {
                                serviceScript.AssignAnnotation(allAnnotations, modelCoordinateCount);
                            }
                            else
                            {
                                serviceScript.AssignTaskBasedOnCondition(GameStatus.DisbleModelScript);
                            }

                            if (isPlayingVideo)
                            {
                                serviceScript.AssignTaskBasedOnCondition(GameStatus.AssignVideoStatusAsNotSkip);
                            }
                            else
                            {
                                serviceScript.AssignTaskBasedOnCondition(GameStatus.AssignVideoStatusAsSkip);
                            }
                        }
                        

                    }

                    serviceScript.QueryApiAndPassResult(teacherCharacterGender);
                    VrManager.PreferredLanguage(language);
                }
                else
                {
                    Debug.Log("notsynched");
                }
                // If parsing as an object fails, it's likely a plain string response

                //  VrManager.AssignValues(typeOfGames);


            }
        }


    }


   
    private void UpdateModelPreferences(List<Dictionary<string, string>> modelIds)
    {
        string modelScript = modelDetail["script"];
        JSONNode modelId = modelDetail["modelId"];
        string modelName = modelId["modelName"];
        string modelUrl = modelId["modelUrl"].Value.Trim();
        string thumbnailUrl = modelId["thumbnailUrl"];
        PlayerPrefs.SetString(StaticStrings.modelName, modelName);
        PlayerPrefs.SetString(StaticStrings.modelUrl, modelUrl);
        PlayerPrefs.SetString(StaticStrings.thumbnailUrl, thumbnailUrl);
        PlayerPrefs.SetString("ModelScript", modelScript);
    }


    public string AddingURL(string url)
    {
        youtubeURL = url;
        return url;
    }

    //Assign all modewl details here
    public void ProcessNextModelDetail()
    {
        if (allModelDetails != null && currentIndex < allModelDetails.Count)
        {
            modelDetail = allModelDetails[currentIndex];
            Dictionary<string, string> modelIdData = new Dictionary<string, string>();

            // Extract modelId node
            JSONNode modelIdNode = modelDetail["modelId"];
            modelIdData["id"] = modelIdNode["_id"];
            modelIdData["modelName"] = modelIdNode["modelName"];
            modelIdData["description"] = modelIdNode["description"];
            modelIdData["modelUrl"] = modelIdNode["modelUrl"];
            modelIdData["thumbnailUrl"] = modelIdNode["thumbnailUrl"];
            modelIdData["script"] = modelDetail["script"];

            UpdateModelPreferences(modelIdData);
            modelIdDatas = modelIdData;
            currentIndex++; // Increment the index for the next call
        }
        else
        {
            Debug.Log("No more modelDetails to process or modelDetails is null.");
        }
    }
    public void MultipleModelCall()
    {
        // UpdateModelPreferences(modelIdDatas);
        ProcessNextModelDetail();
    }
    private void UpdateModelPreferences(Dictionary<string, string> modelId)
    {
        string modelName = modelId["modelName"];
        string modelUrl = modelId["modelUrl"].Trim();
        string thumbnailUrl = modelId["thumbnailUrl"];
        string modelScript = modelId["script"];

        PlayerPrefs.SetString(StaticStrings.modelName, modelName);
        PlayerPrefs.SetString(StaticStrings.modelUrl, modelUrl);
        PlayerPrefs.SetString(StaticStrings.thumbnailUrl, thumbnailUrl);
        PlayerPrefs.SetString("ModelScript", modelScript);
        PlayerPrefs.Save(); // Save the changes to PlayerPrefs
    }
    private void UpdateVideoPreferences(JSONNode videoDetail)
    {
        string videoScript = videoDetail["script"];
        JSONNode videoId = videoDetail["VideoId"];
        string videoURL = videoId["videoURL"].Value.Trim();
        string videoTitle = videoId["title"];
        string videoDescription = videoId["description"];
        string videoThumbnail = videoId["thumbnail"];
        string videoType = videoId["typeOfVideo"];
        // VrManager.UpdateVideoType(videoType);
        PlayerPrefs.SetString(StaticStrings.videoUrl, videoURL);
        PlayerPrefs.SetString(StaticStrings.videoScript, videoScript);
    }
    public void UpdateYoutubePreferance(bool youTubeVideoAudio, string youTubeVideoScript, string youTubeStartTimer, string youTubeEndTimer,string youTubeUrl)
    {
        PlayerPrefs.SetString(StaticStrings.videoUrl, youTubeUrl);
        PlayerPrefs.SetString(StaticStrings.videoScript, youTubeVideoScript);
        videoManager.AssignYoutubeProperty(youTubeStartTimer, youTubeEndTimer, youTubeVideoAudio, youTubeUrl, youTubeVideoScript);

    }
  
}
public class BypassCertificateHandler : CertificateHandler
{
    protected override bool ValidateCertificate(byte[] certificateData)
    {
        // Always accept
        return true;
    }
}