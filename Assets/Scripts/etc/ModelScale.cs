using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static ElevenLabsTTS_Manager;

public class ModelScale : MonoBehaviour
{
    public GameObject referenceObject; // Reference object (e.g., cube)
    public Transform distancePoint;
    // Array of models you want to resize
    public GameObject[] modelsToResize;
    private bool isFirstTime;
    public AnnotationManager annotationManager;
    private List<Vector3> boundingBoxScales = new List<Vector3>();
    private int numberOFElement = 1;
    private int elementInBoundingBox = 0;
    private GameObject downloadedModel;
    private int loopStoper=1;
    private bool isAssignValue;
    void Start()
    {
        annotationManager = FindObjectOfType<AnnotationManager>();

        isFirstTime = true;
        // Initialize modelsToResize array
        modelsToResize = new GameObject[0];
        //FindTheModel();
    }

    public void FindTheModel()
    {
        // Find game objects with the specified tags and combine their arrays
        GameObject[] modelsVRModel = GameObject.FindGameObjectsWithTag("VRModel");
        GameObject[] modelsVRModel1 = GameObject.FindGameObjectsWithTag("VRModel1");
        GameObject[] modelsVRModel3 = GameObject.FindGameObjectsWithTag("VRModel2");

        // Combine arrays
        GameObject[] combinedModels = CombineArrays(modelsVRModel, modelsVRModel1, modelsVRModel3);

        // Set modelsToResize array
        modelsToResize = combinedModels;

        // Start the resizing activity
        if (isFirstTime)
        {
            //  SetServerValue();
            isFirstTime = false;
        }
        else
        {
            // SetServerValue();
        }

    }

    public void SetServerValue(GameObject model)
    {
        boundingBoxScales = annotationManager.boundingBoxScales;

        /* foreach (GameObject model in modelsToResize)
         {
            */ // model.transform.localScale = new Vector3(0.02f, 0.02f, 0.02f);
        if (boundingBoxScales.Count >= numberOFElement)
        {
            if (boundingBoxScales[elementInBoundingBox] != Vector3.zero)
            {
                isAssignValue=true;
                model.transform.GetChild(0).gameObject.transform.localScale = boundingBoxScales[elementInBoundingBox];
            }
               
        }

        elementInBoundingBox++;
        numberOFElement++;
        /*}*/
    }

    private GameObject[] CombineArrays(params GameObject[][] arrays)
    {
        List<GameObject> combinedList = new List<GameObject>();

        foreach (var array in arrays)
        {
            combinedList.AddRange(array);
        }

        return combinedList.ToArray();
    }

   

    Vector3 GetBoundingBoxSize(GameObject obj)
    {
        Renderer[] renderers = obj.GetComponentsInChildren<Renderer>();
        Bounds bounds = new Bounds(obj.transform.position, Vector3.zero);
        foreach (Renderer renderer in renderers)
        {
            bounds.Encapsulate(renderer.bounds);
        }
        return bounds.size;
    }


    public void StartActivitySingleObject(GameObject modelToResize)
    {
        if(!isAssignValue)
        {
            Debug.Log(modelToResize.transform.localScale);
            Vector3 boundingBoxSize = referenceObject.GetComponent<Renderer>().bounds.size;

            // Calculate the combined size of all mesh renderers in the target object hierarchy
            Vector3 modelSize = GetCombinedMeshRendererSize(modelToResize);



            // Calculate the scale needed to fit inside the bounding box
            Vector3 localScale = modelToResize.transform.localScale;

            // Calculate scale factor for each axis
            float scaleX = boundingBoxSize.x / modelSize.x;
            float scaleY = boundingBoxSize.y / modelSize.y;
            float scaleZ = boundingBoxSize.z / modelSize.z;

            // Apply the minimum scale factor to maintain aspect ratio
            float scaleFactor = Mathf.Min(scaleX, scaleY, scaleZ);
            localScale.x *= scaleFactor;
            localScale.y *= scaleFactor;
            localScale.z *= scaleFactor;

            modelToResize.transform.localScale = localScale;
            // Position the model at the specified distance point
            modelToResize.transform.position = distancePoint.position;

            Debug.Log($"Model {modelToResize.name} resized with scale factor {scaleFactor} to match reference size.");

            // this function run while any axis  of renderer.bounds value will be zero

            if (modelSize.x == 0 || modelSize.y == 0 || modelSize.z == 0)
            {
                if (loopStoper <= 3)
                {
                    downloadedModel = modelToResize;
                    // StartActivitySingleObject(downloadedModel);


                    loopStoper++;
                }


            }

        }
        isAssignValue = false;
        //StartActivitySingleObjectSecondIteration(modelToResize);
    }
    private Vector3 GetCombinedMeshRendererSize(GameObject obj)
    {

        Renderer[] renderers = obj.GetComponentsInChildren<Renderer>();
        Bounds bounds = new Bounds(obj.transform.position, Vector3.zero);
        foreach (Renderer renderer in renderers)
        {
            bounds.Encapsulate(renderer.bounds);
        }
        return bounds.size;
    }

    public void SetBoxColliderBasedOnSize(GameObject modelChild)
    {
        GameObject model = modelChild.transform.parent.gameObject;
        BoxCollider boxCollider;
        // Get the BoxCollider component attached to the GameObject
        if (model.GetComponent<BoxCollider>() == null)
        {
            boxCollider = model.AddComponent<BoxCollider>();
        }
        else
        {
            boxCollider = model.GetComponent<BoxCollider>();
        }



        // Check if the BoxCollider component exists
        if (boxCollider != null)
        {
            // Get all child Renderers
            Renderer[] renderers = model.GetComponentsInChildren<Renderer>();

            // Filter Renderers that have a MeshRenderer component
            List<Renderer> meshRenderers = new List<Renderer>();
            foreach (Renderer renderer in renderers)
            {
                if (renderer is MeshRenderer && renderer.tag != "notAChild" && renderer.name != "Quad")
                {
                    meshRenderers.Add(renderer);
                }
            }

            // Check if there are any child Renderers with MeshRenderer component
            if (meshRenderers.Count > 0)
            {
                // Calculate the combined bounds of all the filtered renderers
                Bounds bounds = meshRenderers[0].bounds;
                foreach (Renderer renderer in meshRenderers)
                {
                    bounds.Encapsulate(renderer.bounds);
                }

                // Calculate the center position relative to the GameObject's position
                Vector3 centerOffset = bounds.center - model.transform.position;

                // Set the BoxCollider's center to the calculated center offset
                boxCollider.center = centerOffset;

                // Set the BoxCollider's size based on the combined bounds
                boxCollider.size = bounds.size;
            }
            else
            {
                // Warn if the GameObject and its children with MeshRenderer components are not found
                Debug.LogWarning("GameObject and its children do not have a Renderer component with MeshRenderer.");
            }
        }
        else
        {
            // Warn if the GameObject doesn't have a BoxCollider
            Debug.LogWarning("GameObject does not have a BoxCollider component.");
        }
    }
}
