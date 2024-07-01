using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class Ontrigger : MonoBehaviour
{
   
    private GamificationController optionSelection;
    public Button answerButtons;
    public Transform destination;
    public Transform staringPoint;
    public Transform staringPointArrow;
    public Transform fallpoint;
    private bool isBasketBall;
    public bool isEntered;
    private GamificationController gamificationController;
    public InputActionReference inputActionController;
    [Header("Basket Ball details")]
    public AudioSource BasketBallAudioClip;
    public GameObject basketBall;
    public GameObject ballVisualize;
    public GameObject ballVisualizeChild;
    [Header("Arrow Ball details")]
    public AudioSource ArrowAudioClip;
    public GameObject arrow;
    public GameObject arrowVisualize;
    private  GamificationManager  gamificationManager;
   



    private void Start()
    {
        gamificationManager=FindObjectOfType<GamificationManager>();
        gamificationController =FindAnyObjectByType<GamificationController>();
         optionSelection =FindObjectOfType<GamificationController>();
        //staringPoint = gameObject.transform;
        GamificationController.isGamefication=true;
    }

   
    private void OnTriggerStay(Collider other)
    {
      
        if (other.gameObject.CompareTag("Reticle"))
        {
          isEntered = true;
            if (GamificationController.isGamefication)
            {
              

                 if (Input.GetKeyDown(KeyCode.Space))  // play mode
                 {
                    
                      GamificationController.isGamefication  = false;
                    StartSpawning();
                    //ballVisualize.GetComponent<MeshRenderer>().enabled = false;
                    //ballVisualizeChild.GetComponent<MeshRenderer>().enabled = false;
                    //arrowVisualize.GetComponent<MeshRenderer>().enabled = false;
                }
                if (inputActionController.action.triggered) // build mode
                {
                    GamificationController.isGamefication = false;
                    StartSpawning();
                    //ballVisualize.GetComponent<MeshRenderer>().enabled = false;
                    //ballVisualizeChild.GetComponent<MeshRenderer>().enabled = false;
                    //arrowVisualize.GetComponent<MeshRenderer>().enabled = false;
                }
               
            }
          
           
        }
        if (other.gameObject.CompareTag("BasketBall"))
        {
            Button temp = answerButtons.GetComponent<Button>();
            temp.onClick.Invoke();
        }
        if (other.gameObject.CompareTag("arrow"))
        {
            Button temp = answerButtons.GetComponent<Button>();
            temp.onClick.Invoke();
        }






    }
    
    private void OnTriggerExit(Collider other)
    {
        isEntered=false;
    }
    public void StartSpawning()
    {
        gamificationController.UnchildStartPoint();
        SpawnBall();
    }
    private void Update()
    {
       

    }
  
    private void SpawnBall()
    {
          if (gamificationManager.currentGameName == "Basketball")
        {
            BasketBallAudioClip.Play();
            GameObject newBall = Instantiate(basketBall, destination.position, Quaternion.identity);
            SpawnObject spawn = FindAnyObjectByType<SpawnObject>();
            spawn.SetStartPointAndDestination(fallpoint, optionSelection.isBasketBall, staringPoint,destination);
        }
        if (gamificationManager.currentGameName == "Archery")
        {
            ArrowAudioClip.Play();
            destination= staringPointArrow;
            GameObject newBall = Instantiate(arrow,  arrowVisualize.transform.position, arrowVisualize.transform.rotation);
            SpawnObject spawn = FindAnyObjectByType<SpawnObject>();
            spawn.SetStartPointAndDestination(fallpoint, optionSelection.isBasketBall, staringPoint, destination);
        }

    }

}
