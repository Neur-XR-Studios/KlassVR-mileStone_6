using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ISession
{
     void StartSession();
      bool hasError(); 
}


public abstract  class YoutubeStatusHandler 
{

     public abstract ISession CreateSession();
}
