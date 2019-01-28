using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;
using System;

public class MenuManager : MonoBehaviour {

    private OldGameSession oldGameSession;

    // SoundManager instance only open on main menu.
    private SoundManager soundManager;

    // MenuManager handles all menu interactions and outputs.
    void Start()
    {
        // Debug.Log("Init menu manager");

        // Load data of last game session.
        if (SceneManager.GetActiveScene().name == "MainMenu")
        {
            if (File.Exists(OldGameSession.FILE_NAME))
            {
                oldGameSession = new OldGameSession(new StreamReader(OldGameSession.FILE_NAME));
            } 
        }

        if (SceneManager.GetActiveScene().name == "MainMenu" || SceneManager.GetActiveScene().name == "MenuLvlSelect")
        {
            soundManager = GetComponent<SoundManager>();
            soundManager.StartMenuLoop();
        }
  
    }

    // Called when the button to go back to the main menu is called.
    public void OnBtnReturnMainMenu()
    {
       // Debug.Log("Back to Main Menu button interacted");

        if (soundManager != null)
        {
            soundManager.StopSound(true);
        }
        SceneManager.LoadScene("MainMenu"); 
    }

    /* Main Menu */

    // Called when the play button is pressed.
    public void OnBtnPlay()
    {
        if (soundManager != null)
        {
            soundManager.StopSound(true);
        }

        // Debug.Log("Play button interacted");

        if (oldGameSession != null)
        {
            // If they finished the last level before they quit, set them to next level automatically.
            if (oldGameSession.AutoProgress())
            {
                SceneManager.LoadScene(MapManager.GetNextLevel(oldGameSession.LastLevel));
            } else
            {
                // Set them to the level they were last trying.
                SceneManager.LoadScene(oldGameSession.LastLevel);
            }

            return;
        }

        // If first time, load up introduction.
        SceneManager.LoadScene("Level_Introduction");
    }

    // Called when the level select button is pressed.
    public void OnBtnLevelSelect()
    {
        if (soundManager != null)
        {
            soundManager.StopSound(true);
        }


        // Debug.Log("Level Select button interacted");
        SceneManager.LoadScene("MenuLvlSelect");
    }

    // Called when the highscores button is pressed.
    public void OnBtnHighscores()
    {
        // Debug.Log("Highscores button interacted");
    }

    // Called when the exit is pressed.
    public void OnBtnExit()
    {
        if (soundManager != null)
        {
            soundManager.StopSound(false);
        }

        // Debug.Log("Exit button interacted");
        Application.Quit();
    }

    /* Level Select Menu */

    // Called when a level play button on the level select menu is pressed.
    public void OnBtnLevelPlay(int level)
    {
        // Debug.Log(string.Format("Button Level {0} Play button interacted", level));

        if (soundManager != null)
        {
            soundManager.StopSound(true);
        }


        switch (level)
        {
            case 1:
                SceneManager.LoadScene("Level_Introduction");
                break;
            case 2:
                SceneManager.LoadScene("Level_2");
                break;
            case 3:
                break;
            default:
                SceneManager.LoadScene("Level_Introduction");
                break;
        }

    }

    // Called when a level scoreboard button on the level select menu is pressed.
    public void OnBtnLevelScoreboard(int level)
    {
        // Debug.Log(String.Format("Button Level {0} Scoreboard button interacted", level));

        switch (level)
        {
            case 1:
                break;
            case 2:
                break;
            case 3:
                break;
            default:
                break;
        }
    }

    /* Highscores Menu */

}
