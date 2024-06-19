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
    [SerializeField] Button startMatchBtn;
    [SerializeField] TextMeshProUGUI startMatchBtTxt;
    [SerializeField] Button[] buttons;
    public GameObject currentCharacter;
    private bool playerOneIsReady;
    private bool playerTwoIsReady;

    [Header("Telas de menu")]
    [SerializeField] GameObject mainMenuScreen;
    [SerializeField] GameObject lobbyScreen;
    [SerializeField] GameObject loadingScreen;
    [SerializeField] GameObject gameRoomScreen;
    [SerializeField] GameObject newRoomScreen;
    [SerializeField] GameObject playerUiScreen;
    [SerializeField] GameObject victoryScreen;
    [SerializeField] GameObject defeatScreen;

    [Header("Inputs para inserção de texto")]
    [SerializeField] TMP_InputField nicknameInput;
    [SerializeField] private TMP_InputField roomInput;


    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            LoadScreen(0);
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
        LoadScreen(2);
        PhotonNetwork.ConnectUsingSettings();
        PhotonNetwork.LocalPlayer.NickName = nicknameInput.text;
    }

    public void SelectCharacter(GameObject _character)
    {
        currentCharacter = _character;

        if (PhotonNetwork.PlayerList[0] == PhotonNetwork.LocalPlayer)
        {
            if (playerTwoIsReady) startMatchBtn.interactable = true;
            photonView.RPC(nameof(Ready), RpcTarget.AllBuffered);
        }        
        else startMatchBtn.interactable = true;
    }

    public void StartMatch()
    {
        if (PhotonNetwork.PlayerList[0] == PhotonNetwork.LocalPlayer)
        {
            if (PhotonNetwork.CurrentRoom.PlayerCount == 2)
            {
                //photonView.RPC(nameof(LoadLevel), RpcTarget.AllBuffered);
                GameManager.instance.StartGame();
            }
        }
        else
        {
            startMatchBtn.interactable = false;
            photonView.RPC(nameof(Ready), RpcTarget.AllBuffered);
        }        
    }
    #endregion

    //região destinada a métodos do photon automáticos para conexão online
    #region Photon methods
    public override void OnConnectedToMaster()
    {
        Debug.Log("On Connected To Master");
        PhotonNetwork.JoinLobby();
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
        if (PhotonNetwork.PlayerList[0] == PhotonNetwork.LocalPlayer)
        {
            startMatchBtTxt.text = "Começar partida";
        }
        else 
        { 
            startMatchBtTxt.text = "Pronto";
        }
    }

    #endregion
    

    [PunRPC]
    public void Ready()
    {
        if (playerOneIsReady)
        {
            photonView.RPC(nameof(ActivateBtn), RpcTarget.MasterClient);
        }

        if (PhotonNetwork.PlayerList[0] == PhotonNetwork.LocalPlayer)
        {
            playerOneIsReady = true;
        }
        else
        {
            playerTwoIsReady = true;
        }
    }

    [PunRPC]
    void ActivateBtn()
    {
        startMatchBtn.interactable = true;
    }

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
                break;
            case 4:
                newRoomScreen.SetActive(true); 
                break;
            case 5:
                playerUiScreen.SetActive(true);
                break;
            case 6:
                playerUiScreen.SetActive(true);
                victoryScreen.SetActive(true);
                break;
            case 7:
                playerUiScreen.SetActive(true);
                defeatScreen.SetActive(true);
                break;
        }
    }

    [PunRPC]
    public void LoadLevel()
    {
        LoadScreen(2);
        SceneManager.LoadScene(sceneName);
    }
}
