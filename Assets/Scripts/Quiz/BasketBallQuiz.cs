using LibTessDotNet;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.Timeline;
using UnityEngine.UI;

public class BasketBallQuiz : MonoBehaviour
{
    public TextMeshProUGUI questionText;
    public TextMeshProUGUI MCQquestionText;
    public TextMeshProUGUI questionPanelText;
    public Button[] answerButtons;
    public Button nextButton;
    private bool buttonClicked = false;
    public UnityEvent reloadSceneEvent;
    private API apiManager;
    public UnityEvent StartEvent;
    public GameObject elementWantToDelete;
    public UnityEvent nextAction;
    public bool isArrow;
    private int basketballScore;
    private int arrowScore;

    [System.Serializable]
    private class Wrapper
    {
        public List<QuestionData> questions;
    }

    [System.Serializable]
    public struct QuestionData
    {
        public string question;
        public List<AnswerData> options;
        public string createdBy;
        public string sessionId;
        public string id;
    }

   

   // private List<QuestionData> questions;
    private int currentQuestionIndex = 0;
    public MeshRenderer[] meshRender;
    public bool isBasketBall;
    public GameObject basketBall;
    public GameObject arrow;
    private GamificationManager gamificationManager;
    private List<AnswerData> answers=new List<AnswerData>();
  
    private void OnEnable()
    {
        gamificationManager = FindAnyObjectByType<GamificationManager>();
        BallSpawner.isGamefication=true;
        apiManager = FindAnyObjectByType<API>();
         Wrapper wrapper = JsonUtility.FromJson<Wrapper>("{\"questions\":" + apiManager.assessmentsString + "}");
       // questions = wrapper.questions;

        //DisplayQuestion();
        StartEvent.Invoke();
        nextButton.enabled = false;
        Invoke("DeleteElement", 4f);
        foreach (var element in meshRender)
        {
            element.enabled = true;
        }

    }
    private void Start()
    {
        nextButton.enabled = false;
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.H))
        {
            NextButtonClick();
        }
    }

    public void DisplayQuestion(List<string>heading, List<AnswerData> answerOptions,string actualQuestion)
    {
        answers = answerOptions;
        questionText.text = actualQuestion;
        if (questionText != null && currentQuestionIndex < 1)
        {
            // MCQquestionText.text = $"MCQ{currentQuestionIndex + 1}";
           //  MCQquestionText.text = heading[0];
           // questionPanelText.text = $"Question{currentQuestionIndex + 1}";
            questionPanelText.text = heading[1];
            //questionText.text = $"{questions[currentQuestionIndex].question}";
           
        }

        for (int i = 0; i < answerButtons.Length; i++)
        {
            if (i < answerButtons.Length)
            {
                if (answerButtons[i] != null)
                {
                    answerButtons[i].gameObject.SetActive(true);

                    TextMeshProUGUI buttonText = answerButtons[i].GetComponentInChildren<TextMeshProUGUI>();
                    if (buttonText != null)
                    {
                       buttonText.text = answerOptions[i].text;
                    }
                    else
                    {
                        Debug.LogError("TextMeshProUGUI component not found on the button.");
                    }

                    int answerIndex = i;
                    answerButtons[i].onClick.AddListener(() => OnAnswerClick(answerIndex));
                  
                }
                else
                {
                    Debug.LogError("Button is null at index " + i);
                }
            }
            else if (answerButtons[i] != null)
            {
                answerButtons[i].gameObject.SetActive(false);
            }
        }

        buttonClicked = false;
    }

    private void OnAnswerClick(int selectedAnswerIndex)
    {
        if (!buttonClicked)
        {
            Button clickedButton = answerButtons[selectedAnswerIndex];
            // Create colors with adjusted alpha values
            Color greenWithAlpha = new Color(Color.green.r, Color.green.g, Color.green.b, 95f / 255f);
            Color redWithAlpha = new Color(Color.red.r, Color.red.g, Color.red.b, 95f / 255f);
            // Change button color immediately for the correct answer button
            //int correctAnswerIndex = questions[currentQuestionIndex].options.FindIndex(option => option.isCorrect);
            int correctAnswerIndex = answers.FindIndex(answers => answers.isCorrect);
            ChangeButtonColor(answerButtons[correctAnswerIndex], greenWithAlpha);
            // Change button color immediately for the clicked button
            ChangeButtonColor(clickedButton, answers[selectedAnswerIndex].isCorrect ? greenWithAlpha : redWithAlpha);
            if (answers[selectedAnswerIndex].isCorrect)
            {
                if(isArrow)
                {
                    //  arrowScore++;
                    gamificationManager.arrowScore++;
                }
                else
                {
                    gamificationManager.basketBallScore++;
                  //  basketballScore++;
                }
            }
            buttonClicked = true;
            ToggleButtonActivation(false);
        }

    }

    public void ToggleButtonActivation(bool activated)
    {
        foreach (var button in answerButtons)
        {
            button.enabled = activated;
        }
    }

   

    public void NextButtonClick()
    {
        
        nextButton.enabled = false;
        for (int i = 0; i < answerButtons.Length; i++)
        {
            Color newColor=  new Color(150f / 255f, 150f / 255f, 150f / 255f, 255f / 255f);
            StartCoroutine( RestoreButtonColor(answerButtons[i], newColor));
            Debug.Log(answerButtons[i].gameObject.name);
          
        }
        ToggleButtonActivation(false);
        buttonClicked = false;

        currentQuestionIndex++;
        if (currentQuestionIndex < 1)
        {
          
            // DisplayQuestion();
        }
        else
        {
            if(isArrow)
            {
                //add api here
                Debug.Log(arrowScore);
              
            }
            else
            {
                //add api here
              
                Debug.Log(basketballScore);
            }
            gamificationManager.OnGameCompleted();
            reloadSceneEvent.Invoke();
            currentQuestionIndex = 0;
        }
    }

    private void ChangeButtonColor(Button button, Color color)
    {

        Graphic graphic = button.GetComponent<Graphic>();
        if (graphic != null)
        {
           graphic.color = color;
        }
        else
        {
            Debug.LogError("Graphic component not found on the button.");
        }
    }

    private IEnumerator RestoreButtonColor(Button button, Color color)
    {
        // This is essentially not causing any delay, consider removing or increasing the time if needed

        // Create a new color with baseColor RGB values but with the Alpha set to 95/255
          Color colors = new Color(color.r, color.g, color.b,  255f);

        // Restore button color after the delay
        ChangeButtonColor(button, colors);

        // If the button is the correct answer button, find its index and restore its color
        int correctAnswerIndex = answers.FindIndex(answers => answers.isCorrect);
        if (correctAnswerIndex != -1)
        {
            ChangeButtonColor(answerButtons[correctAnswerIndex], color);
            
        }
    //    ToggleButtonActivation(false);
        yield return new WaitForSeconds(0f);
    }
    public void DeleteElement()
    {
        Destroy(elementWantToDelete);
    }
    public void SceneReLoad()
    {
        nextAction.Invoke();
       // SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
    public void SceneLoading()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
