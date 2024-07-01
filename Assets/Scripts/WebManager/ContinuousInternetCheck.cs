using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class ContinuousInternetCheck : MonoBehaviour
{
    // URL to ping to check for internet connectivity.
    private string testUrl = "http://google.com";
    // Time interval in seconds to check the internet connection.
    private float checkInterval = 10f; // Adjust the interval as needed.
    public GameObject internteCanva;
    void Start()
    {
        // Start checking the internet connection at regular intervals.
        InvokeRepeating(nameof(CheckInternetConnection), 0f, checkInterval);
    }

    private void CheckInternetConnection()
    {
        StartCoroutine(CheckInternetConnectionCoroutine());
    }

    private IEnumerator CheckInternetConnectionCoroutine()
    {
        UnityWebRequest request = new UnityWebRequest(testUrl);
        request.method = UnityWebRequest.kHttpVerbHEAD;

        yield return request.SendWebRequest();

        if (request.error != null)
        {
            Debug.Log("No internet connection: " + request.error);
            internteCanva.SetActive(true);
        }
        else
        {
            Debug.Log("Internet connection is available.");
            internteCanva.SetActive(false);
        }
    }
}
