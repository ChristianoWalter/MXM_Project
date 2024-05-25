using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

public class CreateRoom : MonoBehaviourPunCallbacks
{
    [SerializeField]
    private Text _roomName;
    private Text RoomName
    {
        get { return _roomName;}
    }


    public void OnClick_CreateRoom()
    {

        RoomOptions roomOptions = new RoomOptions() { IsVisible = true, IsOpen = true, MaxPlayers = 4};


        if (PhotonNetwork.CreateRoom(RoomName.text, roomOptions, TypedLobby.Default))
        {
            Debug.Log("Create room successfuly sent.");
        }
        
        else
        {
            Debug.Log("Create room failed to send.");
        }
    }

    private void OnPhotonCreateRoomFailed(object[] codeAndMessage)
        {
            Debug.Log("Create room failed: " + codeAndMessage[1]);
        }

    private void OnCreatedRoom()
        {
            Debug.Log("Room created successfully.");
        }
}
