  using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Another : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
      //  EventManager.AddHandler(GameEvent.OnPlayerLanded, Onlanded);

    }

    private void Onlanded()
    {
        Debug.Log("hai");
       // EventManager.RemoveHandler(GameEvent.OnPlayerLanded, Onlanded);
    }

    // Update is called once per frame
    void Update()
    {

    }
}
