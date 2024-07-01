using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyUtilityClass : MonoBehaviour
{
    private System.Action customAction;

    public MyUtilityClass(System.Action customAction)
    {
        this.customAction = customAction;
    }

    public void MyReusableFunction(GameObject targetGameObject, bool enable)
    {
        // Perform the custom action
        if (customAction != null)
            customAction.Invoke();

        // Enable or disable the target GameObject
        if (targetGameObject != null)
            targetGameObject.SetActive(enable);
    }
}
