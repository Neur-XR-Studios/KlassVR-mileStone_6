using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public  class RoomController: MonoBehaviour
{
    public virtual void ShowClassRomm(GameObject theClassTobeDisplay)
    {
        Debug.Log("here we display room");
    }
}
public class ClassRoomOne: RoomController
{
    public override void ShowClassRomm(GameObject classOne)
    {
        classOne.SetActive(true);
    }

}
public class ClassRoomTwo : RoomController
{
    public override void ShowClassRomm(GameObject classOne)
    {
        classOne.SetActive(true);
    }

}
public class ClassRoomThree : RoomController
{
    public override void ShowClassRomm(GameObject classOne)
    {
        classOne.SetActive(true);
    }

}
public class ChangeClassRoom : MonoBehaviour
{
    public GameObject[] classRooms; 
    // Start is called before the first frame update
    void Start()
    {
        
    }
    public void AssignClassRoom(string roomDetails)
    {
        RoomController classRoom1= new ClassRoomOne();
        RoomController classRoom2= new ClassRoomOne();
        RoomController classRoom3= new ClassRoomOne();
        IEnumerator enumerator= classRooms.GetEnumerator();
       while(enumerator.MoveNext())
        {
           GameObject currentRoom= (GameObject)enumerator.Current;
            currentRoom.SetActive(false);
        }
        switch (roomDetails)
        {
            case "classEnvironment 1":
                classRoom1.ShowClassRomm(classRooms[0]);
                break;
            case "classEnvironment 2":
                classRoom2.ShowClassRomm(classRooms[1]);
                break;
            case "classEnvironment 3":
                classRoom3.ShowClassRomm(classRooms[2]);
                break;
            
            default:
                print("Incorrect intelligence level.");
                break;
        }
        


    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
