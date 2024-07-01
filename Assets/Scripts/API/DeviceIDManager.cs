using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using SimpleJSON;

public class DevideIDRegistraction
{
    public string deviceID;
    public string schoolID;

  
}

public class DeviceIDManager : MonoBehaviour
{
    [System.Serializable]
    public class ApiResponse
    {
        public int code;
        public string message;
        // Include other fields as necessary
    }
    public API apiManager;
    private int oneTimeConfig;
    public GameObject[] toggleItem;
    public TextMeshProUGUI inputText;
    private string deviceID;
    public GameObject wrongPanel;
    public GameObject truePanel;
    private DevideIDRegistraction registration;
    void Awake()
    {
        //Uncomment
        // device Sync handler.cs(203,126)  api.cs(94)
      deviceID = SystemInfo.deviceUniqueIdentifier;
     // deviceID = "aj4y";
        registration = new DevideIDRegistraction();
       
       

    }

   public void OneTimeDeviceConfiguration()
    {
            ToggleFunction(false);
      
    }
    IEnumerator WebPostResponse(string api, string jsonData)
    {

        var req = new UnityWebRequest(api, "POST");    //UnityWebRequest handles the flow of HTTP communication with web servers.
        byte[] jsonToSend = new System.Text.UTF8Encoding().GetBytes(jsonData); //To calculate the exact size required by GetBytes to store the resulting bytes
        req.uploadHandler = (UploadHandler)new UploadHandlerRaw(jsonToSend); //This subclass copies input data into a native-code memory buffer at construction time,
                                                                             //and transmits that data verbatim as HTTP request body data.
        req.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
        req.SetRequestHeader("Content-Type", "application/json");
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
            string  Responses = req.downloadHandler.text;
            ApiResponse response = JsonUtility.FromJson<ApiResponse>(Responses);
            if (response.message == "Device created successfully")
            {
                truePanel.SetActive(true);
               // ToggleFunction(true);
               //  apiManager.CallingClassVrExperiance();
                gameObject.SetActive(false);
            }
            else
            {
                inputText.text = "";
                wrongPanel.SetActive(true);
            }
               

           
        }
       
    
    }
  
    public void ToggleFunction(bool value)
    {
        foreach (var item in toggleItem)
        {
            item.SetActive(value);
        }
    }
    public void OnButtonClick(string value)
    {
        inputText.text += value;
    }

    public void OnBackspaceClick()
    {
        if (inputText.text.Length > 0)
        {
            inputText.text = inputText.text.Substring(0, inputText.text.Length - 1);
        }
    }

    public void OnSubmitClick()
    {
     
        registration.deviceID = deviceID;
        registration.schoolID = inputText.text;
        string mydate = JsonUtility.ToJson(registration);
      StartCoroutine(WebPostResponse("https://44.200.7.3/v1/devices", mydate));
      //  StartCoroutine(WebPostResponse("http://192.168.0.251:3000/v1/devices", mydate));
    /*    ToggleFunction(true);
        gameObject.SetActive(false);*/

    }
    public class BypassCertificateHandler : CertificateHandler
    {
        protected override bool ValidateCertificate(byte[] certificateData)
        {
            // Always accept
            return true;
        }
    }
}

