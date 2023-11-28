using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Services.Lobbies.Models;
using UnityEngine;

public class LobbyItem : MonoBehaviour
{
    [SerializeField] TMP_Text lobbyNameText, lobbyPlayersText;
    Lobby lobby;
    LobbiesList lobbiesList;
    public void Initialise(LobbiesList lobbiesList, Lobby lobby)
    {
        this.lobbiesList = lobbiesList;
        this.lobby = lobby;

        lobbyNameText.text = lobby.Name;
        lobbyPlayersText.text = $"{lobby.Players.Count}/{lobby.MaxPlayers}";
    }

    public void Join()
    {
        AudioManager.Instance.PlaySfx("Join Game", gameObject);
        lobbiesList.JoinAsync(lobby);
    }
}
