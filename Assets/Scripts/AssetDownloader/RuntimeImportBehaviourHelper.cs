using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Transformers;
using UnityEngine.XR.Interaction.Toolkit;

public class RuntimeImportBehaviourHelper : MonoBehaviour
{
    // Start is called before the first frame update
    private GoogleTextToSpeech googleTextToSpeech;
    private GameObject model;
    private bool isDestroy;
    private GameObject extractModel;
    private RuntimeImportBehaviour runtimeImportBehaviour;
    private ModelScale modelScale;

    void Awake()
    {
       
         runtimeImportBehaviour =gameObject.GetComponent<RuntimeImportBehaviour>();
        modelScale=FindObjectOfType<ModelScale>();
    }
    public void FindingModel()
    {
        googleTextToSpeech = FindObjectOfType<GoogleTextToSpeech>();
        model = GameObject.FindWithTag(runtimeImportBehaviour.tagName);

        //Layer


      //  modelScale.FindTheModel();
        GameObject emptyObject = new GameObject("Parent");
        emptyObject.transform.localPosition =gameObject.transform.position;
        emptyObject.transform.rotation = Quaternion.identity;
        emptyObject.transform.localScale = Vector3.one;
       
        model.transform.SetParent(emptyObject.transform);
        Debug.Log(emptyObject.transform.GetChild(0).name);
        AddProperty(emptyObject);
        model.transform.position = Vector3.zero;
        emptyObject.tag = "3dmodel";
        emptyObject.transform.GetChild(0).transform.localPosition = Vector3.zero;


    }
    public void AddProperty(GameObject parentObject)
    {

        /*model.transform.position = gameObject.transform.position;
        model.transform.rotation = gameObject.transform.rotation;*/

        if (parentObject.transform.childCount > 0)
        {
            parentObject.transform.GetChild(0).transform.localPosition = Vector3.zero;
            Transform firstChild = parentObject.transform;

            extractModel = firstChild.gameObject;
            BoxCollider boxCollider = firstChild.GetComponent<BoxCollider>();
            if (boxCollider == null)
            {
                AddinSmallBoxCollider(firstChild.gameObject);
                // AddingBoxColliderBasedOnSize(firstChild.gameObject);
             

            }
            else
            {
               // AddingBoxColliderBasedOnSize(firstChild.gameObject);
                AddinSmallBoxCollider(firstChild.gameObject);
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

    public void AddinSmallBoxCollider(GameObject model)
    {
        BoxCollider boxCollider = model.AddComponent<BoxCollider>();
        boxCollider.size = new Vector3(0.1f, 0.1f, 0.1f);
    }
    public void AddingBoxColliderBasedOnSize(GameObject model)
    {
        // Add a BoxCollider component to the GameObject
        BoxCollider boxCollider = model.AddComponent<BoxCollider>();

        // Check if the GameObject has a Renderer component
        Renderer modelRenderer = model.GetComponent<Renderer>();
        if (modelRenderer != null)
        {
            // If the GameObject has a Renderer component,
            // adjust the BoxCollider size to match the Renderer's bounds
            boxCollider.size = modelRenderer.bounds.size;
            // Set the center of the BoxCollider to the center of the GameObject
            boxCollider.center = modelRenderer.bounds.center - model.transform.position;
        }
        else
        {
            // If the GameObject does not have a Renderer, check its children
            Renderer[] renderers = model.GetComponentsInChildren<Renderer>();
            if (renderers.Length > 0)
            {
                // Calculate the combined bounds of all the children's renderers
                Bounds bounds = renderers[0].bounds;
                foreach (Renderer renderer in renderers)
                {
                    bounds.Encapsulate(renderer.bounds);
                }
                // Calculate the center position relative to the GameObject's position
                Vector3 centerOffset = bounds.center - model.transform.position;

                // Set the BoxCollider's center to the calculated center offset
                boxCollider.center = centerOffset;

                // Set the BoxCollider's size based on the combined bounds
                //  boxCollider.size = bounds.size;
                boxCollider.size = new Vector3(0.1f, 0.1f, 0.1f);
            }
            else
            {
                Debug.LogWarning("GameObject does not have a Renderer or Renderer in children to base the BoxCollider size on.");
            }
        }
    }

    public void AssetDownloadCallback()
    {
       FindingModel();
        Invoke("StartSpeaking", 2f);
    }
    public void customCallBack(string text)
    {
        FindingModel();
        googleTextToSpeech.StartTalking(text);
    }
    public void CustomStartSpeak()
    {
        Invoke("StartSpeaking", 2f);
    }
    public void StartSpeaking()
    {
        googleTextToSpeech.AssistantStartTalking();
    }
    public void DisableObject()
    {
       
       
       
       
        XRGeneralGrabTransformer script= extractModel.GetComponent<XRGeneralGrabTransformer>();
        XRGrabInteractable scripttwo = extractModel.GetComponent<XRGrabInteractable>();
        scripttwo.colliders.Clear();
        scripttwo.ClearMultipleGrabTransformers();
        scripttwo.ClearSingleGrabTransformers();
        script.allowOneHandedScaling = false;
        script.clampScaling = false;
        BoxCollider scriptBox = extractModel.GetComponent<BoxCollider>();
        scriptBox.enabled = false;
        Destroy(script);
        Destroy(scripttwo);
        extractModel.gameObject.SetActive(false);
        Destroy(extractModel.gameObject);


        /*  BoxCollider scriptBox = firstChild.GetComponent<BoxCollider>();
          Rigidbody rig = firstChild.GetComponent<Rigidbody>();
          Destroy(script);
          Destroy(scripttwo);
          Destroy(scriptBox);
          Destroy(rig);*/
        // model.SetActive(false);
        //  isDestroy =true;
    }
    // Update is called once per frame
    void Update()
    {
        if(isDestroy)
        {
            if(model!=null)
            {
                Destroy(model.GetComponent<XRGeneralGrabTransformer>());
                Destroy(model.GetComponent<XRGrabInteractable>());
                Destroy(model);
            }

        }
        
    }
  
}
