using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.UI;
public class CountdownTimer : MonoBehaviour
{
    public TextMeshProUGUI countdownText;
    public float countdownTime = 10f;
    public UnityEvent Startevent;

    private void Start()
    {
        StartCoroutine(StartCountdown());
    }

    IEnumerator StartCountdown()
    {
        while (countdownTime > 0)
        {
            UpdateUI();
            yield return new WaitForSeconds(1f);
            countdownTime--;
        }

        // Optional: Perform actions when the countdown reaches 0
        Startevent.Invoke();
        gameObject.SetActive(false);
    }

    void UpdateUI()
    {
        countdownText.text = countdownTime.ToString();
    }
}
