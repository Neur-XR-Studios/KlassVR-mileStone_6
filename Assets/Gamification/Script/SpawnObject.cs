using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class SpawnObject : MonoBehaviour
{
    private  Transform startPoint; 
    private Transform endPoint;

    public float duration_A = 2f; 
    public float duration_Ar = 1f; 
    private float elapsedTime_A = 0f;
    public float rotationSpeed_A = 5f;

    private Transform fallPoint;
    public float duration1_B = 2f; // Duration of movement from start to end
    public float duration2_B = 0.8f; // Duration of movement from end to fall point
    private float elapsedTime1_B = 0f;
    private float elapsedTime2_B = 0f;
    private  GamificationController optionselection;

    public bool colorchangingbool;
   // public ButtonColorChanger ButtonColorChangerScript;
    //public BallSpawner ballSpawner;
    public bool enableTravel;
    public bool isClicked;
    private TextMeshProUGUI errorText;
    private bool isBasketBall;
    private bool isStartGame;

    private GamificationManager gamificationManager;
    private void Awake()
    {
        
    }
    private void Start()
    {
        try
        {
           
          

            optionselection =FindAnyObjectByType<GamificationController>();
            gamificationManager=FindObjectOfType<GamificationManager>();


            colorchangingbool = false;
           
           
          
        }
        catch (Exception e)
        {
            errorText= GameObject.FindGameObjectWithTag("text").GetComponent<TextMeshProUGUI>();
            errorText.text = "An error occurred during Start: " + e.Message;
         
          
        }


    }
    public void SetStartPointAndDestination(Transform fallingPoint,bool isbasketBall,Transform startingPoint,Transform destination)
     {
        fallPoint = fallingPoint;
        isBasketBall=isbasketBall;
       
        startPoint = destination;
        endPoint = startingPoint;
        isStartGame = true;
    }
    void Update()
    {
        if(isStartGame)
        {
            if (gamificationManager.currentGameName == "Basketball")
            {
                BasketBallDuration();
            }
            else
            {
                ArcheryDuration();
            }
        }
      
           
        
       
    }

    public void BasketballPosition(bool Option,string value)
    {
           
        

    }
        


    public void BasketBallDuration()
    {
        if (elapsedTime1_B < duration1_B)
        {
            // Calculate t parameter for the Bezier curve based on elapsed time
            float t1 = elapsedTime1_B / duration1_B;

            // Calculate Bezier curve point using start and end points
            Vector3 curvePoint1 = CalculateBezierPoint_B(startPoint.position, endPoint.position, t1);

            // Move the object to the calculated curve point
            transform.position = curvePoint1;

            // Increment elapsed time for the first segment
            elapsedTime1_B += Time.deltaTime;
        }
        else if (elapsedTime2_B < duration2_B)
        {
            // Calculate t parameter for the linear interpolation based on elapsed time
            float t2 = elapsedTime2_B / duration2_B;

            // Linearly interpolate between the end point and fall point
            Vector3 curvePoint2 = Vector3.Lerp(endPoint.position, fallPoint.position, t2);

            // Move the object to the calculated curve point
            transform.position = curvePoint2;

            // Increment elapsed time for the second segment
            elapsedTime2_B += Time.deltaTime;
        }
        else
        {
            // If both durations are exceeded, stop updating the position
            transform.position = fallPoint.position;
        }
    }

    public void ArcheryDuration()
    {
           if (elapsedTime_A < 1f)
        {
            // Calculate t parameter for the Bezier curve based on elapsed time
            float t1 = elapsedTime1_B / 1f;

            // Calculate Bezier curve point using start and end points
            Vector3 curvePoint1 = CalculateBezierPoint_B(startPoint.position, endPoint.position, t1);

            // Move the object to the calculated curve point
            transform.position = curvePoint1;

            // Increment elapsed time for the first segment
            elapsedTime1_B += Time.deltaTime;
        }
        else
        {
            // If the elapsed time exceeds the duration, stop spawning
            //ballSpawner.isSpawn = false;
        }


    }


 
    private Vector3 CalculateBezierPoint_A(Vector3 p0, Vector3 p1, float t)
    {
        // Bezier curve formula
        return Mathf.Pow(1 - t, 2) * p0 + 2 * (1 - t) * t * p0 + Mathf.Pow(t, 2) * p1;
    }

    private Vector3 CalculateBezierPoint_B(Vector3 p0, Vector3 p1, float t)
    {
        // Bezier curve formula
        float oneMinusT = 1f - t;
        return oneMinusT * p0 + t * p1;
    }
    private void OnTriggerEnter(Collider other)
    {
            if (other.gameObject.CompareTag("Ground"))
            {
            optionselection.EnableBasketBallButton();
            ObjectDestroy();
            }

       
            if (other.gameObject.CompareTag("Archery"))
            {
            optionselection.EnableArrowButton();
            ObjectDestroy();
            }
    }

    public void ObjectDestroy()
    {
      
        optionselection.EnbleClick();
        Destroy(gameObject);
       
    }
 
   

      

       
    
}
