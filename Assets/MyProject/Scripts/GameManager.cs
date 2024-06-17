using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviourPunCallbacks
{
    public static GameManager instance;

    [SerializeField] public GameObject playerPrefabTeamRed;
    [SerializeField] public GameObject playerPrefabTeamBlue;
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
            photonView.RPC(nameof(CreateAvatar), RpcTarget.AllBuffered);
        }
    }


    [PunRPC]
    void CreateAvatar()
    {
        //PhotonNetwork.LocalPlayer.NickName = Netmanager.instance.inputNickname.text;

        if (PhotonNetwork.PlayerList[0] == PhotonNetwork.LocalPlayer)
        {
            Vector3 _pos = new Vector2(-2f, 0f);
            PhotonNetwork.Instantiate(playerPrefabTeamRed.name, _pos, Quaternion.identity);
        }
        else
        {
            Vector3 _pos = new Vector2(2f, 0f);
            PhotonNetwork.Instantiate(playerPrefabTeamBlue.name, _pos, Quaternion.identity);
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
        }
    }

}
