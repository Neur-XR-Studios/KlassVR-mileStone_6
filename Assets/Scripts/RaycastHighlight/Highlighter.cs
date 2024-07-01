using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Highlighter : MonoBehaviour
{
    public Transform rayOrigin; // Origin of the ray
    public float maxDistance = 10f; // Maximum distance the ray can travel
    public Material highlightMaterial; // Material used for highlighting cubes

    private Material originalMaterial; // Store the original material of the cube
    private RaycastHit previousHit; // Store the previous hit to reset material

    void Update()
    {
        // Perform raycasting
        if (Physics.Raycast(rayOrigin.position, rayOrigin.forward, out RaycastHit hit, maxDistance))
        {
            // Check if the ray hits a cube
            if (hit.collider.CompareTag("test"))
            {
                // Highlight the cube by changing its material
                MeshRenderer cubeRenderer = hit.collider.GetComponent<MeshRenderer>();
                if (cubeRenderer != null)
                {
                    // Store the original material
                    if (cubeRenderer.material != highlightMaterial)
                    {
                        originalMaterial = cubeRenderer.material;
                    }

                    // Apply highlight material
                    cubeRenderer.material = highlightMaterial;

                    // Reset the previous hit material
                    if (previousHit.collider != null && previousHit.collider.CompareTag("test") && previousHit.collider != hit.collider)
                    {
                        MeshRenderer previousCubeRenderer = previousHit.collider.GetComponent<MeshRenderer>();
                        if (previousCubeRenderer != null)
                        {
                            previousCubeRenderer.material = originalMaterial;
                        }
                    }

                    // Update the previous hit
                    previousHit = hit;
                }
            }
            else
            {
                // If the ray hits something other than the cube, reset the previous hit material
                if (previousHit.collider != null && previousHit.collider.CompareTag("test"))
                {
                    MeshRenderer previousCubeRenderer = previousHit.collider.GetComponent<MeshRenderer>();
                    if (previousCubeRenderer != null)
                    {
                        previousCubeRenderer.material = originalMaterial;
                    }
                }
            }
        }
        else
        {
            // If the ray doesn't hit anything, reset the previous hit material
            if (previousHit.collider != null && previousHit.collider.CompareTag("test"))
            {
                MeshRenderer previousCubeRenderer = previousHit.collider.GetComponent<MeshRenderer>();
                if (previousCubeRenderer != null)
                {
                    previousCubeRenderer.material = originalMaterial;
                }
            }
        }
    }
}
