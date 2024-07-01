using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ButtonColorChanger : MonoBehaviour
{
    public Button button;
    public Color defaultColor;
    public Color outputColor;
    private void Start()
    {
        // Save the default color of the button
        //defaultColor = button.colors.normalColor;
    }

    public void OnButtonClick()
    {
        defaultColor = button.colors.normalColor;
        Color newColor = outputColor;
        newColor.a = button.image.color.a; 
        button.image.color = newColor;
        StartCoroutine(ResetButtonColorAfterDelay());
    }

    private IEnumerator ResetButtonColorAfterDelay()
    {
        //// Wait for 1 second
        yield return new WaitForSeconds(1f);

        //// Reset the color of the button to default
        button.image.color = defaultColor;
    }
}
