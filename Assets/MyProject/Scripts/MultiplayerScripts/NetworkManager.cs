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

    [Header("Configurações e itens da sala de jogo")]
    //variável para nome da cena/level a ser carregado
    [SerializeField] string sceneName;
    public Button startMatchBtn;
    [SerializeField] TextMeshProUGUI startMatchBtTxt;
    private int playersReady;
    private bool playerTwoIsReady;

    [Header("Telas de menu")]
    //variável para gameobject pai de todas as telas de menu
    public GameObject generalMenusScreen;
    [SerializeField] GameObject lobbyScreen;
    public GameObject loadingScreen;
    public GameObject gameRoomScreen;
    public GameObject waitingScreen;

    [Header("Inputs para inserção de texto")]
    [SerializeField] TMP_InputField nicknameInput;
    [SerializeField] private TMP_InputField roomInput;

    public enum Characters
    {
        Placeholder,
        Keeper
    }

    public Characters selectedCharacter;


    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            generalMenusScreen.SetActive(true);
            loadingScreen.SetActive(false);
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            gameObject.SetActive(false);
        }
    }

    //região destinada a métodos de botões
    #region Button Methods
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
        generalMenusScreen.SetActive(false);
        PhotonNetwork.ConnectUsingSettings();
        PhotonNetwork.LocalPlayer.NickName = nicknameInput.text;
    }

    public void StartMatch()
    {
        if (PhotonNetwork.PlayerList[0] == PhotonNetwork.LocalPlayer)
        {
            if (PhotonNetwork.CurrentRoom.PlayerCount == 2)
            {
                GameManager.instance.StartGame();
            }
        }
        else
        {
            photonView.RPC(nameof(Ready), RpcTarget.MasterClient);
        }        
    }
    #endregion

    //região destinada a métodos do photon para conexão online
    #region Photon methods
    public override void OnConnectedToMaster()
    {
        Debug.Log("On Connected To Master");
        PhotonNetwork.JoinLobby();
    }

    public override void OnJoinedLobby()
    {
        Debug.Log("Joined Lobby");
        loadingScreen.SetActive(false);
        generalMenusScreen.SetActive(true);
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
        gameRoomScreen.SetActive(true);
        startMatchBtn.interactable = false;
        playersReady = 0;
        if (PhotonNetwork.PlayerList[0] == PhotonNetwork.LocalPlayer)
        {
            startMatchBtTxt.text = "Começar partida";
        }
        else 
        { 
            startMatchBtTxt.text = "Pronto";
        }
        loadingScreen.SetActive(false);
        lobbyScreen.SetActive(false);

        /*if (PhotonNetwork.CurrentRoom.PlayerCount == 2)
        {
            photonView.RPC(nameof(LoadLevel), RpcTarget.AllBuffered);

            GameManager.instance.StartGame();
        }*/
        //photonView.RPC("CreatePlayer", PhotonNetwork.LocalPlayer);
    }

    #endregion

    public void SelectedACharacter()
    {
        photonView.RPC(nameof(Ready), RpcTarget.MasterClient);
    }
    
    public void SelectedCharacter()
    {
        photonView.RPC(nameof(Ready), RpcTarget.MasterClient);
    }

    [PunRPC]
    public void Ready()
    {
        if (playersReady == 2)
        {
            startMatchBtn.interactable = true;
        }
        else
        {
            playersReady++;
        }

        /*if (GameManager.instance.playerOnePrefab != null && GameManager.instance.playerTwoPrefab != null)
        {
            if (playerTwoIsReady) startMatchBtn.interactable = true;
        }

        if (PhotonNetwork.PlayerList[0] != PhotonNetwork.LocalPlayer && !playerTwoIsReady) playerTwoIsReady = true;*/
    }

    [PunRPC]
    public void LoadLevel()
    {
        //SceneManager.LoadScene(sceneName);
        waitingScreen.SetActive(false);
        generalMenusScreen.SetActive(false);
        loadingScreen.SetActive(true);
    }
}
