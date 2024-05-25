using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class LobbyNetwork : MonoBehaviourPunCallbacks
{
    // Start is called before the first frame update
    private void Start()
    {
        
        Debug.Log("Connecting to server...");
        PhotonNetwork.ConnectUsingSettings();

    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("On Connected To Master");
        //PhotonNetwork.playerName = PlayerNetwork.Instance.PlayerName;

        PhotonNetwork.JoinLobby();
    }

    public void OnJoinedLobby()
    {
        Debug.Log("Joined Lobby");
    }
}
