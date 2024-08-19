using LibTessDotNet;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;

public class arryMana : MonoBehaviour
{
    // Start is called before the first frame update
    public string element;
    private int[] rewsultArray;
    void Start()
    {
        int[,] matrix = { { 1, 2, 3 }, { 4, 5, 6 } };
      
        //  CheckPalin();
        // Rword();
        //  RveachWord();
        ////   NewStruct struc = new NewStruct();
        //   struc.TargetPosition(2f, 3f);
    }
    int Ysum;
    public void DigitS()
    {
        int Xnum = 123;
       
       while (Xnum > 0)
        {
            Ysum+=Xnum%10;
            Xnum /= 10;
        }
    }
    public void CheckPalin()
    {
        char[] newCharElement=element.ToCharArray();
        System.Array.Reverse(newCharElement);
        string RevElemnt= new string(newCharElement);
        if (System.Array.Equals(element, RevElemnt))
        {
            Debug.Log("its working");
        }

    }
    public  void Rword()
    {
        string[] newRWords = element.Split(' ');
        System.Array.Reverse(newRWords);
        string newRwordString = string.Join(' ', newRWords);
        string[] newString=element.Split(' ');

    }
    public void NewRword()
    {
        string[] newEleent=element.Split(' ');
        foreach (string eleent in newEleent)
        {
            RwordHealper(element);
        }
    }
    public string RwordHealper(string element)
    {
        char[] chars=element.ToCharArray();
        System.Array.Reverse (chars);
        string newstring = new string(chars);
        return newstring;
    }
    public void ExceptionHandler()
    {
        try
        {
            Debug.Log("z positon");

        }
        catch
        {

        }
        finally
        {

        }
    }
    public void RveachWord()
    {
        string[] newRveachWord=element.Split(' ');
       
        for(int i=0;i< newRveachWord.Length;i++)
        {
            newRveachWord[i] = Convertion(newRveachWord[i]);
        }
       // new
    }
    public string Convertion(string word)
    {
        char[] array =word.ToCharArray();
        System.Array.Reverse (array);
        return new string(array);
    }
}

public struct NewStruct
{
    float xPosition;
    float yPosition;

    public float TargetPosition(float x,float y)
    {
        xPosition = x; yPosition = y;
        return xPosition + yPosition;
    }
}
