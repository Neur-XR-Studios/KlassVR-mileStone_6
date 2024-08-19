using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public interface IDataService
{
    void SaveData();
    void LoadData();
}
public interface IloggerService
{
    void Log(string message);
}
public interface IInputService
{
    float GetAxis(string axisName);
}
public class ServiceHandler: IDataService
{
    public void SaveData()
    {
        Debug.Log("Save Data");
    }
    public void LoadData()
    {
        Debug.Log("Load data");
    }

}
public class inputServiceHandler: IInputService
{
    public float GetAxis(string axisName)
    {
        return 0;
    }
}
public class LogHandler: IloggerService
{
   public void Log(string message)
    {
        Debug.Log(message);

    }
}
public class Dependency : MonoBehaviour
{
    private IDataService dataService;
    private IDataService IloggerService;
    private IDataService IDataService;
    // Start is called before the first frame update
    void Start()
    {
        dataService.SaveData();
    }
    public  Dependency(IDataService dataService, IDataService IloggerService)
    {
        this.dataService = dataService;
        this.IloggerService = IloggerService;
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
