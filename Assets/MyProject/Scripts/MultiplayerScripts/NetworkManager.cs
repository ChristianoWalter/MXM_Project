using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class NetworkManager : MonoBehaviourPunCallbacks
{
    //transformando o script em um singleton
    public static NetworkManager instance;

    [SerializeField] string sceneName;

    [SerializeField] GameObject chatScreen;
    [SerializeField] GameObject menuScreen;
    [SerializeField] GameObject lobbyScreen;
    public GameObject loadingScreen;
    public GameObject waitingScreen;
    
    [SerializeField] TMP_InputField nicknameInput;
    [SerializeField] private TMP_InputField roomInput;


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

    //método utilizado para atrelar ao botão e criar a sala
    public void CreateRoom()
    {
        RoomOptions roomOptions = new RoomOptions { IsVisible = true, MaxPlayers = 2 };
        if (PhotonNetwork.CreateRoom("Sala:" + roomInput.text + " De:" + PhotonNetwork.LocalPlayer.NickName, roomOptions))
        {
            return;
        }
        else
        {
            Debug.Log("Criação de sala falhou, tente novamente!");
        }
    }


    //método para atrelar ao botão e se conectar ao server
    public void PlayGame()
    {
        loadingScreen.SetActive(true);
        menuScreen.SetActive(false);
        PhotonNetwork.ConnectUsingSettings();
        PhotonNetwork.LocalPlayer.NickName = nicknameInput.text;
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("On Connected To Master");
        PhotonNetwork.JoinLobby();
    }

    public override void OnJoinedLobby()
    {
        Debug.Log("Joined Lobby");
        loadingScreen.SetActive(false);
        menuScreen.SetActive(true);
        //PhotonNetwork.JoinRandomRoom();
        //PhotonNetwork.CreateRoom("GameRoom");
    }


    public override void OnCreatedRoom()
    {
        Debug.Log("Created Room");
        loadingScreen.SetActive(true);
    }

    //m�todo chamado ao entrar em uma sala
    public override void OnJoinedRoom()
    {
        Debug.Log("joinedRoom");
        waitingScreen.SetActive(true);
        loadingScreen.SetActive(false);
        lobbyScreen.SetActive(false);

        if (PhotonNetwork.CurrentRoom.PlayerCount == 2)
        {
            photonView.RPC(nameof(LoadLevel), RpcTarget.AllBuffered);

            GameManager.instance.StartGame();
        }
        //photonView.RPC("CreatePlayer", PhotonNetwork.LocalPlayer);
    }

    [PunRPC]
    public void LoadLevel()
    {
        //SceneManager.LoadScene(sceneName);
        waitingScreen.SetActive(false);
        menuScreen.SetActive(false);
        loadingScreen.SetActive(true);
        chatScreen.SetActive(true);
    }
}
