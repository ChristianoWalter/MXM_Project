using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine.UI;

public class RoomListing : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI roomName;
    [SerializeField] private TextMeshProUGUI Capacity;

   [SerializeField] Button btnEnter;


    private void Start()
    {
        btnEnter.onClick.AddListener(JoinRoom);
    }

    private void JoinRoom()
    {
        PhotonNetwork.JoinRoom(roomName.text);
    }

    //Mantem atualizada as informações das salas, numero de jogadores, capacidade total e nome
    public void UpdateRoomInfo(RoomInfo roomInfo)
    {
        roomName.text = roomInfo.Name;
        //$ = abrir chaves dentro da string, podendo chamar partes do codigo que retornam strings
        Capacity.text = $"{roomInfo.PlayerCount} / {roomInfo.MaxPlayers}";
        
    }


}
