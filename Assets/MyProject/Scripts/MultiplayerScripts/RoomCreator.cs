using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine.UI;
using Unity.VisualScripting;


public class RoomCreator : MonoBehaviourPunCallbacks
{
  
    [SerializeField] private TMP_InputField inputField;
    [SerializeField] private Button btn;

    private void Start()
    {
        btn.onClick.AddListener(CreateRoom);
    }

    private void CreateRoom()
    {
        RoomOptions roomOptions = new RoomOptions{IsVisible = true, MaxPlayers = 2};
        if(PhotonNetwork.CreateRoom("Sala:" + inputField.text + " De:" + PhotonNetwork.LocalPlayer.NickName, roomOptions))
        {
            return;
        }
        else
        {
            Debug.Log("Criação de sala falhou, tente novamente!");
        }
    }

}
