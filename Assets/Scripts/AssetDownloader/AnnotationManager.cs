using SimpleJSON;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class AnnotationManager : MonoBehaviour
{
    [System.Serializable]
    public class AnnotationData
    {
        public string UniqueName;
        public string ObjectName;
        public ModelPositionData ModelPosition;
        public RectData RectProperties;
        public string AnnotationText;
        public string annotationCount;
        public Vector3 AnnotationPosition;
        public Quaternion AnnotationRotation;
        public string heading;
    }
    [System.Serializable]
    public class AnnotationPositionData
    {
        public float x;
        public float y;
        public float z;
    }
    [System.Serializable]
    public class AnnotationRotationData
    {
        public float x;
        public float y;
        public float z;
        public float w;
    }
    [System.Serializable]
    public class ModelPositionData
    {
        public Vector3 position;
        public Quaternion rotation;
    }
    [System.Serializable]
    public class RectData
    {
        public float Width;
        public float Height;
        public float PosX;
        public float PosY;
    }
    public GameObject parentObject;
    public string jsonString;
    public GameObject model;
    public Canvas canvasPrefab;
    private Canvas canvas;
    public TextMeshProUGUI textPrefab;
    public float position;
    public Button buttonPrefab;
    public GameObject cubeButton;
    public Transform target;
    private string downlodedModelName;
    public TextMeshProUGUI displayText;
    public TextMeshProUGUI description;
    private GoogleTextToSpeech googleTextToSpeech;
    private LineRenderer lineRender;
    public float distance;
    private List<string> jsonStrings;
    int numberOfAnnotations;
    private int index;
    private List<string> modelTagNames;
    private int modelCount = 0;
    private List<int> orderOfAnnotation;
    private int orderExcicuter;
    private int listexcicuter;
    public GameObject modelDescriptionPanel;
    private GameObject Nmodel;
    private ModelScale modelScale;
    public AssetDownloader assetDownloader;
    private bool isfirstTime;
    public List<Vector3> boundingBoxScales = new List<Vector3>();
    private bool isFirstTime = true;
    private GameObject modelWithcollider;
    public List<GameObject> tempCube = new List<GameObject>();
    // Start is called before the first frame update
    void Start()
    {
        modelScale = FindObjectOfType<ModelScale>();
        modelTagNames = new List<string>();
        modelTagNames.Add("VRModel");
    }
    public void MultipleModelCall(int value)
    {
        index = value;
        initialization();
    }
    public void initialization()
    {
        parentObject.transform.position = Vector3.zero;
        GameObject objectsWithTag = GameObject.FindGameObjectWithTag(modelTagNames[modelCount]);
        Transform childTransform = objectsWithTag.transform.parent;
        model = childTransform.gameObject;
        googleTextToSpeech = FindObjectOfType<GoogleTextToSpeech>();
        model.transform.SetParent(parentObject.transform);
        Transform child = model.transform.Find("ModelParent");
        //The following line sets the model's position to Vector3.zero, ensuring that the second and third models are positioned correctly.

        //  model.transform.position=Vector3.zero;

        if (orderOfAnnotation.Count > listexcicuter)
        {
            if (orderOfAnnotation[listexcicuter] == orderExcicuter)
            {

                
                AssignPosithionRotationAndCreateCanvas(listexcicuter);
                StartCoroutine(ChangePosition());

                listexcicuter++;

            }
            else
            {
                StartCoroutine(ChangePosition());
            }
        }
        else
        {
            AvoidFloating();
            StartCoroutine(ChangePosition());

            //  modelDescriptionPanel.SetActive(false);
        }


        Invoke("SetPosition", 2f);
        // SetPosition(); 
        modelCount++;
        modelTagNames.Add("VRModel" + modelCount.ToString());
        orderExcicuter++;


    }
    public void AvoidFloating()
    {

    }
    IEnumerator ChangePosition()
    {
        // this functionn will works when we put annotation without bounding box

        yield return new WaitForSeconds(1f);
        Transform child = model.transform.GetChild(0);
        modelScale.StartActivitySingleObject(child.GetChild(0).gameObject);
        child.GetChild(0).gameObject.transform.localPosition = Vector3.zero;
        SetBoxColliderBasedOnSize(model);
        assetDownloader.ChangeModelPosition();
        SetBoxColliderBasedOnSize(model);
       // AssignDistance();


        //  SettingAllignmentOFModel(child.transform.GetChild(0).gameObject);
        /*
                if(isfirstTime==false)
                {
                    modelScale.FindTheModel();
                    modelScale.FindTheModel();
                    isfirstTime = true;
                }

                assetDownloader.ChangeModelPosition();*/
    }

    public void SetPosition()
    {

        // parentObject.transform.localPosition = target.position;

        //-------
        /*  Nmodel = parentObject.transform.GetChild(0).gameObject;
          Vector3 parentBottomPosition = GetBottomPosition(Nmodel);

          // Find the bottom position of the target
          Vector3 targetBottomPosition = GetBottomPosition(target.gameObject);

          // Calculate the offset to align their bottom positions
          Vector3 offset = parentBottomPosition - targetBottomPosition;
  */
        // Apply the offset to the parentObject's position
        /* Nmodel.transform.position -= offset;*/
        //  model.transform.localScale = Vector3.one;

    }
    Vector3 GetBottomPosition(GameObject obj)
    {
        // Get the bounds of the GameObject
        Bounds bounds;
        Renderer renderer = obj.GetComponent<Renderer>();
        if (renderer != null)
        {
            bounds = renderer.bounds;
        }
        else
        {
            Collider collider = obj.GetComponent<Collider>();
            if (collider != null)
            {
                bounds = collider.bounds;
            }
            else
            {
                Debug.LogError("GameObject has neither renderer nor collider.");
                return Vector3.zero;
            }
        }

        // Calculate the bottom position
        Vector3 bottomPosition = bounds.center - Vector3.up * bounds.extents.y;

        return bottomPosition;
    }
    public void AssignValueTOJson(List<string> json, List<int> order)
    {
        orderOfAnnotation = order;
        jsonStrings = json;
        numberOfAnnotations = jsonStrings.Count;
    }
    //--
    public void AssignPosithionRotationAndCreateCanvas(int index)
    {
        if (jsonStrings == null)
        {
            Debug.LogError("jsonStrings is null or the index is out of range.");
            return;
        }

        // Parse the JSON string into a JSON array


        if (jsonStrings.Count <= index)
        {
            /* model.transform.position=Vector3.zero;
             model.transform.rotation = Quaternion.identity;*/
            Debug.Log("no element");
            modelDescriptionPanel.SetActive(false);
        }
        else
        {
            modelDescriptionPanel.SetActive(true);
            JSONNode jsonData = JSON.Parse(jsonStrings[index]);
            // Create a list to hold your AnnotationData objects
            List<AnnotationData> data = new List<AnnotationData>();

            // Iterate over each element in the JSON array
            for (int i = 0; i < jsonData.Count; i++)
            {
                JSONNode annotationNode = jsonData[i];

                // Create an AnnotationData object and populate its fields
                AnnotationData annotation = new AnnotationData();

                annotation.UniqueName = annotationNode["UniqueName"];
                annotation.ObjectName = annotationNode["ObjectName"];
                annotation.heading = annotationNode["heading"];
                downlodedModelName = annotation.ObjectName;

                // Parse ModelPositionData
                annotation.ModelPosition = new ModelPositionData
                {
                    position = new Vector3(
                        annotationNode["ModelPosition"]["position"]["x"].AsFloat,
                        annotationNode["ModelPosition"]["position"]["y"].AsFloat,
                        annotationNode["ModelPosition"]["position"]["z"].AsFloat
                    ),
                    rotation = new Quaternion(
                        annotationNode["ModelPosition"]["rotation"]["x"].AsFloat,
                        annotationNode["ModelPosition"]["rotation"]["y"].AsFloat,
                        annotationNode["ModelPosition"]["rotation"]["z"].AsFloat,
                        annotationNode["ModelPosition"]["rotation"]["w"].AsFloat
                    )
                };

                // Parse RectData
                annotation.RectProperties = new RectData
                {
                    Width = annotationNode["RectWidth"].AsFloat,
                    Height = annotationNode["RectHeight"].AsFloat,
                    PosX = annotationNode["RectPosX"].AsFloat,
                    PosY = annotationNode["RectPosY"].AsFloat
                };
                annotation.AnnotationText = annotationNode["AnnotationText"];
                annotation.annotationCount = annotationNode["ObjectCount"];
                annotation.AnnotationPosition = new Vector3(
                annotationNode["AnnotationPosition"]["x"].AsFloat,
                annotationNode["AnnotationPosition"]["y"].AsFloat,
                annotationNode["AnnotationPosition"]["z"].AsFloat
                );

                annotation.AnnotationRotation = new Quaternion(
                annotationNode["AnnotationRotation"]["x"].AsFloat,
                annotationNode["AnnotationRotation"]["y"].AsFloat,
                annotationNode["AnnotationRotation"]["z"].AsFloat,
                annotationNode["AnnotationRotation"]["w"].AsFloat
            );


                // Add the annotation to the list
                data.Add(annotation);


                Vector3 boundingBoxScale = new Vector3(
            annotationNode["boundingboxScale"]["x"].AsFloat,
            annotationNode["boundingboxScale"]["y"].AsFloat,
            annotationNode["boundingboxScale"]["z"].AsFloat
        );

                // Add to HashSet only if it's not repeated
                if (!boundingBoxScales.Contains(boundingBoxScale))
                {
                    boundingBoxScales.Add(boundingBoxScale);
                }
            }

            // Now you have a list of AnnotationData objects
            // You can iterate through the list and do whatever you need to do with the data
            Transform child = model.transform.Find("ModelParent");
            modelWithcollider = child.transform.parent.gameObject;
            modelScale.SetServerValue(child.gameObject);
            canvas = Instantiate(canvasPrefab);
            canvas.tag = "notAChild";
            canvas.renderMode = RenderMode.WorldSpace;

            canvas.transform.SetParent(model.transform.GetChild(0));

            foreach (var annotation in data)
            {
                if (annotation.ObjectName == downlodedModelName)
                {/*
                  *   modelScale.FindTheModel();
                    model.transform.localPosition = annotation.ModelPosition.position;
                    model.transform.localRotation = annotation.ModelPosition.rotation;*/
                    if (isFirstTime)
                    {
                        //modelScale.FindTheModel();
                        isFirstTime = false;
                    }

                    RectTransform canvasRect = canvas.GetComponent<RectTransform>();
                    canvasRect.anchoredPosition3D = new Vector3(annotation.RectProperties.PosX, annotation.RectProperties.PosY, 0f);


                    TextMeshProUGUI newText = Instantiate(textPrefab, canvas.transform);
                    newText.text = annotation.AnnotationText;


                    //button
                    Button newButton = Instantiate(buttonPrefab, canvas.transform);
                    newButton.onClick.AddListener(() => OnButtonClick(annotation.AnnotationText, annotation.heading));

                    CreateEmptyObjectAtPosition(new Vector3(annotation.AnnotationPosition.x, annotation.AnnotationPosition.y, annotation.AnnotationPosition.z), annotation.AnnotationRotation, newText, newButton, annotation.AnnotationText, annotation.annotationCount, annotation.heading);




                }
            }

        }
        SettingAllignmentOFModel(model.transform.GetChild(0).gameObject);

    }

    public void SettingAllignmentOFModel(GameObject downloadedModelParent)
    {
       // SetBoxColliderBasedOnSize(downloadedModelParent.transform.parent.gameObject);
        //modelScale.SetServerValue();
        //modelScale.FindTheModel();
        // modelScale.SetServerValue();
        /*  downloadedModelParent.transform.GetChild(0).transform.localPosition = Vector3.zero;
         
          assetDownloader.ChangeModelPosition();*/

        //AddingBoxColliderBasedOnSize();



    }

    public void SetBoxColliderBasedOnSize(GameObject model)
    {
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




    void OnButtonClick(string annotationText, string heading)
    {
        displayText.text = annotationText;
        description.text = heading;
        //  googleTextToSpeech.StartTalking(annotationText);
        //  Debug.Log("Button clicked! Annotation Text: " + annotationText);
    }
    public GameObject Detach(GameObject positionAlignment, GameObject downloadedModel)
    {

       // positionAlignment.transform.SetParent(downloadedModel.transform.GetChild(0));
        return positionAlignment;
    }
    private void CreateEmptyObjectAtPosition(Vector3 annotationPosition, Quaternion rotation, TextMeshProUGUI newText, Button newButton, string annotationText, string buttonNumber, string heading)
    {

        BoxCollider boxCollider = model.GetComponent<BoxCollider>();
        float centerWidth = boxCollider.center.y + transform.position.y;


        GameObject emptyObject = new GameObject("EmptyObject");
        emptyObject.tag = "notAChild";
        emptyObject.transform.SetParent(model.transform.GetChild(0));
        emptyObject.transform.localPosition = annotationPosition;
        GameObject sourse = new GameObject("Sourse");
        Transform child = model.transform.GetChild(0);
        sourse.transform.SetParent(child.transform.GetChild(0));


        newText.transform.rotation = emptyObject.transform.rotation;
        GameObject newcubeButton = Instantiate(cubeButton, model.transform.GetChild(0));
        newcubeButton.tag = "notAChild";
        newcubeButton.transform.localScale = new Vector3(.05f, .05f, .05f);
        newcubeButton.transform.localRotation = emptyObject.transform.rotation;
        newcubeButton.transform.localPosition = emptyObject.transform.localPosition;
        sourse.transform.position = emptyObject.transform.position;
     //   emptyObject = Detach(emptyObject, model.transform.GetChild(0).gameObject);
     

      //  emptyObject = Detach(newcubeButton, model.transform.GetChild(0).gameObject);
        newcubeButton.AddComponent<LookAt>();

        // StartCoroutine(AddingLookAt(newcubeButton.gameObject));

        //  newcubeButton.AddComponent<LookAt>();
        Canvas canvas = newcubeButton.GetComponentInChildren<Canvas>();
        canvas.worldCamera = Camera.main;
        Button[] buttons = canvas.GetComponentsInChildren<Button>();

        foreach (Button button in buttons)
        {
            TextMeshProUGUI buttonText = button.GetComponentInChildren<TextMeshProUGUI>();
            if (buttonText != null)
            {
                // Set the text of the TextMeshPro component
                buttonText.text = buttonNumber;
            }
            button.onClick.AddListener(() => OnButtonClick(annotationText, heading));
        }

        Transform line = newcubeButton.transform.Find("LineRender");
        if (line != null)
        {
            GameObject linePrefab = line.gameObject;
            Ir_Testing lineScript = linePrefab.GetComponent<Ir_Testing>();
            lineScript.SetUpPoints(newcubeButton, emptyObject);


        }
        /*  Transform line = newcubeButton.transform.Find("LineRender");
          Transform childTransform = newcubeButton.transform.Find("Point");
          if (line != null)
          {
             GameObject linePrefab= line.gameObject;
             Ir_Testing lineScript = linePrefab.GetComponent<Ir_Testing>();
              lineScript.SetUpPoints(childTransform.gameObject, emptyObject);


          }*/

        tempCube.Add(newcubeButton);
       SettingCubeDistance(newcubeButton);

    }
    /* public void AssignDistance()
     {
         foreach (GameObject cube in tempCube)
         {
             SettingCubeDistance(cube);
         }
         tempCube.Clear();
     }*/
    static int callCount = 0;
    static int alternateCallCount = 0;
    public void SettingCubeDistance(GameObject newcubeButton)
    {
      //  Vector3 direction = (model.transform.position - newcubeButton.transform.position).normalized;
        Vector3 newPosition;
        // float minDistance = 0.2511871f;
        // Vector3 newPosition = newcubeButton.transform.position + newcubeButton.transform.forward * 0.1000000f; // Adjusted line
        if (callCount < 1)
        {
            if (newcubeButton.transform.localPosition.x < 0)
            {
                newPosition = new Vector3(newcubeButton.transform.localPosition.x - 0.2511871f, newcubeButton.transform.localPosition.y, newcubeButton.transform.localPosition.z);
                callCount++;
            }
            else
            {
                if (alternateCallCount < 1)
                {
                    newPosition = new Vector3(newcubeButton.transform.localPosition.x + 0.2511871f, newcubeButton.transform.localPosition.y, newcubeButton.transform.localPosition.z);
                    alternateCallCount++;
                }
                else
                {
                    // Execute this code after the else block is met twice
                    newPosition = new Vector3(newcubeButton.transform.localPosition.x, newcubeButton.transform.localPosition.y + 0.2511871f, newcubeButton.transform.localPosition.z);
                    alternateCallCount = 0; // Reset the alternate counter after executing the new code
                }

            }
        }
        else
        {
            // Execute this code after the condition is met twice
            newPosition = new Vector3(newcubeButton.transform.localPosition.x, newcubeButton.transform.localPosition.y + 0.2511871f, newcubeButton.transform.localPosition.z);
            callCount = 0; // Reset the counter after executing the new codealternateCallCount
           
        }

        newcubeButton.transform.localPosition = newPosition;
        Debug.Log(newcubeButton.transform.position);
        newcubeButton.transform.eulerAngles = Vector3.zero;
        //

      /*  BoxCollider boxCollider = modelWithcollider.GetComponent<BoxCollider>();
        Vector3 closestPoint = boxCollider.ClosestPoint(newcubeButton.transform.position);

        // Check if newcubeButton is inside the collider
        if (boxCollider.bounds.Contains(newcubeButton.transform.position))
        {
            // If inside, calculate the direction from the closest point on the collider to the newcubeButton
            Vector3 moveDirection = (newcubeButton.transform.position - closestPoint).normalized;

            // Move the newcubeButton outside of the collider along the calculated direction
            newcubeButton.transform.position += moveDirection * 0.2f;

            // Optionally, you can reset the rotation of the newcubeButton to zero
            newcubeButton.transform.rotation = Quaternion.identity;

            Debug.Log($"{newcubeButton.name} was inside the collider and has been moved outside.");
        }
        else
        {
            // If outside, still move it a little bit
            Vector3 moveDirection = (newcubeButton.transform.position - closestPoint).normalized;
            newcubeButton.transform.position += moveDirection * 0.2f;

            // Optionally, you can reset the rotation of the newcubeButton to zero
            newcubeButton.transform.rotation = Quaternion.identity;

            Debug.Log($"{newcubeButton.name} was outside the collider and has been moved slightly.");
        }*/
    }
}

