using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SessionLodingPage : MonoBehaviour
{

    public Image sliderNew;
    public TextMeshProUGUI progressText;
    public int loadingTime;
    private int counter = 1;
    // Start is called before the first frame update
    void OnEnable()
    {
      StartCoroutine(GenerateRandomProgress(loadingTime));
    }
    IEnumerator GenerateRandomProgress(int totalCount)
    {
        //  loadingScreen.SetActive(true);
       
        if(counter == 3)
        {
            totalCount = 1;
        }
        if (counter == 4)
        {
            totalCount = 1;
        }
        counter++;
        int totalProgressSteps = totalCount;
        for (int i = 0; i <= totalProgressSteps; i++)
        {
            float progress = Mathf.Clamp01((float)i / totalProgressSteps);
            sliderNew.fillAmount = progress;
            progressText.text = (progress * 100f).ToString("F0") + "%";
            yield return new WaitForSeconds(Random.Range(0.1f, 0.1f)); // Random delay between updates
        }

        // Complete the progress
        sliderNew.fillAmount = 1f;
        progressText.text = "100%";
        gameObject.SetActive(false);
       
    }
    

    // Update is called once per frame
    void Update()
    {
        
    }
}
