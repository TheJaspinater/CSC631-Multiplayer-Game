using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager instance;

    public Text gameTimerDisplay;
    public float Gametimer;

    public Animator anim;
    public GameObject startMenu;
    public InputField usernameField;
    public int killsNeededToWin;
    //public Client clientText;
    public GameObject InGamePauseMenu;
    public Text IpAddressInPauseMenu;
    public bool inLobby = false;
    public bool inGame = false;
    public bool inEndGame = false;
    public GameObject charSelectPNL;
    public GameObject lobbyPNL;

    public PlayerLobbyInfo[] GameLobby = new PlayerLobbyInfo[4];
    
    public Text[] GameLobbyNames = new Text[4];
    public float lobbyTimerMax;
    private float lobbyTimer;
    public GameObject lobbyTimerDisplay;
    public Text lobbyTimerNumberDisplay;
    public GameObject GameLobbyWindow;
    public GameObject inGameUI;
    public GameObject endGameScreen;
    public Text winnerText;

    public struct PlayerLobbyInfo
    {
        public bool isInGame;
        public int playerId;
        public string username;
        public int charChoice;
    }

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

    private void FixedUpdate()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && inGame == true)
        {
            InGamePauseMenu.SetActive(!InGamePauseMenu.activeSelf);
        }

        if (inGame == true)
        {
            if (Gametimer > 0)
            {
                Gametimer -= Time.deltaTime;
                GametimerCountdown(Gametimer);

            }
            else
            {
                CheckForWinner();
            }
        }

        if(inLobby == true)
        {
            UpdateLobbyNames();

            if (GameLobby[3].isInGame == true)
            {
                lobbyTimerDisplay.SetActive(true);
                lobbyTimer -= Time.deltaTime;
                lobbyTimerNumberDisplay.text = lobbyTimer.ToString();
            }
            else
            {
                lobbyTimer = lobbyTimerMax;
                lobbyTimerDisplay.SetActive(false);
            }

            if(lobbyTimer <= 0)
            {
                inGame = true;
                inLobby = false;
                charSelectPNL.SetActive(false);
                lobbyPNL.SetActive(false);
                startMenu.SetActive(false);
                inGameUI.SetActive(true);
                ClientSend.StartGame();
            }
            
        }

    }

    public void DebugStartGame()
    {
        ClientSend.StartGame();
    }

    public void GametimerCountdown(float timeRemaining)
    {
        timeRemaining += 1;
        float minutes = Mathf.FloorToInt(timeRemaining / 60);
        float seconds = Mathf.FloorToInt(timeRemaining % 60);
        gameTimerDisplay.text = string.Format("{0:00};{1:00}", minutes, seconds);
    }

    public void CheckForWinner()
    {
        int winnerID = 0;
        float highestKills = 0;
        for(int i = 1; i <=4; i++)
        {
            if(GameManager.players[i].kills > highestKills)
            {
                highestKills = GameManager.players[i].kills;
                winnerID = GameManager.players[i].id;
            }
        }

        ClientSend.PlayerWin(winnerID);
    }


    /*
     THIS IS THE METHOD THAT COMPLETES THE GAME. USE THE "winnerID" to determine which end screen to show animating. 
        ALSO MAKE SURE TO DESPAWN HEALTH AND MINIMAP OBJECTS. 
     */
    public void CompleteGameSession(int winnerID)
    {
        inGameUI.SetActive(false);
        inEndGame = true;
        endGameScreen.SetActive(true); //quick fix to show victory screen for now. needs animation component.
        if (GameLobby[winnerID-1].charChoice == 0)
        {
            anim.SetBool("bonesWins", true);
        }
        else if(GameLobby[winnerID-1].charChoice == 2)
        {
            anim.SetBool("goopWins", true);
        }
        else if (GameLobby[winnerID-1].charChoice == 1)
        {
            anim.SetBool("robotWins", true);
        }
        else if (GameLobby[winnerID-1].charChoice == 3)
        {
            anim.SetBool("wraithWins", true);
        }
        winnerText.text = string.Format($"The Winner Is You! {GameLobby[winnerID-1].username}");
        
    }

    public void ConnectToServer()
    {
        inLobby = true;
        usernameField.interactable = false;
        Client.instance.ConnectToServer();
    }

    public void UpdateLobbyNames()
    {
        for(int i = 0; i < 4; i++)
        {
            if (GameLobby[i].isInGame == true)
            {
                GameLobbyNames[i].text = GameLobby[i].username;
            }
        }
    }
    
    public void AddPlayerToLobby(int _playerId, string _username, int _charSelect)
    {
        PlayerLobbyInfo newPlayer = new PlayerLobbyInfo();
        newPlayer.isInGame = true;
        newPlayer.playerId = _playerId;
        newPlayer.username = _username;
        newPlayer.charChoice = _charSelect;
        GameLobby[_playerId-1] = newPlayer;
    }

    public void TurnUIelementOnOff(GameObject ElementToSwitch)
    {
        ElementToSwitch.SetActive(!ElementToSwitch.activeSelf);
    }

    public void updateServerIPAddress(InputField input)
    {
        Client.instance.ip = input.text;
        IpAddressInPauseMenu.text = input.text;
    }

    public void UpdateCharacterChoice(int charChoice)
    {
        Client.instance.characterSelection = charChoice;
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