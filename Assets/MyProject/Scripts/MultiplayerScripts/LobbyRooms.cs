using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using System;
using System.Linq;
using Unity.Collections.LowLevel.Unsafe;
using Unity.VisualScripting;

public class LobbyRooms : MonoBehaviourPunCallbacks
{

    [SerializeField] private RoomListing rlPrefab;
    [SerializeField] private RectTransform rlParent;

    //forma de procurar uma room por uma string
    Dictionary<String, RoomListing> CurrentRooms = new();

    //toda vez que uma sala for atualizada, esse metodo é chamado dando uma lista com info de todas as salas
    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        base.OnRoomListUpdate(roomList);



        //Checa se esta listando
        foreach(RoomInfo roomInfo in roomList)
        {
            if(!CurrentRooms.ContainsKey(roomInfo.Name))
            {
                CurrentRooms[roomInfo.Name] = Instantiate(rlPrefab, rlParent);
            }
            //acessa as informações da sala com informações novas
            CurrentRooms[roomInfo.Name].UpdateRoomInfo(roomInfo);
        }

        //passa por cada elemento da lista e transforma em outro elemento, retornando uma nova lista com nomes das salas
        var Names = roomList.Select(info => info.Name);

        //where: retorna uma lista do mesmo tipo mas apenas onde a condição do elemento for verdadeira

        //Passa pelas salas e checa quais não existem mais
        foreach(var CurrentRoom in CurrentRooms.Where(pair => !Names.Contains(pair.Key)))
        {
            Destroy(CurrentRoom.Value.gameObject);
            CurrentRooms.Remove(CurrentRoom.Key);
        }
    }


    /*void UpdateRoomList(List<RoomInfo> roomList)
    {
        for (int i = 0; i < roomList.Count; i++)
        {
            RoomInfo info = roomList[i];
            if (info.RemovedFromList)
            {
                cachedRoomList.Remove(info.Name);
            }
            else
            {
                cachedRoomList[info.Name] = info;
            }
        }
    }*/

}
