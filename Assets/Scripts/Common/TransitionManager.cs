using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TransitionManager : MonoBehaviour
{
    public Text videoTime;
    public GameObject transitionPanel;
    private bool isVideoEnd;
    // Start is called before the first frame update
    void Start()
    {
        isVideoEnd = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (!(videoTime.text == "00:00") && isVideoEnd)
        {
            transitionPanel.SetActive(false);
            Debug.Log("Timerr");
            isVideoEnd=false;
        }
    }
}
