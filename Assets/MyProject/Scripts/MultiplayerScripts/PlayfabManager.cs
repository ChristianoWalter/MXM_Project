using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayFab;
using PlayFab.ClientModels;
using System;
using UnityEditor.PackageManager.Requests;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Device;

public class PlayfabManager : MonoBehaviour
{
    public static PlayfabManager instance;
    public string PlayfabID;

    [Header("Warning screen")]
    [SerializeField] TextMeshProUGUI messageTxt;
    public GameObject warningScreen;

    [Header("Login Screens")]
    public GameObject loginScreen;
    public GameObject createAccountScreen;
    public GameObject anonimousLoginScreen;
    public GameObject recoverAccountScreen;

    [Header("Login Information")]
    [SerializeField] TMP_InputField usernameEmailLoginInput;
    [SerializeField] TMP_InputField passwordLoginInput;
    [SerializeField] Button stillLogedBtn;

    [Header("Create Account Information")]
    [SerializeField] TMP_InputField usernameInput;
    [SerializeField] TMP_InputField passwordInput;
    [SerializeField] TMP_InputField confirmPasswordInput;
    [SerializeField] TMP_InputField emailInput;
    
    [Header("Anonimous Login Information")]
    [SerializeField] TMP_InputField anonimousUsernameInput;
    
    [Header("Recover Account Information")]
    [SerializeField] TMP_InputField recoverEmailInput;

    Dictionary<string, string> playerData = new Dictionary<string, string>();

    private void Awake()
    {
        instance = this;
        ShowScreens(1);
    }

    // Start is called before the first frame update
    void Start()
    {
        //Login();
    }

    // Update is called once per frame
    void Update()
    {

    }

    /*void SetUserData()
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


    #region Login
    void LoginWithCustomID()
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

    }*/


    #region Create account/login in playfab game
    public void CreateAccount(string username, string email, string password)
    {
        var registerRequest = new RegisterPlayFabUserRequest()
        {
            Username = username,
            Email = email,
            Password = password
        };
        PlayFabClientAPI.RegisterPlayFabUser(registerRequest,
            result =>
            {
                ShowMessage("Conta criada com sucesso");
                ShowScreens(1);
                NetworkManager.instance.LoadScreen(9);
            },
            error =>
            {
                ShowMessage(error.ErrorMessage);
                ShowScreens(2);
                NetworkManager.instance.LoadScreen(9);
                ShowMessage(error.ErrorMessage);
            }
            );
    }

    public void UserLogin(string _username, string _password)
    {
        var _request = new LoginWithPlayFabRequest()
        {
            Username = _username,
            Password = _password
        };
        PlayFabClientAPI.LoginWithPlayFab(_request, UserLoginSuccess, 
             error =>
        {
            var _requestEmail = new LoginWithEmailAddressRequest()
            {
                Email = _username,
                Password = _password
            };
            PlayFabClientAPI.LoginWithEmailAddress(_requestEmail, UserLoginSuccess, UserLoginFailed);
        }
        );
    }

    private void UserLoginSuccess(LoginResult result)
    {
        NetworkManager.instance.LoadScreen(2);
        NetworkManager.instance.PhotonLogin(usernameEmailLoginInput.text);
    }

    private void UserLoginFailed(PlayFabError error)
    {
        NetworkManager.instance.LoadScreen(9);
        ShowMessage("Falha ao efetuar login: " + $"Error {error.ErrorMessage}");
    }

    public void BtnLogin()
    {
        NetworkManager.instance.LoadScreen(2);

        string _usernameOrEmail = usernameEmailLoginInput.text;
        string _password = passwordLoginInput.text;

        if (string.IsNullOrEmpty(_usernameOrEmail) || string.IsNullOrEmpty(_password))
        {
            ShowMessage("Preencha todos os campos");
            NetworkManager.instance.LoadScreen(9);
        }
        else if (_usernameOrEmail.Length < 3)
        {
            ShowMessage("Dados do usuário inválidos");
            NetworkManager.instance.LoadScreen(9);
        }
        else
        {
            UserLogin(_usernameOrEmail, _password);
        }
    }

    public void BtnCreateAccount()
    {
        NetworkManager.instance.LoadScreen(2);

        string _username = usernameInput.text;
        string _email = emailInput.text;
        string _password = passwordInput.text;
        string _confirmPassword = confirmPasswordInput.text;

        if (string.IsNullOrEmpty(_username) ||
            string.IsNullOrEmpty(_email) ||
            string.IsNullOrEmpty(_password) ||
            string.IsNullOrEmpty(_confirmPassword))
        {
            Debug.Log("Favor preencher todos os campos");
            ShowMessage("Favor preencher todos os campos");
            NetworkManager.instance.LoadScreen(9);
        }
        else if (_username.Length < 3)
        {
            Debug.Log("Username precisa ter ao menos 3 caracteres");
            ShowMessage("Username precisa ter ao menos 3 caracteres");
            NetworkManager.instance.LoadScreen(9);
        }
        else if (_password.Length < 6)
        {
            ShowMessage("Senha precisa ter ao menos 6 caracteres");
            NetworkManager.instance.LoadScreen(9);
        }
        else if (_password != _confirmPassword)
        {
            Debug.Log("A senha não confere!");
            ShowMessage("A senha não confere!");
            NetworkManager.instance.LoadScreen(9);
        }
        else
        {
            CreateAccount(_username, _email, _password);
        }

    }

    public void BtnAnonimousLogin()
    {
        NetworkManager.instance.LoadScreen(2);
        NetworkManager.instance.PhotonLogin(anonimousUsernameInput.text);
    }

    public void BtnStayLoged()
    {
        if (stillLogedBtn)
        {

        }
    }
    #endregion

    #region Account Manager
    public void BtnRecoverAccount()
    {
        var _request = new SendAccountRecoveryEmailRequest()
        {
            Email = recoverEmailInput.text,
            TitleId = PlayFabSettings.TitleId
        };
        PlayFabClientAPI.SendAccountRecoveryEmail(_request, RecoverAccountSuccess, RecoverAccountFailed);
        NetworkManager.instance.LoadScreen(2);
    }

    private void RecoverAccountSuccess(SendAccountRecoveryEmailResult result)
    {
        ShowMessage("Solicitação enviada ao email com sucesso!");
        ShowScreens(1);
        NetworkManager.instance.LoadScreen(9);
    }

    private void RecoverAccountFailed(PlayFabError error)
    {
        ShowMessage(error.ErrorMessage);
        NetworkManager.instance.LoadScreen(9);
    }

    public void DeleteAccount()
    {
        ExecuteCloudScriptRequest request = new ExecuteCloudScriptRequest()
        {
            FunctionName = "deletePlayerAccount",
            GeneratePlayStreamEvent = true
        };
        PlayFabClientAPI.ExecuteCloudScript(request, DeleteAccountSuccess, DeleteAccountFailed);
    }

    private void DeleteAccountSuccess(ExecuteCloudScriptResult result)
    {
        ShowScreens(1);
        NetworkManager.instance.LoadScreen(9);
        ShowMessage("Conta deletada com sucesso");
    }

    private void DeleteAccountFailed(PlayFabError error)
    {
        ShowMessage($"Error {error.ErrorMessage}");
    }
    #endregion

    #region Screens controller
    public void ShowScreens(int screens)
    {
        loginScreen.SetActive(false);
        createAccountScreen.SetActive(false);
        anonimousLoginScreen.SetActive(false);
        recoverAccountScreen.SetActive(false);

        switch (screens)
        {
            case 1:
                loginScreen.SetActive(true);
                break;
            case 2: 
                createAccountScreen.SetActive(true);
                break;
            case 3:
                anonimousLoginScreen.SetActive(true);
                break;
            case 4:
                recoverAccountScreen.SetActive(true);
                break;
        }
    }

    public void ShowMessage(string message)
    {
        warningScreen.SetActive(true);
        messageTxt.text = message;
    }
    #endregion
}

