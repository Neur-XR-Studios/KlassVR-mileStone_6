
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FPSCounter : MonoBehaviour
{
    public float updateInterval = 0.5f; // Update interval in seconds
    private float lastInterval; // Last interval end time
    private int frames = 0; // Frames over current interval
    private float fps; // Frames per second
    public TextMeshProUGUI fpsText;
   
    private void Start()
    {
        lastInterval = Time.realtimeSinceStartup;
        frames = 0;
       
       
    }
   
    private void Update()
    {
        ++frames;

        // Check if update interval has passed
        if (Time.realtimeSinceStartup > lastInterval + updateInterval)
        {
            // Calculate frames per second
            fps = frames / (Time.realtimeSinceStartup - lastInterval);
            frames = 0;
            lastInterval = Time.realtimeSinceStartup;

            // Display the FPS in the console
           // Debug.Log("FPS: " + fps);
            fpsText.text= fps.ToString();
        }
    }
}
