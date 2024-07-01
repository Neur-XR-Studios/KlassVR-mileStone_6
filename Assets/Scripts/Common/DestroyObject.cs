using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyObject : MonoBehaviour
{
    public GameObject TransitionLoaderPanel;
    // Start is called before the first frame update
    void OnEnable()
    {
        Invoke("LoaderPanelDisable",1);
    }

    // Update is called once per frame
    public void LoaderPanelDisable()
    {
        TransitionLoaderPanel.SetActive(false);
    }
}
