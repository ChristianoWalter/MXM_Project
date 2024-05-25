using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class RoomLayoutGroup : MonoBehaviour
{
    
    [SerializeField]
    private GameObject _roomListingPrefab;
    private GameObject RoomListingPrefab
    {
        get { return _roomListingPrefab; }
    }

    private List<RoomListing> _roomListingButtons = new List<RoomListing>();
    private  List<RoomListing> RoomListingButtons
    {
        get { return _roomListingButtons; }
    }

    //Photon vai avisar que existe um novo RoomUpdate
    private void OnReceivedRoomListUpdate()
    {
        //Pega todos os números
        RoomInfo[] rooms = PhotonNetwork.GetCustomRoomList();

        //Vai passar todos os os números e passar pelo RoomReceived
        foreach (RoomInfo room in rooms)
        {
            RoomReceived(room);
        }

        RemoveOldRooms();
    }

    //RoomReceived vai trackear para ver se o room já existe, se já existe vai apenas dar um update nos valores
    //Tambem trackea se um button já existe para uma room
    private void RoomReceived(RoomInfo room)
    {
        //vai olhar todos os scripts de private List<RoomListing> RoomListingButtons, lá encima, categorizando eles como x e comparando com a variavel x.RoomName, presente no script RoomListing
        //se é igual ao parametro room, presente nesse metodo, então é .Name, logo o index retorna um valor diferente de -1
        int index = RoomListingButtons.FindIndex(x => x.RoomName == room.Name);

        //se retornou -1 significa que a room não pode ser achada. Logo criaremos um butão para essa room e adiciona-lo na lista
        if (index == -1)
        {
            if (room.IsVisible && room.PlayerCount < room.MaxPlayers)
            {
                //isso seta o parente do objeto que acabamos de fazer para o transform do objeto que o tem esse script de RoomLayoutGroup
                GameObject roomListingObj = Instantiate(RoomListingPrefab);
                //definimos falso para alinhar com o LayoutGroup ao inves de ficar solto
                roomListingObj.transform.SetParent(transform, false);

                //extrai o objeto disso acima
                RoomListing roomListing = roomListingObj.GetComponent<RoomListing>();
                //adiciona essa room/objeto para a lista de buttons
                RoomListingButtons.Add(roomListing);
                //seta o index para onde essa room/objeto esta na lista
                index = (RoomListingButtons.Count - 1);
            }
        }

        if (index != -1)
        {
            //isso puxa o objeto do index que acabamos de setar e criar
            //Caso achemos uma RoomListing o if (index == -1) será skipado, ou seja o index não será
            RoomListing roomListing = RoomListingButtons[index];
            //Estamos achando o RoomListing na nossa lista e apenas dando um update no nome
            roomListing.SetRoomNameText(room.Name);
            roomListing.Updated = true;

        }   

    }

    //RemoveOldRooms vai remover rooms com butões que temos mas não existem mais
    private void RemoveOldRooms()
    {
        List<RoomListing> removeRooms = new List<RoomListing>();

        foreach (RoomListing roomListing in RoomListingButtons)
        {
            //se não está updatado adiciona a lista de RemoveOldRooms
            if (!roomListing.Updated)
                removeRooms.Add(roomListing);
            else
                roomListing.Updated = false;

        }
        foreach (RoomListing roomListing in RoomListingButtons)
        {
            GameObject roomListingObj = roomListing.gameObject;
            RoomListingButtons.Remove(roomListing);
            Destroy(roomListingObj);
        }
    }

}
