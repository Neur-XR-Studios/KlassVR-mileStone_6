using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class LoadingBar : MonoBehaviour
{
    public GameObject loadingScreen;
   // public Slider slider;
    public Image sliderNew;
    public TextMeshProUGUI progressText;
    private RuntimeImportBehaviour importBehaviour;
    public GameObject[] toggleItem;
    private GameObject parentObject;
    public UnityEvent StartGame;
    private int totalCount;
    private bool IsmodelDownload;
    public DeviceSynHandler deviceSynHandler;
    private bool isLoaded;
   
    private API api;
  
    private void Start()
    {
        api=FindObjectOfType<API>();
      //  deviceSynHandler=FindObjectOfType<DeviceSynHandler>();
        parentObject = transform.parent.gameObject;
        //importBehaviour =FindAnyObjectByType<RuntimeImportBehaviour>();
        // SimulateLoading();
        ToggleObject(false);
    }
  
    public void CustomSumulatedLoading()
    {
        StartCoroutine(GenerateRandomProgress());
        totalCount = 20;
    }
    public void SimulateLoading(RuntimeImportBehaviour finalScript)
    {
        importBehaviour=finalScript;
        IsmodelDownload =true;
        totalCount = importBehaviour.totalStep;
        StartCoroutine(GenerateRandomProgress());
    }

    IEnumerator GenerateRandomProgress()
    {
        loadingScreen.SetActive(true);

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
        isLoaded=true;
        DeviceSync();
    }
    private void Update()
    {
       
        
    }

    public void DeviceSync()
    {
        deviceSynHandler.SynDevice();


    }
    public void SynchDevice()
    {
        ToggleObject(true);
        if (IsmodelDownload)
            StartGame.Invoke();

        //api.CallingClassVrExperiance();


    }
    public void ToggleObject(bool isTrue)
    {
        if (isTrue)
        {
            foreach (var item in toggleItem)
            {
                item.SetActive(true); 
            }
            parentObject.SetActive(false);

        }
        else
        { 
            foreach (var item in toggleItem)

            { item.SetActive(false); 
            }
           
        }
            
    }
}
