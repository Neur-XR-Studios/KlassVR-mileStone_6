using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnableTruePanel : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {

        StartCoroutine(EnablePanle());
    }
    IEnumerator EnablePanle()
    {
       
        yield return new WaitForSeconds(3f);
        gameObject.SetActive(false);
      
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
