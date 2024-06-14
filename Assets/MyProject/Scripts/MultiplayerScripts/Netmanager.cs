using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using TMPro;

public class Netmanager : MonoBehaviourPunCallbacks
{
    public static Netmanager instance; //singleton

    public TMP_InputField inputNickname;
    [SerializeField] GameObject menuScreen;
    public GameObject loadingScreen;

    [SerializeField] int playerCount = 2;

    [HideInInspector] public Transform cameraPlayer;


    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            menuScreen.SetActive(true);
            loadingScreen.SetActive(false);
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            gameObject.SetActive(false);
        }
    }

    public void ConnectToPhoton()
    {
        Debug.Log("ConnectToPhoton");
        loadingScreen.SetActive(true);
        menuScreen.SetActive(false);
        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("OnConnectedToMaster");
        PhotonNetwork.JoinLobby();
    }

    public override void OnJoinedLobby()
    {
        Debug.Log("OnJoinedLobby");
        PhotonNetwork.JoinRandomRoom();
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.Log("OnJoinRandomFailed");
        RoomOptions options = new RoomOptions();
        options.MaxPlayers = (byte)playerCount;

        PhotonNetwork.CreateRoom(null, options);
    }

    public override void OnCreatedRoom()
    {
        Debug.Log("OnCreatedRoom");

    }

    public override void OnJoinedRoom()
    {
        Debug.Log("OnJoinedRoom");
        Debug.Log("Playercount: " + PhotonNetwork.CurrentRoom.PlayerCount);
        Debug.Log("Nickname: " + inputNickname.text);

        GameManager.instance.StartGame();

        //photonView.RPC("CreatePlayerAvatar", PhotonNetwork.LocalPlayer);
    }

    
}
