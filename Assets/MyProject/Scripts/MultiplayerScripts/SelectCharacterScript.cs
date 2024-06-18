using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

public class SelectCharacterScript : MonoBehaviourPunCallbacks
{
    [SerializeField] GameObject characterSelected;
    private bool alreadyHasSelect;
    [SerializeField] Button btn;
    [SerializeField] Button[] otherBtns;

    public enum Characters
    {
        Placeholder,
        Keeper
    }

    public Characters selectedCharacter;

    public void SelectCharacter()
    {
        btn.interactable = false;
        //photonView.RPC(nameof(SetCharacter), RpcTarget.AllBuffered);
        //GameManager.instance.playerPrefab = characterSelected;
        if (!alreadyHasSelect)
        {
            NetworkManager.instance.SelectedCharacter();
            alreadyHasSelect = true;
            //if (PhotonNetwork.PlayerList[0] != PhotonNetwork.LocalPlayer) NetworkManager.instance.startMatchBtn.interactable = true;
        }
    }

    [PunRPC]
    public void SetCharacter()
    {
        if (PhotonNetwork.PlayerList[0] == PhotonNetwork.LocalPlayer)
        {
            GameManager.instance.playerOnePrefab = characterSelected;
        }
        else
        {
            GameManager.instance.playerTwoPrefab = characterSelected;
        }
    }
}
