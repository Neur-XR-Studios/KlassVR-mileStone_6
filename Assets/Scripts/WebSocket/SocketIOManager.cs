using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UniJSON;
using TMPro;

public class SocketIOManager : MonoBehaviour
{
    // private SocketIOUnity socket;
    public string serverUrl; // Replace with your server's address
    [Serializable]
    public class DeviceStatus
    {
        public string deviceId;
        public bool isOnline;
    }
    public class SynchDevice
    {
        public string deviceId;
        public bool isCompleted;
    }
    // public TextMeshProUGUI textMeshProUGUI;
}
