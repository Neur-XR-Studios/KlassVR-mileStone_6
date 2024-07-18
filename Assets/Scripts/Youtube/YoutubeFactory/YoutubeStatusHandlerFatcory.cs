using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ISession
{
     void StartSession();
      bool hasError(); 
}


public abstract  class YoutubeStatusHandlerFatcory 
{

     public abstract ISession CreateSession();
}
public class YoutubeFactory : YoutubeStatusHandlerFatcory
{
    private Youtubstatustatus videSession;
    public override ISession CreateSession()
    {

        return videSession;

    }
}
