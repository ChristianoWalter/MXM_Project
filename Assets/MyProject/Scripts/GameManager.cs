using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.SceneManagement;
using Unity.VisualScripting;

public class GameManager : MonoBehaviourPunCallbacks
{
    public static GameManager instance;

    [SerializeField] public GameObject playerPrefab;
    [SerializeField] public GameObject playerOnePrefab;
    [SerializeField] public GameObject playerTwoPrefab;
    bool youLose;
    public List<PlayerController> playersInGame = new List<PlayerController>();

    private void Awake()
    {
        instance = this;
        //gameScreen.SetActive(false);
    }

    private void Start()
    {
        /*NetworkManager.instance.LoadScreen(5);
        StartGame();*/
    }

    public void StartGame()
    {
        if (PhotonNetwork.CurrentRoom.PlayerCount == 2)
        {
            photonView.RPC(nameof(CreatePlayer), RpcTarget.AllBuffered);
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
        if (youLose)
        {
            NetworkManager.instance.LoadScreen(7);
        }
        else
        {
            NetworkManager.instance.LoadScreen(6);
        }
    }

    [PunRPC]
    void CreatePlayer()
    {
        playerPrefab = NetworkManager.instance.currentCharacter;
        NetworkManager.instance.LoadScreen(5);

        if (PhotonNetwork.PlayerList[0] == PhotonNetwork.LocalPlayer)
        {
            Vector2 playerPosition = new Vector2(-2f, 0f);
            playerOnePrefab = PhotonNetwork.Instantiate(playerPrefab.name, playerPosition, Quaternion.identity);
            //playersInGame.Add(playerOnePrefab.GetComponent<PlayerController>());

        }
        else
        {
            Vector2 playerPosition = new Vector2(2f, 0f);
            playerTwoPrefab = PhotonNetwork.Instantiate(playerPrefab.name, playerPosition, Quaternion.identity);
            //playersInGame.Add(playerTwoPrefab.GetComponent<PlayerController>());
        }
    }

    
}
