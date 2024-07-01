using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineRendererController : MonoBehaviour
{
    // Start is called before the first frame update
    private LineRenderer lineRenderer;
    public GameObject destination;
    private bool isActiveLine;
    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
    }
    public void AssignDestinationObject(GameObject destinationObj)
    {
        destination = destinationObj;
        isActiveLine=true;
    }

    // Update is called once per frame
    void Update()
    {
        if(isActiveLine)
        {
          
            lineRenderer.SetPosition(1, destination.transform.localPosition);
        }
       
    }
}
