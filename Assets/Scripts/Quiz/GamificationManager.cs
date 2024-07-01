using SimpleJSON;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;


[System.Serializable]
public struct AnswerData
{
    public string text;
    public bool isCorrect;
    public string _id;
}
public class GamificationManager : MonoBehaviour
{
    public class StudentScore
    {
        public string studentID;
        public string experienceConductedID;
        public string schoolId;
        public string sectionID;
        public string gradeID;
        public int score;

    }
    public class DeviceStatus
    {
        public bool isCompleted;
    }
    public GameObject arrowGame;
    public GameObject basketballGame;
    public GameObject mcqGame;
    public GameObject gamificationCollider;
    private List<string> selectedGames = new List<string>(); // Array to store selected game names
    private int currentGameIndex = 0;
    public GameObject thankyouCanvas;
    private class Wrapper
    {
        public List<QuestionData> questions;
    }

    [System.Serializable]
    public struct QuestionData
    {
        public string question;
        public List<AnswerData> options;
        public string createdBy;
        public string sessionId;
        public string id;
        public string typeOfGame;
    }


    private int currentQuestionIndex = 0;
    public MeshRenderer[] meshRender;
    public API apiManager;
    private string gamificationType;
    private string questionPanelText;
    private string actualQuestion;
    private string answer;
    public List<QuestionData> questions;
    public List<List<AnswerData>> allOptions = new List<List<AnswerData>>();
    private List<string> typeOfGame = new List<string>();
    private int gameCount;
    //public  BasketBallQuiz[] gameType;
    private List<string> headingDetails = new List<string>();
    List<string[]> options = new List<string[]>();
    List<string> questionlist = new List<string>();
    public string currentGameName;
    public GameObject arrowVisualize;
    public  int arrowScore;
    public  int basketBallScore;
    public  int mcqScore;

    public GameObject[] disableObjects;
    public GameObject classRoom;
    public GameObject welcomeSessionScene;
   

    private void Start()
    {

        //  apiManager =FindAnyObjectByType<API>();
        //  StartCoroutine(EnableThankYouCanvas());


    }
  
    public void AssignGames()
    {
        if (string.IsNullOrEmpty(apiManager.assessmentsString))
        {
           // StartCoroutine(EnableThankYouCanvas());
           thankyouCanvas.SetActive(true);
        }
        else
        {
            //welcomeSessionScene.SetActive(true);
            Invoke("StartAfterLoadingCanvas", 1f);
        }






    }
    public void StartAfterLoadingCanvas()
    {

        Wrapper wrapper = JsonUtility.FromJson<Wrapper>("{\"questions\":" + apiManager.assessmentsString + "}");
        questions = wrapper.questions;

        List<AnswerData> currentOptions = new List<AnswerData>();
        foreach (var questionData in wrapper.questions)
        {
            Debug.Log("Type of Game: " + questionData.typeOfGame);
        }

        gamificationType = $"MCQ{currentQuestionIndex + 1}";
        questionPanelText = $"Question{currentQuestionIndex + 1}";
        actualQuestion = $"{questions[currentQuestionIndex].question}";
        headingDetails.Add(gamificationType);
        headingDetails.Add(questionPanelText);
        headingDetails.Add(actualQuestion);

        foreach (var questionData in questions)
        {

            allOptions.Add(questionData.options);
            typeOfGame.Add(questionData.typeOfGame);
            questionlist.Add(questionData.question);


        }
        EnableNextGame(typeOfGame[gameCount]);

    }
    private void EnableNextGame(string currentGame)
    {
        if (currentGameIndex <= allOptions.Count)
        {
            // string nextGameName = selectedGames[currentGameIndex];
            currentGameName = currentGame;
            switch (currentGameName)
            {
                case "Archery":
                 //   arrowScore++;
                    arrowGame.SetActive(true);
                    arrowVisualize.SetActive(true);
                    gamificationCollider.SetActive(true);
                    arrowGame.GetComponent<BasketBallQuiz>().DisplayQuestion(headingDetails, allOptions[gameCount], questionlist[gameCount]);
                    break;
                case "Basketball":
                  //  basketBallScore++;
                    basketballGame.SetActive(true);

                    gamificationCollider.SetActive(true);
                    basketballGame.GetComponent<BasketBallQuiz>().DisplayQuestion(headingDetails, allOptions[gameCount], questionlist[gameCount]);
                    break;
                case "MCQ":
                   // mcqScore++;
                    mcqGame.GetComponent<QuizManagers>().DisplayQuestion(headingDetails, allOptions[gameCount], questionlist[gameCount]);
                    mcqGame.SetActive(true);
                    break;
                default:
                    Debug.LogWarning("Unknown game name: " + currentGameName);
                    break;
            }
            currentGameIndex++; // Move to the next game
        }
        else
        {
            // Load scene

        }
    }
     
    IEnumerator EnableThankYouCanvas(string experienceId, string schoolId, string sectionID, string gradeID)
    {

        DeviceSynHandler.isGameOver = true;
        thankyouCanvas.SetActive(true);
        StudentScore student = new StudentScore()
        {
            studentID = SystemInfo.deviceUniqueIdentifier,//deviceid
            experienceConductedID = experienceId,
            schoolId = schoolId,
            sectionID = sectionID,
            gradeID = gradeID,
            score = arrowScore + basketBallScore + mcqScore,
            // optional, defaults to 60
        };
        string jsonData = JsonUtility.ToJson(student);

        StartCoroutine(PostRequest("https://44.200.7.3/v1/performance", jsonData));
        PlayerPrefs.SetString("PlayerStatus", "Completed");
        yield return new WaitForSeconds(3.5f);
      
      
    }
    IEnumerator PostRequest(string api, string jsonData)
    {

        var req = new UnityWebRequest(api, "POST");
        byte[] jsonToSend = new System.Text.UTF8Encoding().GetBytes(jsonData);
        req.uploadHandler = (UploadHandler)new UploadHandlerRaw(jsonToSend);

        req.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
        req.SetRequestHeader("Content-Type", "application/json");
        req.SetRequestHeader("device-id", SystemInfo.deviceUniqueIdentifier);
        req.certificateHandler = new BypassCertificateHandler();
        //Send the request then wait here until it returns
        yield return req.SendWebRequest();
        if (req.isNetworkError) // error in request
        {
            Debug.Log("Error While Sending: " + req.error);
        }
        else // done
        {

            Debug.Log("return" + req.downloadHandler.text);
            string value = req.downloadHandler.text;

        }

    }
    public void   CallingThankYouCanvas()
    {
        foreach (GameObject item in disableObjects)
        {
            item.SetActive(false);
        }
        classRoom.SetActive(true);
        ActiveDeviceStatus();
        StartCoroutine(EnableThankYouCanvas(exid, sid, sectionId, gradeId));
    }
   

    private string exid;
    private string sid;
    private string sectionId;
    private string gradeId;
    public void StudentPerformance(string experianceID,string schoolId,string sectId,string gradId)
    {
        exid = experianceID;
        sid=schoolId;
        sectionId = sectId;
        gradeId = gradId;
    }

    public void OnGameCompleted()
    {
        // Disable the completed game
        switch (currentGameName)
        {
            case "Archery":
                arrowGame.SetActive(false);
                break;
            case "Basketball":
                basketballGame.SetActive(false);
                break;
            case "MCQ":
                mcqGame.SetActive(false);
                break;
            default:
                Debug.LogWarning("Unknown game name: " + currentGameName);
                break;
        }
        gameCount++;
        if (gameCount < typeOfGame.Count)
        {
            EnableNextGame(typeOfGame[gameCount]);
        }
        else
        {
            CallingThankYouCanvas();
           //StartCoroutine(EnableThankYouCanvas());
        }


    }
    public void CallingThankyouOutSide()
    {
        CallingThankYouCanvas();
    }
    public void AssignGames(List<string> selectedgames)
    {
        this.selectedGames = selectedgames;
    }
    // Method to enable selected games based on data from API
    public void ActiveDeviceStatus()
    {

        DeviceStatus status = new DeviceStatus
        {
           
            
            isCompleted = true
        };
        string jsonData = JsonUtility.ToJson(status);
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
        request.SetRequestHeader("device-id", SystemInfo.deviceUniqueIdentifier);
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
          
        }

        
       
    }
}
