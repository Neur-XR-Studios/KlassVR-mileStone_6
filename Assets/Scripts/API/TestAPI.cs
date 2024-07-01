using SimpleJSON;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestAPI : MonoBehaviour
{
    public string json;
    private API api;  

    private void Start()
    {
        api = gameObject.GetComponent<API>();
        ParseData();
       
    }
    public void ParseData()
    {


       
        var spaceData = JSONNode.Parse(json);
      //  api.DummyTest(json);
        Debug.Log("hi");
    }
}
