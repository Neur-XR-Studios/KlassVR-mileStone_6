using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class XRCameraRig : MonoBehaviour
{
 
 
    /// <summary>
    /// Always coincides with average of the left and right eye poses.
    /// </summary>
    public Transform centerEyeAnchor;
 
    /// <summary>
    /// Always coincides with the pose of the left hand.
    /// </summary>
    public Transform leftHandAnchor;
    /// <summary>
    /// Always coincides with the pose of the right hand.
    /// </summary>
    public Transform rightHandAnchor;
    /// <summary>
    /// Anchors controller pose to fix offset issues for the left hand.
    /// </summary>
    public Transform leftControllerAnchor;
    /// <summary>
    /// Anchors controller pose to fix offset issues for the right hand.
    /// </summary>
    public Transform rightControllerAnchor;


 

    /// <summary>
    /// Occurs when the eye pose anchors have been set.
    /// </summary>
    public event System.Action<XRCameraRig> UpdatedAnchors;


 

    public void Update()
    {
        if (UpdatedAnchors != null)
        {
            UpdatedAnchors(this);
        }
    }


}
