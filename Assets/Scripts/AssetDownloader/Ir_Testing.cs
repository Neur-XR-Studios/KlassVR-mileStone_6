using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ir_Testing : MonoBehaviour
{
    public Transform[] points;
    [SerializeField] private LineController line;
    public bool isLining;
    private void Start()
    {
       
    }
    public void SetUpPoints(GameObject source, GameObject Destination)
    {

        points[0]= source.transform;
        points[1]= Destination.transform;
        isLining = true;
        line.SetUpLine(points);
    }
}
