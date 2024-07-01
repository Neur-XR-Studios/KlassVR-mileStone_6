using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class AddingImage : MonoBehaviour
{
    public string imageUrl = "URL_TO_YOUR_IMAGE"; // URL to your 360 image
    public Material sphereMaterial;
    public Material skybox;
    public string imagePath = "Assets/Scripts/360Image/kris-guico-rsB-he-ye7w-unsplash.jpg";
    public string imageScript;
    private string newUrl;
    public GameObject VideoSphere;
    public float imageTimer;
    // Start is called before the first frame update
    void Start()
    {

        /*   Renderer renderer = GetComponent<Renderer>();
           if (renderer != null)
           {
               sphereMaterial = renderer.material;
               LoadImageFromFile();
           }*/
    }


    public void LoadAndAssignImage(string url, string script,string imageTime)
    {
        //  
        if (!string.IsNullOrEmpty(imageTime))
        {
            imageTimer = ConvertTimeToSeconds(imageTime);
        }
        else
        {
            imageTimer = 10;
        }
      
        newUrl = url;
        imageScript = script;
        Renderer renderer = GetComponent<Renderer>();
        if (renderer != null)
        {
           //sphereMaterial = renderer.material;
            //StartCoroutine(LoadImage(url));
        }
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

    public void AssignSkyBox()
    {
        VideoSphere.SetActive(false);
        RenderSettings.skybox = skybox;
        StartCoroutine(LoadImageNew(newUrl));
    }

    IEnumerator LoadImageNew(string url)
    {
        using (WWW www = new WWW(url))
        {
            yield return www;
            if (string.IsNullOrEmpty(www.error))
            {
                Texture2D texture = new Texture2D(2, 2, TextureFormat.ASTC_8x8, false);
                www.LoadImageIntoTexture(texture);
                skybox.mainTexture = texture;
            }
            else
            {
                Debug.LogError("Error loading image: " + www.error);
            }
        }
    }
    IEnumerator LoadImage(string url)
    {
        using (WWW www = new WWW(url))
        {
            yield return www;
            if (string.IsNullOrEmpty(www.error))
            {
                Texture2D texture = new Texture2D(2, 2, TextureFormat.ASTC_8x8, false);
                www.LoadImageIntoTexture(texture);
                sphereMaterial.mainTexture = texture;
            }
            else
            {
                Debug.LogError("Error loading image: " + www.error);
            }
        }
    }

}
