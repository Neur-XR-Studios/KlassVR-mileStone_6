using Google.Apis.Services;
using Google.Apis.YouTube.v3;
using RenderHeads.Media.AVProVideo;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;



public class YouTubePlayer : MonoBehaviour
{
    public MediaPlayer mediaPlayer;
    private string videoPlayerURL;
    private bool videoOpened = false;
    public string youtubeApiKey;
    public string youtubeVideoUrl;
    public string youtubeVideoId;

    private void Start()
    {
        youtubeVideoId = "Q3pO8W67LXY";
        FetchVideoData(youtubeVideoId);
    }

    private void FetchVideoData(string videoId)
    {
        YouTubeService youtubeService = new YouTubeService(new BaseClientService.Initializer()
        {
            ApiKey = youtubeApiKey
        });

        var request = youtubeService.Videos.List("snippet,contentDetails");
        request.Id = videoId;

        var response = request.Execute();

        if (response.Items.Count > 0)
        {
            string videoTitle = response.Items[0].Snippet.Title;
            string videoDuration = response.Items[0].ContentDetails.Duration;

            string videoUrl = ConstructVideoUrl(videoId);

            PlayVideo(videoUrl);
        }
        else
        {
            Debug.LogError("Video not found!");
        }
    }

    private string ConstructVideoUrl(string videoId)
    {
        // Construct the direct media URL using the video ID
        return "https://www.youtube.com/watch?v=" + videoId;
    }

    private void PlayVideo(string videoUrl)
    {
        bool isOpening = mediaPlayer.OpenMedia(new MediaPath(videoUrl, MediaPathType.AbsolutePathOrURL), autoPlay: true);
        videoOpened = isOpening;

    }
}