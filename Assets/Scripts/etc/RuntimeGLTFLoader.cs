using System;
using System.IO;
using TMPro;
using UnityEngine;
public class RuntimeGLTFLoader : MonoBehaviour
{
    public TextMeshProUGUI errorText;
    async void Start()
    {
        // Create a new GltfImport instance
        var gltf = new GLTFast.GltfImport();

        // Specify the URL or file path of the glTF asset
        var gltfUrl = "https://raw.githubusercontent.com/KhronosGroup/glTF-Sample-Models/master/2.0/Duck/glTF/Duck.gltf";

        // Or use a local file path
        // var gltfPath = "/path/to/your/file.gltf";

        try
        {
            // Load glTF asynchronously
            bool success = await gltf.Load(gltfUrl);

            if (success)
            {
                Debug.Log("glTF loaded successfully!");

                // Instantiate the main scene of the glTF
                await gltf.InstantiateMainSceneAsync(transform);

                Debug.Log("glTF instantiated successfully!");
            }
            else
            {
                Debug.LogError("Loading glTF failed!");
                errorText.text = "Loading glTF failed!";
            }
        }
        catch (Exception ex)
        {
            errorText.text = ex.Message;
            Debug.LogError($"Exception while loading glTF: {ex.Message}");
        }
    }
}
