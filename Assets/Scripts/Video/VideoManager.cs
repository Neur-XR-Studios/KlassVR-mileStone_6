using RenderHeads.Media.AVProVideo;
using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using UnityEngine.Video;
using LightShaft.Scripts;

public class VideoManager : MonoBehaviour
{
   // public VideoPlayer videoPlayer;
    public MediaPlayer mediaPlayer;
    private string videoPlayerURL;
    // public UnityEvent Action;
    private GoogleTextToSpeech googleTextToSpeech;
    //  public UnityEvent VideoEndAction;
    private float duration;
    public StereoPacking stereoPackingMode = StereoPacking.TopBottom;
    private ServiceScript service;
    private string videoScript;
    private bool videoOpened = false;
    private bool isModelSkip;
    private string url;
   
    private float startTimer;
    private float endTimer;
    public GameObject youtubePlayer;
    public YoutubePlayer ytbPlayer;
    private string youtubeURL;
    private bool isYouTubeVideoAudio;
    private VideoPlayer videoPlayer;
   private string YoutubeScript;
    public AudioSource YoutubeAudioSource;
    public GameObject avpro;
    private bool isThreeSixty;
    private Coroutine videoCoroutine;
    private void Start()
    {
        googleTextToSpeech = FindAnyObjectByType<GoogleTextToSpeech>();
        service = FindObjectOfType<ServiceScript>();

    }
    public void AssignYoutubeProperty(string youTubeStartTimer, string youTubeEndTimer,bool youTubeVideoAudio,string youtubeUrl,string script)
    {
        YoutubeScript = script;
        isYouTubeVideoAudio = youTubeVideoAudio;
        youtubeURL = youtubeUrl;

        if (!string.IsNullOrEmpty(youTubeStartTimer))
        {
            // Split the start timer string into minutes and seconds
            string[] startTimeParts = youTubeStartTimer.Split(':');
            int startMinutes = int.Parse(startTimeParts[0]);
            int startSeconds = int.Parse(startTimeParts[1]);

            // Calculate total start time in seconds
            startTimer = startMinutes * 60 + startSeconds;
        }

        if (!string.IsNullOrEmpty(youTubeEndTimer))
        {
            // Split the end timer string into minutes and seconds
            string[] endTimeParts = youTubeEndTimer.Split(':');
            int endMinutes = int.Parse(endTimeParts[0]);
            int endSeconds = int.Parse(endTimeParts[1]);

            // Calculate total end time in seconds
            endTimer = endMinutes * 60 + endSeconds;
        }
    }
   
    public void MuteVolumn()
    {

        mediaPlayer.AudioVolume = 0;

    }
    public void ThreeSixtyImagestatus(bool value)
    {
        isThreeSixty = value;
    }
    public void ChangeVideoType(string videoType)
    {
        /*  if (videoType == "stereoscopic-side-to-side")
          {
              MediaHints hints = mediaPlayer.FallbackMediaHints;
              hints.stereoPacking = StereoPacking.LeftRight; // Set for left-right stereoscopic video
              mediaPlayer.FallbackMediaHints = hints;
          }
          else if (videoType == "stereoscopic-top-to-bottom")
          {
              MediaHints hints = mediaPlayer.FallbackMediaHints;
              hints.stereoPacking = stereoPackingMode;
              mediaPlayer.FallbackMediaHints = hints;
          }
          else
          {
              Debug.Log("Playing Properly");
          }*/
    }
   
    public void PlayVideo()
    {
        videoPlayerURL = PlayerPrefs.GetString(StaticStrings.videoUrl);
        if (!string.IsNullOrEmpty(youtubeURL))
        {
            avpro.SetActive(false);
            youtubePlayer.gameObject.SetActive(true);
            videoPlayer = youtubePlayer.GetComponent<VideoPlayer>();
           // ytbPlayer.startFromSecondTime = (int)startTimer; 
            videoPlayer.prepareCompleted += OnPrepareCompleted;
        }
        else
        {
          
            videoScript = PlayerPrefs.GetString(StaticStrings.videoScript);

            youtubePlayer.gameObject.SetActive(false);
            //comment it future
            bool isOpening = mediaPlayer.OpenMedia(new MediaPath(videoPlayerURL, MediaPathType.AbsolutePathOrURL), autoPlay: true);
            videoOpened = isOpening;

            //uncomment this line while imlimenting seek
            /* bool isOpening = mediaPlayer.OpenMedia(new MediaPath(url, MediaPathType.AbsolutePathOrURL), autoPlay: true);
             videoOpened = isOpening;*/
            StartCoroutine(OpenVideo());

            if (isOpening)
            {
                
            }
        }
        
      
    
    }
    IEnumerator OpenVideo()
    {
        yield return new WaitUntil(() => videoOpened);
        IsAudioNullInVideo();
    }
    public void IsAudioNullInVideo()
    {
        if (string.IsNullOrEmpty(videoScript))
        {
            //   GetMediaInfo();
            StartCoroutine(GetMediaInfo());

        }
        else
        {
           StartCoroutine(AssinOperation());
            MuteVolumn();


        }



    }
    public void Seek()
    {
        ytbPlayer.startFromSecondTime= (int)endTimer;
    }
    IEnumerator GetMediaInfo()
    {
        yield return new WaitUntil(() => mediaPlayer.Info.GetDuration() > 0);
        double durationMsDouble = mediaPlayer.Info.GetDuration();
        duration = (float)durationMsDouble;
        StartCoroutine(ActivateVideoEndAction());
    }


    private IEnumerator ActivateVideoEndAction()
    {
        yield return new WaitForSeconds(duration);
        if (!isThreeSixty)
        {
            service.AssignTaskBasedOnCondition(GameStatus.NoAudioInVideo);
        }
        else
        {
            service.AssignTaskBasedOnCondition(GameStatus.threesixtyImage);
        }


        // VideoEndAction.Invoke();
    }
    private void Performe()
    {
        if(videoCoroutine!=null)
        {
            StopCoroutine(videoCoroutine);
            videoCoroutine = null;
        }
     
        if (!isThreeSixty)
        {
            // service.AssignTaskBasedOnCondition(GameStatus.WithoutSkipModel);
            service.AssignTaskBasedOnCondition(GameStatus.threesixtyImage);
        }
        else
        {
            service.AssignTaskBasedOnCondition(GameStatus.threesixtyImage);
        }

       
        // Action.Invoke();
    }
   
    private IEnumerator AssinOperation()
    {
      
            googleTextToSpeech.StartTalking(videoScript);
            yield return new WaitUntil(() => googleTextToSpeech.IsAudioReady);
           videoCoroutine = StartCoroutine(IsVideoEndFirst());
            Invoke("Performe", googleTextToSpeech.audioClipLength);
        
     
    }
    IEnumerator IsVideoEndFirst()
    {
        yield return new WaitUntil(() => mediaPlayer.Info.GetDuration() > 0);
        double durationMsDouble = mediaPlayer.Info.GetDuration();
        duration = (float)durationMsDouble;
        yield return new WaitForSeconds(duration);
        if (!isThreeSixty)
        {
            service.AssignTaskBasedOnCondition(GameStatus.VideoEndFirst);
        }
        else
        {
            //   service.AssignTaskBasedOnCondition(GameStatus.SkipModel);
            service.AssignTaskBasedOnCondition(GameStatus.VideoEndFirst);
        }

    }
    void OnPrepareCompleted(VideoPlayer source)
    {
        
        if (isYouTubeVideoAudio)
        {
          
            YoutubeAudioSource.mute = true;
            googleTextToSpeech.StartTalking(YoutubeScript);
        }
        float length = endTimer - startTimer;
        StartCoroutine(StopVideoAfterDuration(youtubePlayer.GetComponent<VideoPlayer>(), length));
           
    }
   
   
    IEnumerator StopVideoAfterDuration(VideoPlayer videoPlayer, double duration)
    {
        yield return new WaitForSeconds((float)duration);

        // Pause or stop the video
        videoPlayer.Pause(); // or videoPlayer.Stop() if you want to stop it completely
        youtubePlayer.SetActive(false);
        Performe();
    }

  
}

   

