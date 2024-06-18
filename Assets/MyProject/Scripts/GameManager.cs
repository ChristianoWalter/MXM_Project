using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviourPunCallbacks
{
    public static GameManager instance;

    [SerializeField] GameObject placeholderPrefab;
    [SerializeField] GameObject keeperPrefab;

    [SerializeField] public GameObject playerOnePrefab;
    [SerializeField] public GameObject playerPrefab;
    [SerializeField] public GameObject playerTwoPrefab;
    public List<PlayerController> playersInGame = new List<PlayerController>();
    [HideInInspector] public Transform cameraPlayer;

    [SerializeField] GameObject gameScreen;


    private void Awake()
    {
        instance = this;
        gameScreen.SetActive(false);
    }

    public void StartGame()
    {
        //photonView.RPC(nameof(CreatePlayerAvatar), PhotonNetwork.LocalPlayer, Netmanager.instance.inputNickname.text);

        if (PhotonNetwork.CurrentRoom.PlayerCount == 2)
        {
            photonView.RPC(nameof(CreatePlayer), RpcTarget.AllBuffered);
        }
    }

    [PunRPC]
    void CreateAvatar()
    {
        //PhotonNetwork.LocalPlayer.NickName = Netmanager.instance.inputNickname.text;

        if (PhotonNetwork.PlayerList[0] == PhotonNetwork.LocalPlayer)
        {
            Vector3 _pos = new Vector2(-2f, 0f);
             var _player = PhotonNetwork.Instantiate(playerPrefab.name, _pos, Quaternion.identity);
            playersInGame.Add(_player.GetComponent<PlayerController>());
        }
        else
        {
            Vector3 _pos = new Vector2(2f, 0f);
            PhotonNetwork.Instantiate(playerPrefab.name, _pos, Quaternion.identity);
        }

        gameScreen.SetActive(true);

        if (Netmanager.instance != null)
        {
            Netmanager.instance.loadingScreen.SetActive(false);
            PhotonNetwork.LocalPlayer.NickName = Netmanager.instance.inputNickname.text;
        }

        if (NetworkManager.instance != null)
        {
            NetworkManager.instance.loadingScreen.SetActive(false);
            NetworkManager.instance.generalMenusScreen.SetActive(false);
        }
    }
    
    [PunRPC]
    void CreatePlayer()
    {
        //PhotonNetwork.LocalPlayer.NickName = Netmanager.instance.inputNickname.text;

        if (PhotonNetwork.PlayerList[0] == PhotonNetwork.LocalPlayer)
        {
            Vector3 _pos = new Vector2(-2f, 0f);
             playerOnePrefab = PhotonNetwork.Instantiate(playerPrefab.name, _pos, Quaternion.identity);
        }
        else
        {
            Vector3 _pos = new Vector2(2f, 0f);
            playerTwoPrefab = PhotonNetwork.Instantiate(playerPrefab.name, _pos, Quaternion.identity);
        }

        gameScreen.SetActive(true);

        if (Netmanager.instance != null)
        {
            Netmanager.instance.loadingScreen.SetActive(false);
            PhotonNetwork.LocalPlayer.NickName = Netmanager.instance.inputNickname.text;
        }

        if (NetworkManager.instance != null)
        {
            NetworkManager.instance.loadingScreen.SetActive(false);
            NetworkManager.instance.generalMenusScreen.SetActive(false);
        }
    }

}
