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
    [SerializeField] Button readyBtn;
    [SerializeField] TextMeshProUGUI startMatchBtTxt;
    [SerializeField] Button[] buttons;
    public GameObject currentCharacter;
    public GameObject currentLevel;
    int playersReady;
    private bool playerIsReady;
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
    [SerializeField] GameObject playerSelectionScreen;
    [SerializeField] GameObject mapSelectionScreen;

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
    
    //região destinada a métodos de seleção de personagens e mapas
    #region Character and Map selection Methods

    public void SelectCharacter(GameObject _character)
    {
        currentCharacter = _character;
        readyBtn.interactable = true;
    }

    public void Ready()
    {
        readyBtn.interactable = false;
        photonView.RPC(nameof(ReadyRpc), RpcTarget.MasterClient);
    }

    [PunRPC]
    public void ReadyRpc()
    {
        if (playerIsReady)
        {
            LoadScreen(6);
        }
        else
        {
            playerIsReady = true;
        }
    }

    public void SelectLevel(GameObject _level)
    {
        currentLevel = _level;
        PhotonNetwork.Instantiate(currentLevel.name, Vector2.zero, Quaternion.identity);
        //photonView.RPC(nameof(SelectLvlRpc), RpcTarget.AllBuffered);
        startMatchBtn.interactable = true;
    }

    [PunRPC]
    void SelectLvlRpc()
    {
        PhotonNetwork.Instantiate(currentLevel.name, Vector2.zero, Quaternion.identity);
    }

    public void StartMatch()
    {
        GameManager.instance.StartGame();
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
    }

    #endregion
    


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
        playerSelectionScreen.SetActive(false);
        mapSelectionScreen.SetActive(false);

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
        }
    }

    [PunRPC]
    public void LoadLevel()
    {
        LoadScreen(2);
        SceneManager.LoadScene(sceneName);
    }
}
