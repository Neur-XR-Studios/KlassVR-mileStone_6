using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnbleErrorPanel : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject nupad;
    public Button enter;
    private void OnEnable()
    {
        StartCoroutine(EnablePanle());
    }
    IEnumerator EnablePanle()
    {
        nupad.SetActive(false);
        yield return new WaitForSeconds(3f);
        gameObject.SetActive(false);
        nupad.SetActive(true);
        enter.enabled = true;
    }
}
