using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.SceneManagement;
using Unity.VisualScripting;
using UnityEngine.UI;
using TMPro;

public class GameManager : MonoBehaviourPunCallbacks
{
    public static GameManager instance;

    [Header("Objetos do Player")]
    GameObject level;
    [SerializeField] public GameObject playerPrefab;
    [SerializeField] public GameObject playerOnePrefab;
    [SerializeField] public GameObject playerTwoPrefab;
    public HealthBar healthBarOne;
    public HealthBar healthBarTwo;
    public EnergyBar energyBarOne;
    public EnergyBar energyBarTwo;

    [Header("Configurações da partida")]
    [SerializeField] TextMeshProUGUI gameTimeTxt;
    [SerializeField] TextMeshProUGUI timeToStartTxt;
    [SerializeField] float timeToEnd;
    [SerializeField] float timeToStart = 4;
    [SerializeField] Button playAgainVicBtn;
    [SerializeField] Button playAgainDefBtn;
    bool playAgain;
    bool gameStarted;
    bool gameFinished;
    bool youLose;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Update()
    {
        RunGameTime();
    }


    void RunGameTime()
    {
        if (gameStarted && !gameFinished)
        {
            if (timeToStart != 0)
            {
                timeToStart = Mathf.Max(timeToStart - Time.deltaTime, 0);
                timeToStartTxt.text = ((int)timeToStart).ToString("0");
            }
            else
            {
                if (timeToStartTxt.gameObject.activeSelf)
                {
                    if (PhotonNetwork.PlayerList[0] == PhotonNetwork.LocalPlayer)
                    {
                        playerOnePrefab.GetComponent<PlayerController>().isInMatch = true;
                    }
                    else
                    {
                        playerTwoPrefab.GetComponent<PlayerController>().isInMatch = true;
                    }
                    timeToStartTxt.gameObject.SetActive(false);
                }
                else
                {
                    if (timeToEnd != 0)
                    {
                        timeToEnd = Mathf.Max(timeToEnd - Time.deltaTime, 0);
                    }
                    else
                    {
                        CheckHealth();
                    }
                    gameTimeTxt.text = ((int)timeToEnd).ToString("00");
                }
            }
        }
    }

    public void PlayAgain()
    {
        photonView.RPC(nameof(PlayAgainRpc), RpcTarget.AllBuffered);
    }

    [PunRPC]
    void PlayAgainRpc()
    {
        if (playAgain)
        {
            if (playerOnePrefab != null) Destroy(playerOnePrefab);

            if (playerTwoPrefab != null) Destroy(playerTwoPrefab);

            if (level != null) Destroy(level);

            NetworkManager.instance.LoadScreen(3);
            timeToStart = 4;
            gameStarted = false;
            gameFinished = false;
            timeToStartTxt.gameObject.SetActive(true);
            playAgain = false;
            youLose = false;
        }
        else playAgain = true;
    }

    public void StartGame()
    {
        if (PhotonNetwork.CurrentRoom.PlayerCount == 2)
        {
            photonView.RPC(nameof(LoadLevel), RpcTarget.AllBuffered);
            level = PhotonNetwork.InstantiateRoomObject(NetworkManager.instance.currentLevel.name, Vector2.zero, Quaternion.identity);
            photonView.RPC(nameof(CreatePlayer), RpcTarget.AllBuffered);
        }
    }
    
    void CheckHealth()
    {
        if (PhotonNetwork.PlayerList[0] == PhotonNetwork.LocalPlayer)
        {
            if(playerOnePrefab.GetComponent<PlayerController>().currentHealth < playerTwoPrefab.GetComponent<PlayerController>().currentHealth) GameOver();
        }
        else
        {
            if (playerOnePrefab.GetComponent<PlayerController>().currentHealth > playerTwoPrefab.GetComponent<PlayerController>().currentHealth) GameOver();
        }
    }

    public void GameOver()
    {
        youLose = true;
        photonView.RPC(nameof(EndGame), RpcTarget.AllBuffered);
    }

    [PunRPC]
    void EndGame()
    {
        gameFinished = true;

        if (youLose)
        { 
            PlayfabManager.instance.GetUserData(0, 1);
            NetworkManager.instance.LoadScreen(8);
        }
        else
        {
            PlayfabManager.instance.GetUserData(1, 0);
            NetworkManager.instance.LoadScreen(7);
        }

        if (PhotonNetwork.PlayerList[0] == PhotonNetwork.LocalPlayer)
        {
            playerOnePrefab.GetComponent<PlayerController>().isInMatch = false;
        }
        else
        {
            playerTwoPrefab.GetComponent<PlayerController>().isInMatch = false;
        }
    }
    
    [PunRPC]
    void LoadLevel()
    {
        NetworkManager.instance.LoadScreen(5);
    }

    [PunRPC]
    void CreatePlayer()
    {
        gameStarted = true;
        playerPrefab = NetworkManager.instance.currentCharacter;

        if (PhotonNetwork.PlayerList[0] == PhotonNetwork.LocalPlayer)
        {
            Vector2 playerPosition = new Vector2(-2f, 0f);
            playerOnePrefab = PhotonNetwork.Instantiate(playerPrefab.name, playerPosition, Quaternion.identity);
            photonView.RPC(nameof(SetPlayerOne), RpcTarget.AllBuffered, playerOnePrefab.GetComponent<PhotonView>().ViewID);
        }
        else
        {
            Vector2 playerPosition = new Vector2(2f, 0f);
            playerTwoPrefab = PhotonNetwork.Instantiate(playerPrefab.name, playerPosition, Quaternion.identity);
            photonView.RPC(nameof(SetPlayerTwo), RpcTarget.AllBuffered, playerTwoPrefab.GetComponent<PhotonView>().ViewID);
        }
    }

    [PunRPC]
    void SetPlayerOne(int id)
    {
        playerOnePrefab = PhotonView.Find(id).gameObject;
    }
    
    [PunRPC]
    void SetPlayerTwo(int id)
    {
        playerTwoPrefab = PhotonView.Find(id).gameObject;
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        base.OnPlayerLeftRoom(otherPlayer);
        if (playerOnePrefab != null) Destroy(playerOnePrefab);

        if (playerTwoPrefab != null) Destroy(playerTwoPrefab);
    }
}
