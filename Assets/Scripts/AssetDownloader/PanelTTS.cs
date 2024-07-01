using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PanelTTS : MonoBehaviour
{
    private GoogleTextToSpeech googleTextToSpeech;
    private string annotationText;
    public TextMeshProUGUI boardtext;
    private void Start()
    {
        googleTextToSpeech=FindAnyObjectByType<GoogleTextToSpeech>();
    }
    public void TTS()
    {
        annotationText=boardtext.text;
        if(annotationText!=null) 
        googleTextToSpeech.StartTalking(annotationText);
    }
  /*  private void Update()
    {
        if (Input.GetKeyDown("space"))
        {
            TTS();
        }
    }*/
}
