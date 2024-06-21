using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;
using UnityEngine.EventSystems;
using Photon.Realtime;

public class ChatBox : MonoBehaviourPunCallbacks
{
    public TextMeshProUGUI chatLogText;
    public TMP_InputField chatInput;


    // instance
    public static ChatBox instance;


    void Awake()
    {
        instance = this;
    }


    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            if (EventSystem.current.currentSelectedGameObject == chatInput.gameObject)
                BtnSendMsg();
            else
                EventSystem.current.SetSelectedGameObject(chatInput.gameObject);
        }
    }


    // Quando o jogador aciona o botão enviar mensagem
    public void BtnSendMsg()
    {
        if (chatInput.text.Length > 0)
        {
            photonView.RPC("Log", RpcTarget.All, PhotonNetwork.LocalPlayer.NickName, chatInput.text);
            chatInput.text = "";
        }


        EventSystem.current.SetSelectedGameObject(null);
    }

    public void SendNotification(string _massage)
    {
        photonView.RPC("NotificationLog", RpcTarget.AllBuffered, PhotonNetwork.LocalPlayer.NickName, _massage);
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        base.OnPlayerLeftRoom(otherPlayer);
        photonView.RPC("NotificationLog", RpcTarget.AllBuffered, otherPlayer.NickName, "saiu da sala");
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        base.OnPlayerEnteredRoom(newPlayer);
        photonView.RPC("NotificationLog", RpcTarget.AllBuffered, newPlayer.NickName, "entrou na sala");
    }

    // chamado quando um jogador digita uma mensagem na caixa de bate-papo
    // envia para todos os jogadores na sala para atualizar sua interface do usuário
    [PunRPC]
    void Log(string playerName, string message)
    {
        // atualiza o chat log com as mensagens enviadas
        chatLogText.text += string.Format("<b>{0}:</b> {1}\n", playerName, message);


        // ajusta o tamanho do chat log conforme o tamanho do texto
        chatLogText.rectTransform.sizeDelta = new Vector2(chatLogText.rectTransform.sizeDelta.x, chatLogText.mesh.bounds.size.y + 20);
    }
    
    [PunRPC]
    void NotificationLog(string playerName, string message)
    {
        // atualiza o chat log com as mensagens enviadas
        chatLogText.text += string.Format("<b>{0} <b> {1}\n", playerName, message);

        Debug.Log(message);
        // ajusta o tamanho do chat log conforme o tamanho do texto
        chatLogText.rectTransform.sizeDelta = new Vector2(chatLogText.rectTransform.sizeDelta.x, chatLogText.mesh.bounds.size.y + 20);
    }
}
