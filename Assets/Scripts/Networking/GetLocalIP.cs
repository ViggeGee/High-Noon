using System.Net.Sockets;
using System.Net;
using UnityEngine;

public class GetLocalIP : MonoBehaviour
{
    void Start()
    {
        string localIP = GetLocalIPAddress();
        Debug.Log("Local IP Address: " + localIP);
    }

    public string GetLocalIPAddress()
    {
        string localIP = string.Empty;

        try
        {
            // Get the host name of the machine
            string hostName = Dns.GetHostName();

            // Get the IP address of the machine
            foreach (IPAddress ip in Dns.GetHostAddresses(hostName))
            {
                // Look for the IPv4 address (skipping IPv6)
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    localIP = ip.ToString();
                    break;
                }
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError("Error retrieving IP address: " + e.Message);
        }

        return localIP;
    }
}
