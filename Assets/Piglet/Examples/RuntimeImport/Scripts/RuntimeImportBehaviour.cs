﻿using Piglet;
using UnityEngine;
using System.Collections;
using UnityEngine.Events;
using System.Collections.Generic;
/// This MonoBehaviour provides a minimal example for using
/// Piglet to import glTF models at runtime.
/// </summary>
public class RuntimeImportBehaviour : MonoBehaviour
{
    public string modelURL;
    public UnityEvent OnAssetDownloaded;
    public UnityEvent OnLoadingBar;
    private  bool isDownloadComplete = false;
    /// <summary>
    /// The currently running glTF import task.
    /// </summary>
    private GltfImportTask _task;

    /// <summary>
    /// Root GameObject of the imported glTF model.
    /// </summary>
    private GameObject _model;
    [HideInInspector]
    public int completedStep;
    [HideInInspector]
    public int totalStep;
    private bool isFirstTime=true;
    private bool isLoaded;
    public int setup, complete, totals;
    public string tagName;
     
    /// <summary>
    /// Unity callback that is invoked before the first frame.
    /// Create the glTF import task and set up callbacks for
    /// progress messages and successful completion.
    ///  private GoogleTextToSpeech googleTextToSpeech;
    /// </summary>
    /// 
    
   
    void Start()
    {
      //  googleTextToSpeech = FindObjectOfType<GoogleTextToSpeech>();
    }
    public void StartDownloadingModel(string url)
    {
      //  modelURL = PlayerPrefs.GetString(urlName);
        modelURL = url;
      
        // Note: To import a local .gltf/.glb/.zip file, you may
        // instead pass an absolute file path to GetImportTask
        // (e.g. "C:/Users/Joe/Desktop/piggleston.glb"), or a byte[]
        // array containing the raw byte content of the file.
        _task = RuntimeGltfImporter.GetImportTask(modelURL
            /*"https://awesomesaucelabs.github.io/piglet-webgl-demo/StreamingAssets/piggleston.glb"*/);
        _task.OnProgress = OnProgress;
        _task.OnCompleted = OnComplete;
    }
    /// <summary>
    /// Callback that is invoked by the glTF import task
    /// after it has successfully completed.
    /// </summary>
    /// <param name="importedModel">
    /// the root GameObject of the imported glTF model
    /// </param>
    private void OnComplete(GameObject importedModel)
    {
        int childCount = importedModel.transform.childCount;
        if (childCount == 1)
        {
            importedModel.transform.GetChild(0).localPosition = Vector3.zero;
        }

        _model = importedModel;
        
        Debug.Log("Success!");
        // _model.tag = tagName;
        GameObject mainParentObject = new GameObject("ModelParent");
        _model.transform.SetParent(mainParentObject.transform);
        _model.transform.parent.tag = tagName;
        isDownloadComplete = true;
        if (isLoaded)
            StartGame();
        mainParentObject.transform.position= gameObject.transform.position;
        mainParentObject.transform.rotation= gameObject.transform.rotation;





    }
    private void StartGame()
    {
        OnAssetDownloaded.Invoke();
    }

    /// <summary>
    /// Callback that is invoked by the glTF import task
    /// to report intermediate progress.
    /// </summary>
    /// <param name="step">
    /// The current step of the glTF import process.  Each step imports
    /// a different type of glTF entity (e.g. textures, materials).
    /// </param>
    /// <param name="completed">
    /// The number of glTF entities (e.g. textures, materials) that have been
    /// successfully imported for the current import step.
    /// </param>
    /// <param name="total">
    /// The total number of glTF entities (e.g. textures, materials) that will
    /// be imported for the current import step.
    /// </param>
    private void OnProgress(GltfImportStep step, int completed, int total)
    {
        Debug.LogFormat("{0}: {1}/{2}", step, completed, total);
        setup = completed; complete = completed; totals = total;
        if(step==GltfImportStep.Node)

        {
            totalStep = total;
            completedStep = completed;
            if (isFirstTime)
            {
                OnLoadingBar.Invoke();
            }

          
           
        }
        if(step== GltfImportStep.Download)
        {
            isLoaded=true;
        }
    }
    public bool IsDownloadComplete()
    {
        return isDownloadComplete;
    }
    /// <summary>
    /// Unity callback that is invoked after every frame.
    /// Here we call MoveNext() to advance execution
    /// of the glTF import task. Once the model has been successfully
    /// imported, we auto-spin the model about the y-axis.
    /// </summary>
    void Update()
    {
        // advance execution of glTF import task
       _task.MoveNext();

        // spin model about y-axis
       // if (_model != null)
           // _model.transform.Rotate(0, 1, 0);
    }
}
