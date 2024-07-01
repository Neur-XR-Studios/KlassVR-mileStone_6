
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Transformers;

namespace TriLibCore.Samples
{
    /// <summary>
    /// Represents a sample that loads a compressed (Zipped) Model.
    /// </summary>
    public class LoadModelFromURLSample : MonoBehaviour
    {
        /// <summary>
        /// The Model URL.
        /// </summary>
        /// p
        /// 
        private  GameObject lodedGameObject;
        public string ModelURL = "https://ricardoreis.net/trilib/demos/sample/TriLibSampleModel.zip";
        public Transform modelPosition;
        /// <summary>
        /// Cached Asset Loader Options instance.
        /// </summary>
        private AssetLoaderOptions _assetLoaderOptions;

        /// <summary>
        /// Creates the AssetLoaderOptions instance, configures the Web Request, and downloads the Model.
        /// </summary>
        /// <remarks>
        /// You can create the AssetLoaderOptions by right clicking on the Assets Explorer and selecting "TriLib->Create->AssetLoaderOptions->Pre-Built AssetLoaderOptions".
        /// </remarks>
        /// 
        public void StartDownload(string url)
        {
            ModelURL = url;
            if (_assetLoaderOptions == null)
            {
                var assetLoaderOptions = AssetLoader.CreateDefaultLoaderOptions(false, true);
            }
            var webRequest = AssetDownloader.CreateWebRequest(ModelURL);
            AssetDownloader.LoadModelFromUri(webRequest, OnLoad, OnMaterialsLoad, OnProgress, OnError, null, _assetLoaderOptions);

        }
        private void Start()
        {
           
        }
     
        /// <summary>
        /// Called when any error occurs.
        /// </summary>
        /// <param name="obj">The contextualized error, containing the original exception and the context passed to the method where the error was thrown.</param>
        private void OnError(IContextualizedError obj)
        {
            Debug.LogError($"An error occurred while loading your Model: {obj.GetInnerException()}");
        }

        /// <summary>
        /// Called when the Model loading progress changes.
        /// </summary>
        /// <param name="assetLoaderContext">The context used to load the Model.</param>
        /// <param name="progress">The loading progress.</param>
        private void OnProgress(AssetLoaderContext assetLoaderContext, float progress)
        {
            Debug.Log($"Loading Model. Progress: {progress:P}");
        }

        /// <summary>
        /// Called when the Model (including Textures and Materials) has been fully loaded.
        /// </summary>
        /// <remarks>The loaded GameObject is available on the assetLoaderContext.RootGameObject field.</remarks>
        /// <param name="assetLoaderContext">The context used to load the Model.</param>
        private void OnMaterialsLoad(AssetLoaderContext assetLoaderContext)
        {
            Debug.Log("Materials loaded. Model fully loaded.");
        }

        /// <summary>
        /// Called when the Model Meshes and hierarchy are loaded.
        /// </summary>
        /// <remarks>The loaded GameObject is available on the assetLoaderContext.RootGameObject field.</remarks>
        /// <param name="assetLoaderContext">The context used to load the Model.</param>
        private void OnLoad(AssetLoaderContext assetLoaderContext)
        {
            Debug.Log("Model loaded. Loading materials.");
            GameObject loadedModel = assetLoaderContext.RootGameObject;
            lodedGameObject = assetLoaderContext.RootGameObject;
            // Change the position of the loaded model
            loadedModel.transform.position = modelPosition.position;
            loadedModel.transform.rotation = modelPosition.rotation;
            AddProperty(loadedModel);


        }
        public void DisableObject()
        {
            lodedGameObject.SetActive(false);
        }

        public void AddProperty(GameObject parentObject)
        {

            if (parentObject.transform.childCount > 0)
            {
               
                Transform firstChild = parentObject.transform.GetChild(0);

               
                BoxCollider boxCollider = firstChild.GetComponent<BoxCollider>();
                if (boxCollider == null)
                {
                    
                    boxCollider = firstChild.gameObject.AddComponent<BoxCollider>();
                   
                }
                else
                {
                   
                    Debug.LogWarning("The child already has a BoxCollider.");
                }
                Rigidbody rigidbody = firstChild.GetComponent<Rigidbody>();
                if (rigidbody == null)
                {
                   
                    rigidbody = firstChild.gameObject.AddComponent<Rigidbody>();
                    firstChild.gameObject.GetComponent<Rigidbody>().isKinematic = true;
                    firstChild.gameObject.GetComponent<Rigidbody>().useGravity = false;
                    firstChild.gameObject.AddComponent<XRGeneralGrabTransformer>();
                   

                   
                  
                }
                else
                {
                    
                    Debug.LogWarning("The child already has a Rigidbody.");
                }

                
                XRGrabInteractable xrGrabInteractable = firstChild.GetComponent<XRGrabInteractable>();
                if (xrGrabInteractable == null)
                {
                    
                    xrGrabInteractable = firstChild.gameObject.AddComponent<XRGrabInteractable>();
                    firstChild.gameObject.GetComponent<XRGrabInteractable>().useDynamicAttach = true;
                
                   
                }
                else
                {
                   
                    Debug.LogWarning("The child already has an XR Grab Interactable.");
                }
            }
            else
            {
                Debug.LogWarning("The parent object has no children.");
            }



        }
    }

  
}
