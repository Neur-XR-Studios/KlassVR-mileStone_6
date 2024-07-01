using SimpleJSON;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Networking;
using UnityEngine.UI;


public class annotation360 : MonoBehaviour
{
    public Canvas canvasPrefab;
    public Transform downloadedModelTransform;
    public Transform cubeParentransform;
    private Canvas canvas;
    private List<TMP_InputField> inputFieldList = new List<TMP_InputField>();
    private List<GameObject> lookAtCubess = new List<GameObject>();
    private List<string> objectNameContainer = new List<string>();
    private bool isEditing;
    private string objectName;
    public Button deleteButtonPrefab;
    public GameObject lookAtCube;
  //  public TMP_InputField instantiatedInputField; // Keep this line as it is
    private Transform tempPosition;
    private int objCount;
    public float offsetDistance;
    private List<GameObject> emptyObjectList = new List<GameObject>();
    public GameObject inputFieldParent;
    private Camera Maincamera;
    public Vector3 desiredPosition;
    private TMP_InputField inputfield;
    private Button coreDeleteButton;
    public float rotationSpeed = 5f;
    private bool isVertical;
    private bool isHorizondal;
    private bool isRotating = false;
    //------------------

    private float totalRotation = 0f;
    public float rotationAmount = 90f; // Amount to rotate by each time
                                       // Speed of rotation
    private GameObject Modelchild;
    private List<string> objCountStrings = new List<string>();
    private API apimanager;
    private string json;
    private int rearrangeCount;
    private float lastClickTime = 0f;
    private float doubleClickDelay = 0.2f;
    private List<string> headingText = new List<string>();

    /* [DllImport("__Internal")]
     private static extern void RefreshPage();*/
    private void Start()
    {


        apimanager = FindAnyObjectByType<API>();

        //  jsonString = "";
        objCount = 1;
        InitializeActivity();
      //  AssignValueTOJson("aa");
    }

    public void InitializeActivity()
    {
        GameObject objectsWithTag = GameObject.FindGameObjectWithTag("test");
        downloadedModelTransform = objectsWithTag.transform;
        downloadedModelTransform.transform.SetParent(cubeParentransform);

     //   Transform childTransform = downloadedModelTransform.transform.GetChild(0);

        Modelchild = downloadedModelTransform.gameObject;
        /*Rigidbody modelRig=   Modelchild.AddComponent<Rigidbody>();
        modelRig.useGravity = false;
        modelRig.isKinematic = true;*/
        Collider collider = downloadedModelTransform.GetComponent<Collider>();
        if (!collider)
        {
            // MeshCollider meshCollider = childTransform.AddComponent<MeshCollider>();
            AddMeshColliderRecursively(downloadedModelTransform);
        }
        // Transform childTransform = downloadedModel.transform.GetChild(0);
      //  childTransform.AddComponent<ZoomableObject>();
        Maincamera = Camera.main;
        canvas = Instantiate(canvasPrefab);
        canvas.transform.SetParent(Modelchild.transform);
        SetPositionAndRotation();
        canvas.renderMode = RenderMode.WorldSpace;
        float distance = Vector3.Distance(Maincamera.transform.position, downloadedModelTransform.position);


        // Print the distance to the console
        Debug.Log("Distance from camera to object: " + distance);
        Debug.Log(distance);

    }

    private void AddMeshColliderRecursively(Transform parent)
    {
        if (parent.GetComponent<MeshRenderer>() != null)
        {
            parent.gameObject.AddComponent(typeof(MeshCollider));
        }
        // Iterate through each child of the parent transform
        foreach (Transform child in parent)
        {
            // Check if the child has a MeshRenderer component
            MeshRenderer meshRenderer = child.GetComponent<MeshRenderer>();
            if (meshRenderer != null)
            {
                // If the child has a MeshRenderer, check for a MeshCollider
                MeshCollider meshCollider = child.GetComponent<MeshCollider>();
                if (meshCollider == null)
                {
                    // Explicitly use typeof to add a MeshCollider
                    child.gameObject.AddComponent(typeof(MeshCollider));
                }
            }

            // Recursively call the method for any child objects
            if (child.childCount > 0)
            {
                AddMeshColliderRecursively(child);
            }
        }
    }
    private void SetPositionAndRotation()
    {
        GameObject newObject = new GameObject("NewObject");
        tempPosition = newObject.transform;
        tempPosition.position = downloadedModelTransform.position;
        tempPosition.rotation = downloadedModelTransform.rotation;
    }

    private void SetPositionAndRotationAtTheEnd()
    {
        downloadedModelTransform.position = tempPosition.position;
        downloadedModelTransform.rotation = tempPosition.rotation;
    }

    public void HorizontalRotate(float rotate)
    {
        // Rotate the cube 90 degrees around the y-axis
        //cubeTransform.Rotate(0f, rotate, 0f);
        StartCoroutine(RotateCoroutine(new Vector3(0f, rotate, 0f)));
    }

    public void VerticalRotate(bool rotateUp)
    {
        if (!isRotating) // If the cube is not already rotating
        {
            Vector3 axis = rotateUp ? Vector3.right : -Vector3.right;
            StartCoroutine(Rotate(axis, 90, .2f)); // Rotate 90 degrees around the specified axis
        }
    }
    IEnumerator Rotate(Vector3 axis, float angle, float duration)
    {
        isRotating = true;

        Quaternion from = cubeParentransform.transform.rotation; // Starting rotation
        Quaternion to = cubeParentransform.transform.rotation;
        to *= Quaternion.Euler(axis * angle); // Desired rotation

        float elapsed = 0.0f; // Time since the start of the rotation
        while (elapsed < duration)
        {
            cubeParentransform.transform.rotation = Quaternion.Slerp(from, to, elapsed / duration);
            elapsed += Time.deltaTime; // Update elapsed time
            yield return null; // Wait for the next frame
        }

        cubeParentransform.transform.rotation = to; // Ensure the rotation is exactly the final desired rotation
        isRotating = false; // Rotation is done
    }

    private IEnumerator RotateCoroutine(Vector3 rotation)
    {
        if (!isRotating)
        {
            isRotating = true;
            Quaternion startRotation = downloadedModelTransform.rotation;
            Quaternion targetRotation = Quaternion.Euler(downloadedModelTransform.eulerAngles + rotation);

            float elapsedTime = 0f;

            while (elapsedTime < 1f)
            {
                elapsedTime += Time.deltaTime * rotationSpeed;
                downloadedModelTransform.rotation = Quaternion.Lerp(startRotation, targetRotation, elapsedTime);
                yield return null;
            }
            isRotating = false;
        }

    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.U))
        {
            SendAllToServer();
        }
        if (Input.GetMouseButtonDown(0))
        {
            // Check for double-click
            if (Time.time - lastClickTime < doubleClickDelay)
            {
                // Double-click detected
                OnDoubleClick();
            }
            else
            {
                // Single-click detected, update last click time
                lastClickTime = Time.time;
            }
        }

        // inputfield.transform.position = desiredPosition;
    }
    private void OnDoubleClick()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            Debug.Log("Clicked on: " + hit.collider.gameObject.name);

            if (EventSystem.current.IsPointerOverGameObject())
            {
                return;
            }

            Vector3 mousePosition = hit.point;

            if (!isEditing)
            {
                objectName = hit.collider.gameObject.name;
                //string defaultAnnotationText = "Clicked on: " + objectName;
                string defaultAnnotationText = "Enter your description here.";

                CreateAnnotationInputField(defaultAnnotationText, mousePosition, mousePosition + (hit.normal * offsetDistance));
                // CreateEmptyObjectAtPosition(mousePosition + (hit.normal * offsetDistance));
            }
        }
    }
    private void CreateAnnotationInputField(string text, Vector3 position, Vector3 emptyObjPoeition)
    {


        Vector3 desiredPosition = new(-304f, 36f, -1.083455f);
       // TMP_InputField newInputField = Instantiate(instantiatedInputField, desiredPosition, Quaternion.identity, inputFieldParent.transform);
        /*inputfield = newInputField;
        newInputField.textComponent.fontSize = 20;
        newInputField.textComponent.color = Color.black;
        newInputField.textComponent.enableWordWrapping = true;
        Image imageComponent = newInputField.GetComponentInChildren<Image>();
        coreDeleteButton = newInputField.GetComponentInChildren<Button>();*/
      /*  if (imageComponent != null)

        {
            TMP_Text textMeshProText = imageComponent.GetComponentInChildren<TMP_Text>();

            if (textMeshProText != null)
            {
                textMeshProText.text = objCount.ToString();

            }
            else
            {
                Debug.LogError("TextMeshPro component not found.");
            }

        }
        TMP_Text tmpText = newInputField.textComponent;*/
      /*  tmpText.text = "4";
        RectTransform rectTransform = newInputField.GetComponent<RectTransform>();
        // rectTransform.sizeDelta = new Vector2(407.4f, 402.8f);
        // rectTransform.sizeDelta = new Vector2(200, 30);
        newInputField.transform.localScale = new Vector3(1.29f, 1.29f, 1.29f);*/

        // Assign a unique name to the input field
    /*    newInputField.name = "InputField_" + objCount;

        newInputField.text = text;*/
        string count = objCount.ToString();
        //empty object created
        GameObject emptyObject = new GameObject("EmptyObject" + objCount);
        emptyObject.transform.SetParent(Modelchild.transform);
        emptyObject.transform.position = emptyObjPoeition;
        objectNameContainer.Add(emptyObject.name);
        objCountStrings.Add(count);
        // Add EmptyObject reference to the list




        // Instantiate the delete button near the input field
        // Button deleteButton = Instantiate(deleteButtonPrefab, position /*+ new Vector3(0.2f, 0, 0)*/, Quaternion.identity, canvas.transform);
        GameObject lookAtCubes = Instantiate(lookAtCube, position /*+ new Vector3(0.2f, 0, 0)*/, Quaternion.identity, Modelchild.transform);
        lookAtCubes.AddComponent<LookAt>();
        Transform canvasTransform = lookAtCubes.transform.GetChild(0);
        if (canvasTransform != null)
        {
            // Loop through the children of the Canvas to find TextMeshPro components
            for (int i = 0; i < canvasTransform.childCount; i++)
            {
                // Get the child at index i
                Transform child = canvasTransform.GetChild(i);

                // Check if the child has a TextMeshPro component
                TextMeshProUGUI textMeshPro = child.GetComponent<TextMeshProUGUI>();

                // If the child has a TextMeshPro component and texts array has enough elements

                textMeshPro.text = objCount.ToSafeString();

            }
        }
        else
        {
            Debug.LogError("Canvas not found among the children of the cube!");
        }

        // deleteButton.onClick.AddListener(() => DeleteInputField(newInputField, deleteButton, emptyObject));
     //   coreDeleteButton.onClick.AddListener(() => DeleteInputField(newInputField, lookAtCubes, emptyObject, count));
        // deleteButton.AddComponent<LookAt>();
        // Store input field reference in the list
      //  inputFieldList.Add(newInputField);
        lookAtCubess.Add(lookAtCubes);
        emptyObjectList.Add(emptyObject);

        // Increment the object count for unique names
        objCount++;
    }
    private Transform FindCanvas(Transform parent)
    {
        // Loop through the children of the parent GameObject
        foreach (Transform child in parent)
        {
            // Check if the child's name is "Canvas"
            if (child.name == "Canvas")
            {
                // Return the transform of the Canvas GameObject
                return child;
            }
        }

        // Return null if Canvas not found
        return null;
    }
    private void DeleteInputField(TMP_InputField inputField, GameObject lookA, GameObject emptyObj, string count)
    {
        int index = inputFieldList.IndexOf(inputField); // Get the index of the input field

        // Remove the input field and related objects
        inputFieldList.Remove(inputField);
        emptyObjectList.Remove(emptyObj);

        lookAtCubess.Remove(lookA);
        Destroy(inputField.gameObject);
        Destroy(lookA);
        Destroy(emptyObj.gameObject);
        // canvasClickHandler.IsObjectEnabled = false;
        // Rearrange the count values for all input fields
        for (int i = 0; i < inputFieldList.Count; i++)
        {

            inputFieldList[i].name = "InputField_" + i; // Update the name with the new count value
            TMP_Text textMeshPro = inputFieldList[i].GetComponentInChildren<TMP_Text>();
            if (textMeshPro != null)
            {
                rearrangeCount = i + 1;
                RearrangeNumberingForLookAtCube(lookAtCubess[i], rearrangeCount);
                textMeshPro.text = rearrangeCount.ToString(); // Update the displayed count number
            }
        }
        objCount = rearrangeCount + 1;
    }
    public void RearrangeCount()
    {

    }
    // Call this function to send all input field data to the server
    public void SendAllToServer()
    {
        // downloadedModelTransform.SetParent(null);
        //objCount = 1;
        SetPositionAndRotationAtTheEnd();


        for (int i = 0; i < 3; i++)
        {
           // TMP_InputField inputField = inputFieldList[i];
          //  Transform childTransform = inputField.transform.Find("heading");


           /* if (childTransform != null)
            {
                TMP_InputField headingInputField = childTransform.GetComponent<TMP_InputField>();
                headingText.Add(headingInputField.text);
            }
            else
            {

                Debug.Log("Child object not found!");
            }*/
            GameObject emptyObject = emptyObjectList[i];
            string count = objCountStrings[i];
           /* string uniqueName = inputField.name; // Use the stored unique name of the input field
            string annotationText = inputField.text;*/

            // Extract position from the associated empty object
            Vector3 position = new Vector3(emptyObject.transform.localPosition.x, emptyObject.transform.localPosition.y, emptyObject.transform.localPosition.z);

            SendDataToServer( position,  count);
        }
      //  SerializeAnnotationsToJson();
    }

    public void RearrangeNumberingForLookAtCube(GameObject lookAtCubes, int count)
    {
        Transform canvasTransform = lookAtCubes.transform.GetChild(0);
        if (canvasTransform != null)
        {
            // Loop through the children of the Canvas to find TextMeshPro components
            for (int i = 0; i < canvasTransform.childCount; i++)
            {
                // Get the child at index i
                Transform child = canvasTransform.GetChild(i);

                // Check if the child has a TextMeshPro component
                TextMeshProUGUI textMeshPro = child.GetComponent<TextMeshProUGUI>();

                // If the child has a TextMeshPro component and texts array has enough elements

                textMeshPro.text = count.ToSafeString();

            }
        }
        else
        {
            Debug.LogError("Canvas not found among the children of the cube!");
        }
    }
    private void SendDataToServer( Vector3 position, string count)
    {

        // Implement your server communication logic here
        Debug.Log($" AnnotationPosition: {position}" +
            $"ObjectName: {objectName}, ModelPosition: {tempPosition.position}{tempPosition.rotation}," +
           "ObjectCount:{count}, heading: {heading}");

    }
    /*public void SerializeAnnotationsToJson()
    {
        int i = 0;
        // Example list of annotations for demonstration. You would fill this list dynamically based on your application's data.
        List<Annotation> annotations = new List<Annotation>();

        // Example of dynamically adding annotations. Replace these with your actual data gathering logic.
        foreach (TMP_InputField inputField in inputFieldList)
        {
            GameObject emptyObject = emptyObjectList[i];
            string count = objCountStrings[i];
            string heading = headingText[i];

            // Example data. You should replace these with the actual data from your application.
            string uniqueName = inputField.name;
            string text = inputField.text;
            // Assuming you have a way to get these positions and rotations for each annotation
            Vector3 position = new Vector3(emptyObject.transform.localPosition.x, emptyObject.transform.localPosition.y, emptyObject.transform.localPosition.z);
            Quaternion rotation = Quaternion.identity; // Example rotation

            // Create and add the annotation
            annotations.Add(new Annotation
            {
                AnnotationUniqueName = uniqueName,
                AnnotationText = text,
                AnnotationPosition = new PositionRotation(position.x, position.y, position.z),
                AnnotationRotation = new PositionRotation(rotation.x, rotation.y, rotation.z, rotation.w),
                ObjectName = PlayerPrefs.GetString("ContentId"), // You need to replace this with actual data
                ModelPosition = new ModelPosition(new PositionRotation(0, 0, 0), new PositionRotation(0, 0, 0, 1)),                             // ModelPosition = new ModelPosition(new PositionRotation(0, 0, 0), new PositionRotation(0, 0, 0, 1)),
                CanvasRectWidth = 1118, // Example data
                CanvasRectHeight = 705, // Example data
                CanvasRectPosX = -559, // Example data
                CanvasRectPosY = -352.5f, // Example data
                ObjectCount = int.Parse(objCountStrings[i]), // Assuming this is dynamically updated in your script
                heading = heading

            });
            i++;
        }

        // Convert the list to an array since JsonUtility does not support top-level lists
        AnnotationsList annotationsList = new AnnotationsList { annotations = annotations.ToArray() };

        // Serialize to JSON
        string json = JsonUtility.ToJson(annotationsList, true);
        Debug.Log(json);
       
        // Here you can use the json string, e.g., send it to a server or save it locally
    }*/

    public void ReDirectURl()
    {

        Application.ExternalEval("window.open('" + "https://3.90.103.82/experience" + "','_self')");
        /*#if !UNITY_EDITOR && UNITY_WEBGL
                RefreshPage();
        #endif*/
    }
    IEnumerator ReDirctingURL()
    {
        string originalUrl = "https://3.90.103.82/experience";

        // Request the original URL
        UnityWebRequest www = UnityWebRequest.Get(originalUrl);

        // Wait for the request to complete
        yield return www.SendWebRequest();

        // Check for errors
        if (www.isNetworkError || www.isHttpError)
        {
            Debug.Log("Error: " + www.error);
        }
        else
        {
            // Get the redirected URL
            string redirectUrl = www.url;

            // Log the redirected URL
            Debug.Log("Redirected URL: " + redirectUrl);

            // Open the redirected URL in the browser
            Application.OpenURL(redirectUrl);
        }
    }



    #region Storing Previous annotations

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
    public string jsonString;
    private GameObject downloadedModel;
    private string downlodedModelName;



    public void DownloadPreviousAnnotationValues()
    {
        GameObject objectsWithTag = GameObject.FindGameObjectWithTag("test");
        
        downloadedModel = objectsWithTag.gameObject;
        AssignPosithionRotationAndCreateCanvas();

    }
    public void AssignValueTOJson(string json)
    {
        /*jsonString = json;*/
        DownloadPreviousAnnotationValues();
    }
    public void InstantiateInputFeild(string newText, string cout, GameObject cube, GameObject emptyObj)
    {
        Vector3 desiredPosition = new(-304f, 36f, -1.083455f);
       // TMP_InputField newInputField = Instantiate(instantiatedInputField, desiredPosition, Quaternion.identity, inputFieldParent.transform);
      /*  inputfield = newInputField;
        newInputField.textComponent.fontSize = 20;
        newInputField.textComponent.color = Color.black;
        newInputField.textComponent.enableWordWrapping = true;*/
      /*  Image imageComponent = newInputField.GetComponentInChildren<Image>();
        coreDeleteButton = newInputField.GetComponentInChildren<Button>();*/
      /*  if (imageComponent != null)

        {
            TMP_Text textMeshProText = imageComponent.GetComponentInChildren<TMP_Text>();

            if (textMeshProText != null)
            {
                textMeshProText.text = objCount.ToString();

            }
            else
            {
                Debug.LogError("TextMeshPro component not found.");
            }

        }*/
      /*  TMP_Text tmpText = newInputField.textComponent;
        tmpText.text = "4";
        RectTransform rectTransform = newInputField.GetComponent<RectTransform>();*/
        // rectTransform.sizeDelta = new Vector2(407.4f, 402.8f);
        // rectTransform.sizeDelta = new Vector2(200, 30);
      /*  newInputField.transform.localScale = new Vector3(1.29f, 1.29f, 1.29f);

        // Assign a unique name to the input field
        newInputField.name = "InputField_" + objCount;

        newInputField.text = newText;*/
        string count = objCount.ToString();
        objCountStrings.Add(count);
      //  coreDeleteButton.onClick.AddListener(() => DeleteInputField(newInputField, cube, emptyObj, count));
     //   inputFieldList.Add(newInputField);
        lookAtCubess.Add(cube);
        emptyObjectList.Add(emptyObj);
        objCount++;
    }
    public void AssignPosithionRotationAndCreateCanvas()
    {
        // Parse the JSON string into a JSON array
        JSONNode jsonData = JSON.Parse(jsonString);

        if (jsonData == null)
            return;

        List<AnnotationData> data = new List<AnnotationData>();

        // Iterate over each element in the JSON array
        for (int i = 0; i < jsonData.Count; i++)
        {
            JSONNode annotationNode = jsonData[i];

            // Create an AnnotationData object and populate its fields
            AnnotationData annotation = new AnnotationData();
            annotation.UniqueName = annotationNode["UniqueName"];
            annotation.ObjectName = annotationNode["ObjectName"];
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
        }



        foreach (var annotation in data)
        {
            if (annotation.ObjectName == downlodedModelName)
            {
                downloadedModel.transform.localPosition = annotation.ModelPosition.position;
                downloadedModel.transform.localRotation = annotation.ModelPosition.rotation;







                //  newText.transform.position = new Vector3(annotation.AnnotationPosition.x, annotation.AnnotationPosition.y, annotation.AnnotationPosition.z);
                CreateEmptyObjectAtPosition(new Vector3(annotation.AnnotationPosition.x, annotation.AnnotationPosition.y, annotation.AnnotationPosition.z), annotation.AnnotationRotation, annotation.AnnotationText, annotation.annotationCount);





            }
        }
    }


    private void CreateEmptyObjectAtPosition(Vector3 annotationPosition, Quaternion rotation, string annotationText, string buttonNumber)
    {



        GameObject emptyObject = new GameObject("EmptyObject");
        emptyObject.transform.SetParent(downloadedModel.transform);
        emptyObject.transform.position = annotationPosition;
        //     newText.transform.rotation = emptyObject.transform.rotation;
        GameObject newcubeButton = Instantiate(lookAtCube, downloadedModel.transform);
        newcubeButton.transform.rotation = emptyObject.transform.rotation;
        newcubeButton.transform.position = emptyObject.transform.position;
        Transform canvasTransform = newcubeButton.transform.GetChild(0);
        if (canvasTransform != null)
        {
            // Loop through the children of the Canvas to find TextMeshPro components
            for (int i = 0; i < canvasTransform.childCount; i++)
            {
                // Get the child at index i
                Transform child = canvasTransform.GetChild(i);

                // Check if the child has a TextMeshPro component
                TextMeshProUGUI textMeshPro = child.GetComponent<TextMeshProUGUI>();

                // If the child has a TextMeshPro component and texts array has enough elements

                textMeshPro.text = objCount.ToString();

            }
        }
        else
        {
            Debug.LogError("Canvas not found among the children of the cube!");
        }

        //  SettingCubeDistance(newcubeButton);
        newcubeButton.AddComponent<LookAt>();
        // StartCoroutine(AddingLookAt(newcubeButton.gameObject));

        InstantiateInputFeild(annotationText, buttonNumber, newcubeButton, emptyObject);

    }
    public void SettingCubeDistance(GameObject newcubeButton)
    {

        Vector3 direction = (downloadedModel.transform.position - newcubeButton.transform.position).normalized;
        Vector3 newPosition;
        if (newcubeButton.transform.position.z < 0)
        {
            newPosition = new Vector3(newcubeButton.transform.position.x, newcubeButton.transform.position.y, newcubeButton.transform.position.z - 0.2511871f);
        }
        else
        {
            newPosition = new Vector3(newcubeButton.transform.position.x, newcubeButton.transform.position.y, newcubeButton.transform.position.z + 0.2511871f);
        }

        newcubeButton.transform.position = newPosition;
        Debug.Log(newcubeButton.name + " position adjusted to move to the front.");
        newcubeButton.transform.eulerAngles = Vector3.zero;
    }

    #endregion
}