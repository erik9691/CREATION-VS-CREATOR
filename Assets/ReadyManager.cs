using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.UI;
using TMPro;

public class ReadyManager : NetworkBehaviour
{
    [SerializeField] GameObject[] playerCards;
    [SerializeField] Sprite minionSprite, missingSprite;

    public NetworkList<ulong> n_connectedIds;

    private void Awake()
    {
        n_connectedIds = new NetworkList<ulong>();
    }

    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            NetworkManager.Singleton.OnClientDisconnectCallback += RemoveClientId;
            NetworkManager.Singleton.OnClientConnectedCallback += AddClientId;

            foreach (NetworkClient client in NetworkManager.Singleton.ConnectedClientsList)
            {
                AddClientId(client.ClientId);
            }
        }

        n_connectedIds.OnListChanged += RefreshPlayerList;
    }

    public override void OnNetworkDespawn()
    {
        if (IsServer)
        {
            NetworkManager.Singleton.OnClientDisconnectCallback -= RemoveClientId;
            NetworkManager.Singleton.OnClientConnectedCallback -= AddClientId;
        }
    }

    public void ClickStart()
    {
        ServerManager.Instance.StartGame();
    }

    void AddClientId(ulong id)
    {
        n_connectedIds.Add(id);
    }

    void RemoveClientId(ulong id)
    {
        n_connectedIds.Remove(id);
    }

    void RefreshPlayerList(NetworkListEvent<ulong> changeEvent)
    {
        for (int i = 1; i < playerCards.Length; i++)
        {
            if (i < n_connectedIds.Count)
            {
                playerCards[i].GetComponentInChildren<Image>().sprite = minionSprite;
                playerCards[i].GetComponentInChildren<TextMeshProUGUI>().text = "Minion " + n_connectedIds[i];
            }
            else
            {
                playerCards[i].GetComponentInChildren<Image>().sprite = missingSprite;
                playerCards[i].GetComponentInChildren<TextMeshProUGUI>().text = "Missing...";
            }
        }
    }

}
