using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class DownloadManager : MonoBehaviour
{
    public GameObject[] objectsToEnable;
    public UnityEvent OnDownloadComplete;
    private ClassVrManager classVrManager;

    void Start()
    {
      classVrManager=FindAnyObjectByType<ClassVrManager>();
    }
    public void EnbleCallback()
    {
        RuntimeImportBehaviour[] downloadManagers = FindObjectsOfType<RuntimeImportBehaviour>();
        foreach (RuntimeImportBehaviour manager in downloadManagers)
        {
            manager.OnAssetDownloaded.AddListener(OnDownloadCompleted);
        }

    }

    void OnDownloadCompleted()
    {
        // Check if all downloads are complete
        RuntimeImportBehaviour[] downloadManagers = FindObjectsOfType<RuntimeImportBehaviour>();
        foreach (RuntimeImportBehaviour manager in downloadManagers)
        {
            if (!manager.IsDownloadComplete())
                return; // If any download is not complete, return early
        }

        classVrManager.LoadingBarSimulator();
        // If all downloads are complete, enable the desired game objects

    }
}
