using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;
using TMPro;

public class NetworkManager : MonoBehaviourPunCallbacks
{
    [SerializeField] GameObject playerObject;
    [SerializeField] TMP_InputField nicknameInput;


    private void Start()
    {
        PhotonNetwork.ConnectUsingSettings();
        PhotonNetwork.LocalPlayer.NickName = nicknameInput.text;
        Debug.Log(PhotonNetwork.LocalPlayer.NickName);
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("On Connected To Master");
        PhotonNetwork.JoinLobby();
    }

    public override void OnJoinedLobby()
    {
        Debug.Log("Joined Lobby");
        //PhotonNetwork.JoinRandomRoom();
        //PhotonNetwork.CreateRoom("GameRoom");
    }


    public override void OnCreatedRoom()
    {
        Debug.Log("Created Room");
    }

    //m�todo chamado ao entrar em uma sala
    public override void OnJoinedRoom()
    {
        //photonView.RPC("CreatePlayer", PhotonNetwork.LocalPlayer);
    }
}
