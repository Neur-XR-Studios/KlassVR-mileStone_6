using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BroadcastEvent : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {

        Invoke("Late", 3f);
    }
    public void Late()
    {
//EventManager.Broadcast(GameEvent.OnPlayerLanded);
    }
    // Update is called once per frame
    void Update()
    {

    }
}
