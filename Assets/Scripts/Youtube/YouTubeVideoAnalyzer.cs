using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using Newtonsoft.Json.Linq;

public class YouTubeVideoAnalyzer : MonoBehaviour
{
    public string videoUrl = "https://www.youtube.com/watch?v=p0hAcQTv21o";
   // private string apiKey = "AIzaSyDX9tewMp8PV3ITfnzVC9uCOtzOm-GLZng";
    private string apiKey = "AIzaSyBIk-Kj2xLZDeE92q7APc1FPn-rlqjMALA";

        void Start()
        {
            string videoId = ExtractVideoId(videoUrl);
            Debug.Log("Extracted Video ID: " + videoId);
            StartCoroutine(GetVideoDetails(videoId));
        }

        private string ExtractVideoId(string url)
        {
            var uri = new System.Uri(url);
            var query = System.Web.HttpUtility.ParseQueryString(uri.Query);
            return query["v"];
        }

        IEnumerator GetVideoDetails(string videoId)
        {
            string apiUrl = $"https://www.googleapis.com/youtube/v3/videos?id={videoId}&key={apiKey}&part=snippet,contentDetails";
            Debug.Log("API URL: " + apiUrl);

            UnityWebRequest request = UnityWebRequest.Get(apiUrl);
            yield return request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("Error fetching video details: " + request.error);
                yield break;
            }

            string jsonResponse = request.downloadHandler.text;
            Debug.Log("API Response: " + jsonResponse);

            JObject videoData = JObject.Parse(jsonResponse);
            AnalyzeVideoData(videoData);
        }

        private void AnalyzeVideoData(JObject videoData)
        {
            JToken items = videoData["items"];
            if (items == null || !items.HasValues)
            {
                Debug.LogError("No video details found.");
                return;
            }

            JObject snippet = (JObject)items[0]["snippet"];
            string title = snippet["title"]?.ToString() ?? "No Title";
            string description = snippet["description"]?.ToString() ?? "No Description";
            JArray tags = (JArray)snippet["tags"];

            Debug.Log("Title: " + title);
            Debug.Log("Description: " + description);

            bool isStereo = false;

            // Check contentDetails for projection type
            JObject contentDetails = (JObject)items[0]["contentDetails"];
            string projection = contentDetails["projection"]?.ToString();
            Debug.Log("Projection: " + projection);

            if (projection == "360" || projection == "spherical")
            {
                isStereo = true;  // Assuming that '360' or 'spherical' indicates stereoscopic content
            }
            else if (tags != null)
            {
                Debug.Log("Tags found: " + string.Join(", ", tags));
                foreach (var tag in tags)
                {
                    string tagString = tag.ToString().ToLower();
                    Debug.Log("Checking tag: " + tagString);

                    if (tagString.Contains("3d") ||
                        tagString.Contains("vr") ||
                        tagString.Contains("sbs") ||
                        tagString.Contains("ou") ||
                        tagString.Contains("stereo") ||
                        tagString.Contains("side by side") ||
                        tagString.Contains("over under") ||
                        tagString.Contains("vr 360"))
                    {
                        isStereo = true;
                        break;
                    }
                }
            }
            else
            {
                Debug.LogWarning("No tags found for video.");
            }

            if (isStereo)
            {
                Debug.Log("Video is stereoscopic.");
               
            }
            else
            {
                Debug.Log("Video is monoscopic.");
               
            }
        }

       
    }
