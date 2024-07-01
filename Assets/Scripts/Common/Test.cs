using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class Test : MonoBehaviour
{
    public UnityEvent Click;
    public string SceneName;
    // Start is called before the first frame update
    void Start()
    {
        Invoke("LoadScene", 10f);
    }
    public void LoadScene()
    {
        SceneManager.LoadScene(SceneName);
    }
    public void CLickMe()
    {
        Click.Invoke();
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
