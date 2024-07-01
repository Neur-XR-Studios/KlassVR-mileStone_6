using System;
using UnityEngine;
using GLTFast;
using System.Collections;
using UnityEngine.Networking;

public class GltfLoader : MonoBehaviour
{
    public string gltfUrl;

    private void Start()
    {

        //EventManager.AddHandler(GameEvent.OnPlayerLanded, LoadGltf);
        LoadGltf();
    }

    private void LoadGltf()
    {
        gltfUrl = "https://raw.githubusercontent.com/KhronosGroup/glTF-Sample-Models/master/2.0/Duck/glTF/Duck.gltf"; 
        StartCoroutine(DownloadGltf(gltfUrl));
    }


    private IEnumerator DownloadGltf(string url)
    {
        using (UnityWebRequest www = UnityWebRequest.Get(url))
        {
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.ConnectionError || www.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError($"Failed to download glTF: {www.error}");
            }
            else
            {
                // Save the downloaded glTF to a temporary file
                string tempFilePath = $"{Application.persistentDataPath}/temp.gltf";

                // Load glTF from the temporary file
               LoadGltfFromFile(tempFilePath);
            }
        }
    }

    private async void LoadGltfFromFile(string filePath)
    {
        try
        {
            // Resetting any previous loaded content
          //  ResetScene();

            // Create a glTF importer
            GltfImport gltfImporter = new GltfImport();
           
            // Load glTF from file
            bool success = await gltfImporter.Load($"file://{filePath}"); // <-- Error here
            Debug.Log($"After loading glTF. Success: {success}");

            if (success)
            {
                // Instantiate the main scene from glTF
                await gltfImporter.InstantiateMainSceneAsync(transform);
            }
            else
            {
                Debug.LogError("Failed to load glTF.");
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"An error occurred while loading the glTF: {ex.Message}");
        }
    }

    private void ResetScene()
    {
        // Clear the existing scene content
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }
    }
}
