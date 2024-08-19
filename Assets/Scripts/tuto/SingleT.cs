using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SingleT : MonoBehaviour
{
    public static SingleT singlrTone;
    // Start is called before the first frame update
    void Awake()
    {
        if (singlrTone == null)
        {
            GameObject gameObject = new GameObject();
            gameObject.AddComponent<SingleT>();
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(singlrTone);
        }
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
