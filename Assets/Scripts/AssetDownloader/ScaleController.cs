using UnityEngine;

public class ScaleController : MonoBehaviour
{
    public GameObject referenceCube;
    public bool reachedMaxScale = false; // Bool to track if max scale is reached

    [SerializeField]
    private Vector3 combinedBoundsSize; // Size of the combined bounds

    private Vector3 maxScale;
    private Vector3 currentScale;

    void Start()
    {
        
        referenceCube = GameObject.Find("ScaleLimit");
        maxScale = referenceCube.GetComponent<Renderer>().bounds.size;

        // Calculate the initial combined bounds size
        CalculateCombinedBounds();
    }

    void Update()
    {
        // Check if the current size of the combined bounds exceeds the maximum scale
        if (combinedBoundsSize.x >= 1f)
        {
            // If it does, set the bool to true and store the current scale
            reachedMaxScale = true;
        }
        else
        {
            // If not, set the bool to false and store the current scale
            reachedMaxScale = false;
            currentScale = transform.localScale;
        }

        // Apply the current scale to prevent increasing size if reachedMaxScale is true
        if (reachedMaxScale && transform.localScale.x > currentScale.x)
        {
            //transform.localScale = currentScale;
        }

        // Recalculate the combined bounds size every frame
        CalculateCombinedBounds();
    }

    void CalculateCombinedBounds()
    {
        Bounds combinedBounds = new Bounds(transform.position, Vector3.zero);

        // Iterate through all children to calculate combined bounds
        foreach (Renderer renderer in GetComponentsInChildren<Renderer>())
        {
            combinedBounds.Encapsulate(renderer.bounds);
        }

        // Update the combined bounds size
        combinedBoundsSize = combinedBounds.size;
    }
}
