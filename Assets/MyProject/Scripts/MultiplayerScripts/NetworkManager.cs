using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;
using TMPro;
using System.Linq;

public class NetworkManager : MonoBehaviourPunCallbacks
{
    //transformando o script em um singleton
    public static NetworkManager instance;

    [Header("Configurações e itens da sala de jogo")]
    //variável para nome da cena/level a ser carregado
    [SerializeField] string sceneName;
    [SerializeField] Button startMatchBtn;
    [SerializeField] Button readyBtn;
    [SerializeField] TextMeshProUGUI startMatchBtTxt;
    [SerializeField] Button[] buttons;
    public GameObject currentCharacter;
    public GameObject currentLevel;
    private bool playerIsReady;
    private bool characterSelected;

    [Header("Telas de menu")]
    [SerializeField] GameObject mainMenuScreen;
    [SerializeField] GameObject lobbyScreen;
    [SerializeField] GameObject loadingScreen;
    [SerializeField] GameObject gameRoomScreen;
    [SerializeField] GameObject newRoomScreen;
    [SerializeField] GameObject playerUiScreen;
    [SerializeField] GameObject victoryScreen;
    [SerializeField] GameObject defeatScreen;
    [SerializeField] GameObject playerSelectionScreen;
    [SerializeField] GameObject mapSelectionScreen;
    [SerializeField] GameObject loginScreen;

    [Header("Inputs para inserção de texto")]
    [SerializeField] TMP_InputField nicknameInput;
    [SerializeField] private TMP_InputField roomInput;


    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            LoadScreen(9);
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
        PhotonNetwork.JoinLobby();
        LoadScreen(2);
        //PhotonNetwork.ConnectUsingSettings();
        //PhotonNetwork.LocalPlayer.NickName = nicknameInput.text;
    }

    public void PhotonLogin(string username)
    {
        PhotonNetwork.ConnectUsingSettings();
        PhotonNetwork.LocalPlayer.NickName = username;
    }
    
    //região destinada a métodos de seleção de personagens e mapas
    #region Character and Map selection Methods

    public void SelectCharacter(GameObject _character)
    {
        if (!characterSelected) readyBtn.interactable = true;
        characterSelected = true;
        currentCharacter = _character;
    }

    public void Ready()
    {
        buttons.All(button => button.interactable = false);
        readyBtn.interactable = false;
        photonView.RPC(nameof(ReadyRpc), RpcTarget.AllBuffered);
        ChatBox.instance.SendNotification("está pronto");
    }

    [PunRPC]
    public void ReadyRpc()
    {
        if (playerIsReady)
        {
            if (PhotonNetwork.PlayerList[0] == PhotonNetwork.LocalPlayer) LoadScreen(6);
            playerIsReady = false;
        }
        else
        {
            playerIsReady = true;
        }
    }

    [PunRPC]
    void ReactiveBtn()
    {
        buttons.All(button => button.interactable = true);
        characterSelected = false;
    }
    
    public void SelectLevel(GameObject _level)
    {
        if (currentLevel == null) startMatchBtn.interactable = true;
        currentLevel = _level;
    }
    public void StartMatch()
    {
        photonView.RPC(nameof(ReactiveBtn), RpcTarget.AllBuffered);
        GameManager.instance.StartGame();
    }
    #endregion

    //região destinada a métodos do photon automáticos para conexão online
    #region Photon methods
    public override void OnConnectedToMaster()
    {
        Debug.Log("On Connected To Master");
        LoadScreen(0);
        //PhotonNetwork.JoinLobby();
    }

    public override void OnJoinedLobby()
    {
        Debug.Log("Joined Lobby");
        LoadScreen(1);
    }


    public override void OnCreatedRoom()
    {
        Debug.Log("Created Room");
        LoadScreen(2);
    }

    //m�todo chamado ao entrar em uma sala
    public override void OnJoinedRoom()
    {
        Debug.Log("joinedRoom");
        LoadScreen(3);
        startMatchBtn.interactable = false;
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        base.OnPlayerLeftRoom(otherPlayer);
        if (otherPlayer != PhotonNetwork.LocalPlayer)
        {
            PhotonNetwork.LeaveRoom();
        }
        LoadScreen(1);
    }
    #endregion
    
    //método para seleção e fluxo de telas
    public void LoadScreen(int screenIndex)
    {
        mainMenuScreen.SetActive(false);
        lobbyScreen.SetActive(false);
        loadingScreen.SetActive(false);
        gameRoomScreen.SetActive(false);
        newRoomScreen.SetActive(false);
        playerUiScreen.SetActive(false);
        victoryScreen.SetActive(false);
        defeatScreen.SetActive(false);
        playerSelectionScreen.SetActive(false);
        mapSelectionScreen.SetActive(false);
        loginScreen.SetActive(false);

        switch (screenIndex)
        {
            case 0:
                mainMenuScreen.SetActive(true);
                break;
            case 1:
                lobbyScreen.SetActive(true);
                break;
            case 2:
                loadingScreen.SetActive(true);
                break;
            case 3:
                gameRoomScreen.SetActive(true);
                playerSelectionScreen.SetActive(true);
                break;
            case 4:
                newRoomScreen.SetActive(true); 
                break;
            case 5:
                playerUiScreen.SetActive(true);
                break;
            case 6:
                gameRoomScreen.SetActive(true);
                mapSelectionScreen.SetActive(true);
                break;
            case 7:
                playerUiScreen.SetActive(true);
                victoryScreen.SetActive(true);
                break;
            case 8:
                playerUiScreen.SetActive(true);
                defeatScreen.SetActive(true);
                break;
            case 9:
                loginScreen.SetActive(true);
                break;
        }
    }

    public void QuitRoom()
    {
        PhotonNetwork.LeaveRoom(this);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
