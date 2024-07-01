using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Inputs;
using UnityEngine.UI;
using System.Net;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Rendering;
using TMPro;

public class BallSpawner : MonoBehaviour
{
    public GameObject ballPrefab;
    public GameObject ArrowPrefab;
    public Transform spawnPoint_AType, spawnPoint_BType;
    public InputActionReference triggerAction;
    public bool TriggerEnter = false;
    public GamificationController answerCheckingScript;
    public List<GameObject> DisableOption;


    public GameObject BasketBallObject;
    public GameObject ArcheryObject;
    public Transform RightController;

    public AudioSource BasketBallAudioClip;
    public AudioSource ArrowAudioClip;

    public ButtonColorChanger ButtonColorChangerScript;
    public UnityEvent buttonColorChangerEvent;
    public Quaternion newRotation;
    public GameObject Startpoint, StartPointParent;
    public bool isSpawn;
    public bool isSpawnobj;
    public static bool isClicked;

    public bool optionSelectionbool;

    public SpawnObject spawn;
    public TextMeshProUGUI errorText;
    public static bool isGamefication;
    private GamificationManager gamificationManager;
    private void Start()
    {
        gamificationManager=FindObjectOfType<GamificationManager>();
        DisableOption = new List<GameObject>();
        isSpawn = true;
        isSpawnobj = true;
        RightController = GameObject.Find("RighthandNew").transform;
        isGamefication=true;
        if (GameObject.Find("pointer")!=null)
        {
            answerCheckingScript = GameObject.Find("pointer").GetComponent<GamificationController>();
           // answerCheckingScript.ResetPosition();
        }
        else
        {
            errorText.text = "pointer not found";
        }
      

        /* DisableOptionA = GameObject.FindWithTag("OptionA");
         DisableOptionB = GameObject.FindWithTag("OptionB");
         DisableOptionC = GameObject.FindWithTag("OptionC");
         DisableOptionD = GameObject.FindWithTag("OptionD");*/
      
        Startpoint = GameObject.Find("StartPoint");
        StartPointParent = GameObject.Find("RighthandNew");
        GameObject test = GameObject.FindWithTag("Option" + 1.ToString());
        AssignColliderToArray();
    }
    public void AssignColliderToArray()
    {
        for (int i = 1; i <= 4; i++)
        {

            DisableOption.Add(GameObject.FindWithTag("Option" + i.ToString()));
        }

    }
   
    public void DisableBoxCollider(bool isValue, int option)
    {
        foreach (GameObject obj in DisableOption)
        {
            // Perform operations on each GameObject

            obj.GetComponent<BoxCollider>().enabled = isValue;
        }
        switch (option)
        {
            case 0:
                DisableOption[option].GetComponent<BoxCollider>().enabled = true;
                break;
            case 1:
                DisableOption[option].GetComponent<BoxCollider>().enabled = true;
                break;
            case 2:
                DisableOption[option].GetComponent<BoxCollider>().enabled = true;
                break;
            case 3:
                DisableOption[option].GetComponent<BoxCollider>().enabled = true;
                break;

            default:
                Debug.Log("Invalid day!");
                break;
        }
    }

 /*   public void SpawnPositionManager()
    {
        isGamefication=false;
        if (answerCheckingScript.isOptionA)
        {
            TriggerEnter = true;
            UnChildWithParent();
            DisableNextButton();
            SpawnBall(answerCheckingScript.isOptionA,"a");
            DisableBoxCollider(false, 0);
        }
        if (answerCheckingScript.isOptionB)
        {
            TriggerEnter = true;
            UnChildWithParent();
            DisableNextButton();
            SpawnBall(answerCheckingScript.isOptionB, "b");
            DisableBoxCollider(false, 1);
        }
        if (answerCheckingScript.isOptionC)
        {
            TriggerEnter = true;
            UnChildWithParent();
            DisableNextButton();
            SpawnBall(answerCheckingScript.isOptionC, "c");
            DisableBoxCollider(false, 2);
        }
        if (answerCheckingScript.isOptionD)
        {
            TriggerEnter = true;
            UnChildWithParent();
            DisableNextButton();
            SpawnBall(answerCheckingScript.isOptionD, "d");
            DisableBoxCollider(false, 3);
        }
    }*/

    public void DisableNextButton()
    {
        GameObject NextButton = GameObject.Find("NextButtonn");
        NextButton.GetComponent<Button>().enabled = false;
    }
    private void SpawnBall(bool isClicked,string value)
    {
        if (gamificationManager.currentGameName== "Archery")
        {
             BasketBallAudioClip.Play();
            GameObject newBall = Instantiate(ballPrefab, spawnPoint_BType.position, Quaternion.identity);
            isSpawnobj = false;
            spawn = FindAnyObjectByType<SpawnObject>();
            spawn.BasketballPosition(isClicked, value);
        }
        if (gamificationManager.currentGameName == "Basketball")
        {
            ArrowAudioClip.Play();
            GameObject newBall = Instantiate(ArrowPrefab, spawnPoint_AType.position, Quaternion.identity);
            isSpawnobj = false;
            spawn = FindAnyObjectByType<SpawnObject>();
            //spawn.ArcheryPosition(isClicked, value);
        }
       
    }

    public void GamificationTypeSelection()
    {
        if (gamificationManager.currentGameName == "Basketball")
        {
            BasketBallObject.SetActive(true);
            ArcheryObject.SetActive(false);
        }
        if (gamificationManager.currentGameName == "Archery")
        {
            ArcheryObject.SetActive(true);
            BasketBallObject.SetActive(false);
        }
    }

    public void UnChildWithParent()
    {
        Startpoint.transform.SetParent(null);
    }

    public void ChildWithParent()
    {
        Startpoint.transform.SetParent(StartPointParent.transform);
    }

    public void EnableSpawn()
    {
        isSpawn = true;
    }

    public void EnableSpawnObj()
    {
        isSpawnobj = true;
    }
}
