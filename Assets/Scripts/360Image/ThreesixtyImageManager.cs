using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ThreesixtyImageManager : MonoBehaviour
{
    public AddingImage addingImage;
    public GameObject imge;
    public Transform targetposition;
    public GameObject[] enableToDisable;
    public GameObject[] DisableElemet;
    private WebManager webmanager;
    private API api;

    private GoogleTextToSpeech googleTextToSpeech;
    private GamificationManager gamificationManager;
    private bool isModelSkip;
    private ServiceScript serviceScript;
    public UnityEvent startingEvent;
    public string stop;

    // Start is called before the first frame update
    void Start()
    {
        gamificationManager = FindObjectOfType<GamificationManager>();
        serviceScript = FindObjectOfType<ServiceScript>();
        googleTextToSpeech = FindAnyObjectByType<GoogleTextToSpeech>();
        webmanager = FindAnyObjectByType<WebManager>();
        api = FindObjectOfType<API>();

    }
    public void ModelStatus(bool isModel)
    {
        isModelSkip = isModel;
    }

    public void ThreeSixtyExperianceStarted()
    {
      
        if (!api.is360Image)
        {
            // gamificationManager.AssignGames();
            //webmanager.ActivateWebView();
            ElementToDisable();
            Performe();
        }
        else
        {

            startingEvent.Invoke();
            addingImage.AssignSkyBox();
            ElementToDisable();
            ToggleElement(false);
            // imge.transform.localPosition = targetposition.localPosition;
            StartCoroutine(ThreeSixtyExperiance());
        }




    }
    private void Performe()
    {
        googleTextToSpeech.StopAudio();
        if (!isModelSkip)
        {
            serviceScript.AssignTaskBasedOnCondition(GameStatus.SkipModel);
        }
        else
        {
            serviceScript.AssignTaskBasedOnCondition(GameStatus.WithoutSkipModel);
            serviceScript.AssignTaskBasedOnCondition(GameStatus.VideoEndFirst);

        }

       
        // Action.Invoke();
    }
    public void ElementToDisable()
    {
        foreach (GameObject element in DisableElemet)
        {
            element.SetActive(false);
        }
    }
    IEnumerator ThreeSixtyExperiance()
    {

        if (!string.IsNullOrEmpty(addingImage.imageScript))
        {
            googleTextToSpeech.StartTalking(addingImage.imageScript);
        }

      yield return new WaitForSeconds(addingImage.imageTimer);
       
        imge.SetActive(false);
        ToggleElement(true);
        //webmanager.ActivateWebView();
        Performe();
        //  gamificationManager.AssignGames();
    }
    public void ToggleElement(bool isTrue)
    {
        foreach (GameObject item in enableToDisable)
        {
            item.SetActive(isTrue);
        }
    }
}
