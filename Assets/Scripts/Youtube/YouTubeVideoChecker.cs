using UnityEngine;
using UnityEngine.Networking;
using System;
using System.Text.RegularExpressions;
using System.Collections;

public class YouTubeVideoChecker : MonoBehaviour
{
    public string youtubeUrl = "https://youtu.be/SbvhoOvtMgM?si=mOQK5GVSXRALicDv";
    public string apiKey = "AIzaSyBIk-Kj2xLZDeE92q7APc1FPn-rlqjMALA";

    void Start()
    {
        StartCoroutine(CheckVideoType());
    }

    IEnumerator CheckVideoType()
    {
        string videoId = GetVideoIdFromUrl(youtubeUrl);
        if (string.IsNullOrEmpty(videoId))
        {
            Debug.LogError("Invalid YouTube URL");
            yield break;
        }

        string apiUrl = $"https://www.googleapis.com/youtube/v3/videos?id={videoId}&part=snippet,contentDetails&key={apiKey}";
        using (UnityWebRequest webRequest = UnityWebRequest.Get(apiUrl))
        {
            yield return webRequest.SendWebRequest();

            if (webRequest.result == UnityWebRequest.Result.ConnectionError || webRequest.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError("Error: " + webRequest.error);
            }
            else
            {
                string jsonResponse = webRequest.downloadHandler.text;
                YouTubeVideoInfo videoInfo = JsonUtility.FromJson<YouTubeVideoInfo>(jsonResponse);
                if (videoInfo != null && videoInfo.items != null && videoInfo.items.Length > 0)
                {
                    string dimension = videoInfo.items[0].contentDetails.dimension;
                    string projection = videoInfo.items[0].contentDetails.projection; // Check projection type if available

                    if (!string.IsNullOrEmpty(dimension))
                    {
                        if (dimension == "2d")
                        {
                            Debug.Log("Video Type: Monoscopic (2D)");
                        }
                        else if (dimension == "3d")
                        {
                            if (!string.IsNullOrEmpty(projection) && projection == "360")
                            {
                                Debug.Log("Video Type: Stereoscopic 360-degree (3D)");
                            }
                            else
                            {
                                Debug.Log("Video Type: Stereoscopic (3D)");
                            }
                        }
                        else
                        {
                            Debug.Log("Video Type: Unknown dimension");
                        }
                    }
                    else
                    {
                        Debug.LogWarning("Video type not specified.");
                    }
                }
                else
                {
                    Debug.LogWarning("Video not found.");
                }
            }
        }
    }

    public string GetVideoIdFromUrl(string url)
    {
        string videoId = "";
        try
        {
            var regex = new Regex(@"(?:youtube\.com\/(?:[^\/\n\s]+\/\S+\/|(?:v|e(?:mbed)?)\/|\S*?[?&]v=)|youtu\.be\/)([a-zA-Z0-9_-]{11})", RegexOptions.IgnoreCase);
            var match = regex.Match(url);
            if (match.Success)
            {
                videoId = match.Groups[1].Value;
            }
        }
        catch (Exception ex)
        {
            Debug.LogError("Error extracting video ID: " + ex.Message);
        }
        return videoId;
    }

    [Serializable]
    public class YouTubeVideoInfo
    {
        public YouTubeVideoItem[] items;
    }

    [Serializable]
    public class YouTubeVideoItem
    {
        public YouTubeVideoContentDetails contentDetails;
    }

    [Serializable]
    public class YouTubeVideoContentDetails
    {
        public string dimension;
        public string projection; // Add projection type if available
    }
}