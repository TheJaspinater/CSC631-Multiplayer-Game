using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager instance;

    public GameObject startMenu;
    public InputField usernameField;
    public Client clientText;
    public GameObject InGamePauseMenu;
    public Text IpAddressInPauseMenu;
    public bool inGame = false;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Debug.Log("Instance already exists, destroying object!");
            Destroy(this);
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && inGame == true)
        {
            InGamePauseMenu.SetActive(!InGamePauseMenu.activeSelf);
        }
    }

    public void ConnectToServer()
    {
        inGame = true;
        startMenu.SetActive(false);
        usernameField.interactable = false;
        Client.instance.ConnectToServer();
    }

    public void TurnUIelementOnOff(GameObject ElementToSwitch)
    {
        ElementToSwitch.SetActive(!ElementToSwitch.activeSelf);
    }

    public void updateServerIPAddress(InputField input)
    {
        clientText.ip = input.text;
        IpAddressInPauseMenu.text = input.text;
    }

    public void UpdateCharacterChoice(int charChoice)
    {
        clientText.characterSelection = charChoice;
    }

    public void TurnOnFullscreen()
    {
        Screen.fullScreen = !Screen.fullScreen;
    }

    public void ChangeResolution(Dropdown change)
    {
        switch (change.value)
        {
            case 0:
                Screen.SetResolution(1920, 1080, Screen.fullScreen);
                break;

            case 1:
                Screen.SetResolution(1280, 720, Screen.fullScreen);
                break;

            case 2:
                Screen.SetResolution(800, 600, Screen.fullScreen);
                break;

            default:
                break;
        }
    }

    public void EndGame()
    {
        Application.Quit();
    }
}