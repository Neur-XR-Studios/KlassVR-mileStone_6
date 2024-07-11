using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class GamificationController : MonoBehaviour
{
 
    public bool isBasketBall, isArchery;
    public static bool isGamefication;
    public Button[] nextButton;
    public GameObject startPoint;
    public GameObject startpoint_A;
    public GameObject rightHand;
    public Button[] answerButton;
    public  Button buttonBasketBall;
    public  Button buttonArrow;
    private AudioSource audioSource;
    public GameObject ballPrefab;
    public GameObject arrowPrefab;
    public GameObject arrowVisualize;
    private int gameNumber;
    private GamificationManager gamificationManager;
    public BoxCollider[] boxColliders;
    public bool arrowstatus;
   
    void Start()
    {
        gamificationManager=FindAnyObjectByType<GamificationManager>();
        gameNumber = 1;
        audioSource = gameObject.GetComponent<AudioSource>();

    }
    public void ToggleCollider(bool isTrue)
    {
        foreach (BoxCollider item in boxColliders)
        {
            item.enabled = isTrue;  
        }
        arrowstatus = !isTrue;

}
    public void ActivateArrowStatus()
    {
        ToggleCollider(false);
    }
    public void EnbleClick()
    {
        SetParent();
        //StartCoroutine(EnableNextButton());
    }
    public IEnumerator EnableNextButton()
    {
        yield return new WaitForSeconds(2f);

        nextButton[0].enabled=true;
        nextButton[1].enabled=true;

    }
    public void EnableGamification()
    {
        isGamefication=true;
    }
    public void GameController(int num)
    {
        gameNumber=num;
        if (num ==1)
        {
            isBasketBall = true;
            isArchery = false;
        }
        else
        {
            isBasketBall = false;
            isArchery = true;
        }

        
    }
    public void UnchildStartPoint()
      {
        if(gamificationManager.currentGameName == "Basketball")
        {
            ballPrefab.SetActive(false);
        }
        else
        {
            arrowPrefab.SetActive(false);
          //  arrowVisualize.SetActive(false);
        }
        StartCoroutine(ParentUnchildDelay());
     
    }
    private IEnumerator ParentUnchildDelay()
    {
        yield return new WaitForSeconds(.2f);
        startPoint.transform.SetParent(null);
        startpoint_A.transform.SetParent(null);
      //  yield return new WaitForSeconds(1f);
    }
    public void SetParent()
    {
        startPoint.transform.SetParent(rightHand.transform);
        startpoint_A.transform.SetParent(rightHand.transform);
    }
    public void EnableBasketBallButton()
    {
        audioSource.Play();
        buttonBasketBall.enabled=true;
      
    }
    public void EnableArrowButton()
    {
        audioSource.Play();
        buttonArrow.enabled = true;

      
    }
    // Update is called once per frame



}
