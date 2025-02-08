using System.Net.Sockets;
using System.Net;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode.Transports.UTP;
using TMPro;

public class NetworkManagerUI : MonoBehaviour
{
    [SerializeField] private GameObject networkCanvas;
    [SerializeField] private Button hostButton;
    [SerializeField] private Button clientButton;
    [SerializeField] private TextMeshProUGUI IPInputField;
    [SerializeField] private TextMeshProUGUI PlaceholderTextInputField;
    [SerializeField] private bool useLANConnection = false;

    private PlayerJoined playerJoined;

    private void Awake()
    {
        hostButton.onClick.AddListener(() =>
        {
            if(useLANConnection)
            {
                // Dynamically set the network address before starting the client
                string hostIP = GetLocalIPAddress(); // Or a specific IP address if you're connecting to a remote host
                UnityTransport unityTransport = NetworkManager.Singleton.GetComponent<UnityTransport>();

                unityTransport.SetConnectionData(hostIP, 7777);
                hostButton.GetComponentInChildren<TextMeshProUGUI>().text = hostIP;

                NetworkManager.Singleton.StartHost();
            }
            else
            {
                NetworkManager.Singleton.StartHost();
                networkCanvas.SetActive(false);
            }

            // Start as the host

        });

        clientButton.onClick.AddListener(() =>
        {
            if (useLANConnection)
            {
                string ipText = IPInputField.text.Trim();
                ipText = ipText.Replace("\u200B", "").Replace("\u200C", "").Replace("\u200D", "").Replace("\uFEFF", "");

                if (string.IsNullOrEmpty(ipText))
                {
                    PlaceholderTextInputField.text = "You need to enter IP";
                }
                else
                {
                    UnityTransport unityTransport = NetworkManager.Singleton.GetComponent<UnityTransport>();
                    unityTransport.SetConnectionData(ipText, 7777);

                    // Start as a client
                    NetworkManager.Singleton.StartClient();
                    networkCanvas.SetActive(false);
                }



            }
            else
            {
                NetworkManager.Singleton.StartClient();
                networkCanvas.SetActive(false);
            }

            
            
        });

        //hostButton.gameObject.SetActive(false);
        //clientButton.gameObject.SetActive(false);
        //IPInputField.transform.parent.parent.gameObject.SetActive(false);
    }

    private void Update()
    {
        if(PlayerManager.Instance.PlayersJoined == 2)
        {
            gameObject.SetActive(false);
        }
        if(playerJoined == null)
        {
            playerJoined = FindAnyObjectByType<PlayerJoined>();
        }
        else
        {
            if(playerJoined.GetIsPlayerHost())
            {
                hostButton.gameObject.SetActive(true);
            }
            else
            {
                clientButton.gameObject.SetActive(true);
                IPInputField.transform.parent.parent.gameObject.SetActive(true);
            }
        }
    }
    string GetLocalIPAddress()
    {
        string localIP = string.Empty;
        try
        {
            string hostName = Dns.GetHostName();
            foreach (IPAddress ip in Dns.GetHostAddresses(hostName))
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork) // Check for IPv4 address
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
