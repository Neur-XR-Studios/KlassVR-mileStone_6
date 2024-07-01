using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using TMPro;

public class InternetSpeedTest : MonoBehaviour
{
    public TextMeshProUGUI speedText; // Reference to the UI Text component

    void Start()
    {
        StartCoroutine(TestInternetSpeed());
    }

    IEnumerator TestInternetSpeed()
    {
        while (true)
        {
            float latency = 0f;
            int pingCount = 4; // Number of pings to send

            for (int i = 0; i < pingCount; i++)
            {
                Ping ping = new Ping("8.8.8.8"); // Google's public DNS server
                float startTime = Time.time;

                while (!ping.isDone)
                {
                    yield return null;
                }

                float pingTime = Time.time - startTime;
                latency += pingTime;

                yield return new WaitForSeconds(1f); // Wait 1 second between pings
            }

            float averageLatency = (latency / pingCount) * 1000; // Average latency in ms

            speedText.text = $"Average Latency: {averageLatency:F2} ms";

            yield return new WaitForSeconds(5f); // Wait for 5 seconds before testing again
        }
    }
}