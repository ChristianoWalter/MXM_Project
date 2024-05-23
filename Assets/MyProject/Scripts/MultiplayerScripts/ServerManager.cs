using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using TMPro;

public class ServerManager : MonoBehaviourPunCallbacks
{
    string roomName;
    public TMP_InputField nameOfRoom;
    public GameObject creatRoomPanel;
    public GameObject roomPanel;
    public TextMeshProUGUI roomNameTxt;
    public GameObject playerObject;

    //m�todo para entrar no server master
    public void ConncectToMaster()
    {
        PhotonNetwork.ConnectUsingSettings();
    }

    //m�todo chamado ao conectar no server master e chamada para entrar no lobby
    public override void OnConnectedToMaster()
    {
        Debug.Log("On Conected To Master");
        PhotonNetwork.JoinLobby();
    }

    //m�todo chamado ao entrar no lobby que ativa o painel do lobby
    public override void OnJoinedLobby()
    {
        Debug.Log("Joined Lobby");
        creatRoomPanel.SetActive(true);
    }

    //m�todo para criar a sala e seus par�metros
    public void CreatRoom()
    {
        roomName = nameOfRoom.text;
        PhotonNetwork.CreateRoom(roomName);
    }

    //M�todo chamado caso falhe em criar a sala
    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        PhotonNetwork.CreateRoom(null);
        Debug.Log("CreatRoomFailed");
    }

    //m�todo chamado ao criar a sala
    public override void OnCreatedRoom()
    {
        Debug.Log("Created Room" + roomName);
        roomPanel.SetActive(true);
        roomNameTxt.text = roomName;
    }

    
}
