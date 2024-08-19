using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pattern : MonoBehaviour
{
    public string pattern;
    public string element;
    // Start is called before the first frame update
    void Start()
    {
        
    }

   public void Matcher(string pattern,string element)
    {
        int index = 0;
        index=element.IndexOf(pattern);
        while(index != -1)
        {
            index=element.IndexOf(pattern,index+pattern.Length);
        }
    }
        

}
