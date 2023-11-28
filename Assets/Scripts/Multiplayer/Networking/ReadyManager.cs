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
    [SerializeField] GameObject[] minionObjects;
    [SerializeField] TMP_Text joinCodeText;

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
            joinCodeText.text = HostManager.Instance.JoinCode;
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
        HostManager.Instance.StartGame();
        gameObject.GetComponent<NetworkObject>().Despawn();
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
        AudioManager.Instance.PlaySfx("Join Game", gameObject);
        for (int i = 1; i < playerCards.Length; i++)
        {
            if (i < n_connectedIds.Count)
            {
                playerCards[i].GetComponentInChildren<Image>().sprite = minionSprite;
                playerCards[i].GetComponentInChildren<TextMeshProUGUI>().text = "Minion " + n_connectedIds[i];
                minionObjects[i-1].SetActive(true);
                minionObjects[i-1].GetComponent<Animator>().Play("stumble " + i);
            }
            else
            {
                playerCards[i].GetComponentInChildren<Image>().sprite = missingSprite;
                playerCards[i].GetComponentInChildren<TextMeshProUGUI>().text = "Missing...";
                minionObjects[i-1].SetActive(false);
            }
        }
    }

}
