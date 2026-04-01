using UnityEngine;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Collections.Generic;

public class HandReceiver : MonoBehaviour
{
    private UdpClient udpClient;
    private Thread receiveThread;
    public int port = 5052;

    public bool handDetected = false;
    public List<Vector3> landmarks = new List<Vector3>();

    void Start()
    {
        udpClient = new UdpClient(port);
        receiveThread = new Thread(ReceiveData);
        receiveThread.IsBackground = true;
        receiveThread.Start();
        Debug.Log("Esperando datos en puerto " + port);
    }

    void ReceiveData()
    {
        IPEndPoint remoteEP = new IPEndPoint(IPAddress.Any, port);
        while (true)
        {
            try
            {
                byte[] data = udpClient.Receive(ref remoteEP);
                string json = Encoding.UTF8.GetString(data);
                HandData handData = JsonUtility.FromJson<HandData>(json);
                handDetected = handData.detected;
                landmarks.Clear();
                if (handData.landmarks != null)
                {
                    foreach (var lm in handData.landmarks)
                    {
                        landmarks.Add(new Vector3(lm.x, lm.y, lm.z));
                    }
                }
            }
            catch (System.Exception e) { Debug.LogError(e); }
        }
    }

    void OnDestroy()
    {
        receiveThread?.Abort();
        udpClient?.Close();
    }

    [System.Serializable]
    public class HandData
    {
        public bool detected;
        public Landmark[] landmarks;
    }

    [System.Serializable]
    public class Landmark
    {
        public int id;
        public float x;
        public float y;
        public float z;
    }
}