using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class SingleMode : MonoBehaviour
{
    public static SingleMode instance;

    [SerializeField] GameObject puppetPrefab;
    [SerializeField] public GameObject puppetInGame;
    [SerializeField] GameObject playerCharacter;
    [SerializeField] public GameObject playerCharacterInGame;
    [SerializeField] public bool isSinglePLayer;
    [SerializeField] GameObject level;
    [SerializeField] GameObject levelActive;

    [SerializeField] GameObject pausePanel;

    private void Awake()
    {
        instance = this;
    }

    public void SelectCharacter(GameObject _character)
    {
        playerCharacter = _character;
    }

    public void StartTraining()
    {
        //isSinglePLayer = true;

        NetworkManager.instance.LoadScreen(5);
        GameManager.instance.timeToStartTxt.gameObject.SetActive(false);
        GameManager.instance.gameTimeTxt.text = "00";

        levelActive = Instantiate(level, Vector2.zero, Quaternion.identity);

        Vector2 playerPosition = new Vector2(2f, 0f);
        puppetInGame = PhotonNetwork.Instantiate(puppetPrefab.name, playerPosition, Quaternion.identity);
        puppetInGame.GetComponent<TrainingPuppet>().isPuppet = true;

        playerPosition = new Vector2(-2f, 0f);
        //playerCharacterInGame = Instantiate(playerCharacter, playerPosition, Quaternion.identity);
        playerCharacterInGame = PhotonNetwork.Instantiate(playerCharacter.name, playerPosition, Quaternion.identity);
        playerCharacterInGame.GetComponent<PlayerController>().isInMatch = true;
    }

    public void PauseAction(InputAction.CallbackContext value)
    {
        if (!isSinglePLayer) return;

        if (value.performed)
        {
            PauseUnpause();
        }
    }

    public void PauseUnpause()
    {
        if (pausePanel.activeSelf == true)
        {
            pausePanel.SetActive(false);
            playerCharacterInGame.GetComponent<PlayerController>().canMove = true;
        }
        else
        {
            pausePanel.SetActive(true);
            playerCharacterInGame.GetComponent<PlayerController>().canMove = false;
        }
    }

    public void ExitTraining()
    {
        isSinglePLayer = false;
        Destroy(playerCharacterInGame);
        Destroy(puppetInGame);
        Destroy(levelActive);
        pausePanel.SetActive(false);
        NetworkManager.instance.QuitRoom();
        NetworkManager.instance.LoadScreen(0);
    }
}
