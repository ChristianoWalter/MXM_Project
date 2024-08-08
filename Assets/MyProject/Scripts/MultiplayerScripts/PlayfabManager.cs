using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayFab;
using PlayFab.ClientModels;
using System;
using UnityEditor.PackageManager.Requests;

public class PlayfabManager : MonoBehaviour
{
    public static PlayfabManager instance;
    public string PlayfabID;

    Dictionary<string, string> playerData = new Dictionary<string, string>();

    private void Awake()
    {
        instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        Login();
    }

    // Update is called once per frame
    void Update()
    {

    }

    void SetUserData()
    {
        PlayFabClientAPI.UpdateUserData(new UpdateUserDataRequest()
        {
            Data = new Dictionary<string, string>()
            {
                {"XP", "500"},
                {"Level", "2" }
        }
        },
            result =>
            {
                Debug.Log("dados atualizados");
            },
            error =>
            {
                Debug.Log(error.ErrorMessage);
            }

        );
    }

    void GetUserData(string myPlayfabID)
    {
        PlayFabClientAPI.GetUserData(new GetUserDataRequest()
        {
            PlayFabId = myPlayfabID,
            Keys = null
        },
            result =>
            {
                if (result == null || !result.Data.ContainsKey("XP"))
                {
                    Debug.Log("Sem chave");
                }
                else
                {
                    Debug.Log(result.Data["XP"].Value);
                    Debug.Log(result.Data["Level"].Value);
                }
            },
            error =>
            {

            }
             );
    }

    public void ClienGetTittleData()
    {
        PlayFabClientAPI.GetTitleData(new GetTitleDataRequest(),
            result => {
                if (result == null || !result.Data.ContainsKey("FirstMessage"))
                {
                    Debug.Log("Sem mensagem");
                }
                else Debug.Log(result.Data["FirstMessage"]);
            },
            erro => {

            }
        );
    }

    void SetUserDataWithJson(string _id, string _data)
    {
        PlayFabClientAPI.UpdateUserData(new UpdateUserDataRequest()
        {
            Data = new Dictionary<string, string>() {
            {
                _id, _data
            }}
        },
        result => Debug.Log("Dados Atualizados com sucesso!"),
        error => { Debug.Log("Error" + error.ErrorMessage); });
    }

    void GetUserDataWithJson(string myPlayfabID, string _id)
    {
        PlayFabClientAPI.GetUserData(new GetUserDataRequest()
        {
            PlayFabId = myPlayfabID,
            Keys = null
        },
        result =>
        {
            if (result == null || !result.Data.ContainsKey(_id))
            {
                Debug.Log("Dados não encontrados");
            }
            else
            {
                PlayerInfo _playerInfo = JsonUtility.FromJson<PlayerInfo>(result.Data[_id].Value);
                Debug.Log(_playerInfo.nickName);
                Debug.Log(_playerInfo.level);
                Debug.Log(_playerInfo.currentXP);
            }
        },
        error => { Debug.Log("Error" + error.ErrorMessage); });
    }

    #region Login
    void Login()
    {
        var requestLogin = new LoginWithCustomIDRequest
        {
            CustomId = SystemInfo.deviceUniqueIdentifier,
            CreateAccount = true
        };

        PlayFabClientAPI.LoginWithCustomID(requestLogin, OnLoginSuccess, OnLoginFailed);


    }

    private void OnLoginFailed(PlayFabError error)
    {
        Debug.Log(error.ErrorMessage);
    }

    private void OnLoginSuccess(LoginResult result)
    {
        Debug.Log("Login efetuado");
        PlayfabID = result.PlayFabId;
        Debug.Log(result.PlayFabId);

        PlayerInfo _playerInfo = new PlayerInfo("chris", 1, 0);
        string _playerData = JsonUtility.ToJson(_playerInfo);

        //GetUserDataWithJson(PlayfabID, "PlayerInfo");

        GetAllPlayerData();

        //SetUserDataWithJson("PlayerInfo", _playerData);

        //ClienGetTittleData();
        //GetUserData(PlayfabID);
    }
    #endregion

    public void SavePlayerData(string _dataName, string _data)
    {
        PlayFabClientAPI.UpdateUserData(new UpdateUserDataRequest()
        {
            Data = new Dictionary<string, string>()
            {
                {_dataName, _data}
        }
        },
            result =>
            {
                Debug.Log("Dados " + _dataName + " atualizados");
            },
            error =>
            {
                Debug.Log(error.ErrorMessage);
            }

        );
    }

    void GetAllPlayerData()
    {
        var _request = new GetUserDataRequest()
        {
            PlayFabId = PlayfabID,
            Keys = null
        };
        PlayFabClientAPI.GetUserData(_request, GetAllPlayerDataSuccess, GetAllPlayerDataFail);
    }

    private void GetAllPlayerDataSuccess(GetUserDataResult result)
    {
        //playerData = result.Data;
        Debug.Log("Datos do PlayerData recuperados");
        Debug.Log(playerData.Count);

        foreach (var item in result.Data)
        {
            playerData.Add(item.Key, item.Value.Value);
        }

        foreach (var item in playerData)
        {
            Debug.Log(item.Key + "-" + item.Value);
        }

    }

    private void GetAllPlayerDataFail(PlayFabError error)
    {

    }

    void CreateAccount (string username, string email, string password)
    {
        var registerRequest = new RegisterPlayFabUserRequest()
        {
            Username = username,
            Email = email,
            Password = password
        };
        //PlayFabClientAPI.RegisterPlayFabUser ()
    }

    public void AddUsernamepassword()
    {
        var _request = new AddUsernamePasswordRequest()
        {
            Username = "",
            Password = "",
            Email = ""
        };
        PlayFabClientAPI.AddUsernamePassword(_request, AddUsernamePasswordSucces, AddUsernamePasswordFailed);
    }

    private void AddUsernamePasswordSucces(AddUsernamePasswordResult result)
    {
        Debug.Log(result.Username);
    }

    private void AddUsernamePasswordFailed(PlayFabError error)
    {
        Debug.Log(error.ErrorMessage);
    }
}

