using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Raycast : MonoBehaviour
{
    public string objectTag = "CubeButton";

    private void Update()
    {
        // Check if the left mouse button is clicked
        if (Input.GetMouseButtonDown(0))
        {
            // Cast a ray from the camera's position forward
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            // Check if the ray hits an object with the specified tag
            if (Physics.Raycast(ray, out hit))
            {
                // Check if the object has the desired tag
                if (hit.collider.CompareTag(objectTag))
                {
                    // Handle the interaction with the object
                    Debug.Log("Cube clicked!");
                    // Add your button functionality here
                }
            }
        }
    }
}
