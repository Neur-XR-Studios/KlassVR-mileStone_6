using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FitInsideBoundingBox : MonoBehaviour
{
    public GameObject boundingBoxReference;
    private GameObject[] targetObjects;
    private Dictionary<GameObject, bool> processedObjects = new Dictionary<GameObject, bool>();

    void Start()
    {
        if (boundingBoxReference == null)
        {
            Debug.LogError("Bounding box reference is not assigned!");
            return;
        }
    }

    void Update()
    {
        FindAndFitModels();
    }

    private void FindAndFitModels()
    {
        // Find game objects with the specified tags and combine their arrays
        GameObject[] modelsVRModel = GameObject.FindGameObjectsWithTag("VRModel");
        GameObject[] modelsVRModel1 = GameObject.FindGameObjectsWithTag("VRModel1");
        GameObject[] modelsVRModel2 = GameObject.FindGameObjectsWithTag("VRModel2");

        // Combine arrays
        targetObjects = CombineArrays(modelsVRModel, modelsVRModel1, modelsVRModel2);

        // Fit each target object inside the bounding box if not already processed
        foreach (GameObject targetObject in targetObjects)
        {
            if (targetObject == null || processedObjects.ContainsKey(targetObject)) continue;

            // Mark the object as processed
            processedObjects[targetObject] = true;

            // Create a new parent object to act as the reference model for grabbing
            GameObject referenceModel = new GameObject("ReferenceModel");

            // Set the position of the reference model to the bounding box reference position
            referenceModel.transform.position = boundingBoxReference.transform.position;
            referenceModel.transform.rotation = boundingBoxReference.transform.rotation;

            // Make the target object a child of the reference model
            targetObject.transform.parent = referenceModel.transform;

            // Get the size of the bounding box reference object
            Vector3 boundingBoxSize = boundingBoxReference.GetComponent<Renderer>().bounds.size;

            // Calculate the combined size of all mesh renderers in the target object hierarchy
            Vector3 modelSize = GetCombinedMeshRendererSize(targetObject);

            // Calculate the scale needed to fit inside the bounding box
            Vector3 localScale = targetObject.transform.localScale;

            // Calculate scale factor for each axis
            float scaleX = boundingBoxSize.x / modelSize.x;
            float scaleY = boundingBoxSize.y / modelSize.y;
            float scaleZ = boundingBoxSize.z / modelSize.z;

            // Apply the minimum scale factor to maintain aspect ratio
            float scaleFactor = Mathf.Min(scaleX, scaleY, scaleZ);
            localScale.x *= scaleFactor;
            localScale.y *= scaleFactor;
            localScale.z *= scaleFactor;

            targetObject.transform.localScale = localScale;

            // Position the target object at the center of the bounding box reference
            PositionTargetInsideBoundingBox(boundingBoxReference, targetObject);
        }
    }

    // Recursively calculate the combined size of all mesh renderers in the hierarchy
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

    // Position the target object inside the bounding box reference
    private void PositionTargetInsideBoundingBox(GameObject boundingBoxReference, GameObject targetObject)
    {
        // Get the center of the bounding box reference
        Vector3 boundingBoxCenter = boundingBoxReference.GetComponent<Renderer>().bounds.center;

        // Calculate the offset to center the target object
        Vector3 targetCenter = GetCombinedMeshRendererCenter(targetObject);
        Vector3 offset = targetObject.transform.position - targetCenter;

        // Set the target object's position to the bounding box center
        targetObject.transform.position = boundingBoxCenter + offset;
    }

    // Recursively calculate the combined center of all mesh renderers in the hierarchy
    private Vector3 GetCombinedMeshRendererCenter(GameObject obj)
    {
        Renderer[] renderers = obj.GetComponentsInChildren<Renderer>();
        Bounds bounds = new Bounds(obj.transform.position, Vector3.zero);
        foreach (Renderer renderer in renderers)
        {
            bounds.Encapsulate(renderer.bounds);
        }
        return bounds.center;
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
}
