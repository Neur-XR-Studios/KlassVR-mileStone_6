using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class VideoEndAction : MonoBehaviour
{
    public UnityEvent onVideoDisable;
    private void OnDisable()
    {
       //  Debug.Log("disabled");
        onVideoDisable.Invoke();
    }
}
