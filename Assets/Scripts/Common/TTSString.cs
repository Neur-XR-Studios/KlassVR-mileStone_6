using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public class TTSString : MonoBehaviour
{
    // Start is called before the first frame update
    public GoogleTextToSpeech googleTextToSpeech;
    public  int Delay;
    private string text;
    void Start()
    {
        googleTextToSpeech= FindAnyObjectByType<GoogleTextToSpeech>();
        


    }

   public void StartTalkingAfterDelay(string EneterYourText,int delay)
    {
        Delay = delay;
        text = EneterYourText;
        StartCoroutine(StartTalking());

      
    }
   IEnumerator StartTalking()
    {
      yield return new WaitForSeconds(Delay); 
      googleTextToSpeech.StartTalking(text);
    }
}
