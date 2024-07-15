using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Xml.Linq;
using UnityEngine;
using XR.Interaction.PoseDetection.Debug;
public class  GenericList<T>
{
    private T data;

    public T GetData()
    { return data; }
    public void SetData(T data) {  this.data = data; }
}
public class YoutubestatusPodt: YoutubeTestManager
{

}
public class YoutubeTestManager : MonoBehaviour

{
    [SerializeField]
    private GameObject youtubeTest;
    private string youtubeUrl;
    private API api;
    // Start is called before the first frame update
    public string YoutubeUrl
    {
        set { youtubeUrl = value; }
        get { return youtubeUrl; }
    }

    void Start()
    {
        api = FindAnyObjectByType<API>();
    }
    public void CheckVideostatus()
    {
        Type type = typeof(API);
        Debug.Log($"Type Name: {type.Name}");
        PropertyInfo GeneratedUrl = type.GetProperty("YouTubeUrlTest");
        if (GeneratedUrl != null)
        {
            YoutubeUrl=(string)GeneratedUrl.GetValue(api);
        }
        youtubeTest.SetActive(true);

    }
}
       

       
