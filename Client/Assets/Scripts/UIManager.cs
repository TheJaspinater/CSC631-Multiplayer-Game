using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager instance;

    public GameObject startMenu;
    public InputField usernameField;
    public Client ClientText;

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

    public void ConnectToServer()
    {
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
        ClientText.ip = input.text;
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