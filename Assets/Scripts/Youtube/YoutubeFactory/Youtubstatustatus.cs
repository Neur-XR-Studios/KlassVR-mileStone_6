using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Youtubstatustatus : MonoBehaviour,ISession
{
    public GameObject youtubeVideo;
    private bool hasYoutubeError;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void StartSession()
    {
        youtubeVideo.SetActive(true);
    }
   
    public bool hasError()
    {
        return true;
    }
}
