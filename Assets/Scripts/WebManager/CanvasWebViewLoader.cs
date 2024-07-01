using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Vuplex.WebView;

public class CanvasWebViewLoader : MonoBehaviour
{
    public CanvasWebViewPrefab _webViewPrefab;  // Reference to the CanvasWebViewPrefab in the scene


    void Start()
    {
        // Start the setup process
      
    }
    public  async void SetupWebView(string url)
    {
        if (_webViewPrefab == null)
        {
            Debug.LogError("CanvasWebViewPrefab reference is not set.");
            return;
        }

        // Adjust RectTransform to have valid dimensions
      //  SetRectTransform();

        // Wait until the prefab is initialized
        await _webViewPrefab.WaitUntilInitialized();

        // Optionally load an initial URL
        LoadUrlAtRuntime(url);
    }

    private void SetRectTransform()
    {
        var rectTransform = _webViewPrefab.GetComponent<RectTransform>();
        rectTransform.sizeDelta = new Vector2(800, 600); // Set to desired width and height
        rectTransform.anchoredPosition3D = Vector3.zero;
        rectTransform.localScale = Vector3.one;
    }

    public void LoadUrlAtRuntime(string url)
    {
        if (_webViewPrefab.WebView != null)
        {
           _webViewPrefab.WebView.LoadUrl(url);
           
        }
        else
        {
            Debug.LogError("WebView is not initialized yet.");
        }
    }

  
}
