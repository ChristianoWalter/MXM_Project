using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class NetworkManager : MonoBehaviourPunCallbacks
{
    [SerializeField] GameObject playerObject;


    private void Start()
    {
        PhotonNetwork.ConnectUsingSettings();
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

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        PhotonNetwork.CreateRoom(null);
        Debug.Log("CreatRoomFailed");
    }

    //m�todo chamado ao entrar em uma sala
    public override void OnJoinedRoom()
    {
        //photonView.RPC("CreatePlayer", PhotonNetwork.LocalPlayer);
    }

    [PunRPC]
    void CreatePlayer()
    {
        Vector3 _pos = new Vector3(Random.Range(-3f, 3f), 2f, Random.Range(-3f, 3f));
        PhotonNetwork.Instantiate(playerObject.name, gameObject.transform.position, Quaternion.identity);
    }
}
