using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Windows;
using System.Linq;

public interface Imovgeble
{
    void Moving();
}
public interface Iattackble
{
    void Attack();
}
public interface Italkable
{
    void Talk();
}
public class ISP : MonoBehaviour
{
    public string input;
    // Start is called before the first frame update//HI AJAI BROOO
    //sir
    void Start()
    {
        string[] parts=input.Split(',');
        char[] chars = new char[parts.Length];
        for(int i=0;i<parts.Length; i++)
        {
            chars[i] = parts[i].Trim()[0];
        }
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
