using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour {

    private MapManager mapManager;

    public GameObject entityPlayer;
    private PlayerController playerController;
    public new Camera camera;

    public Transform coinHolder;
    public GameObject prefabCoin;

    public GameObject pausedMenu;
    public bool paused;

    public delegate void GameEvent();

    // Init - GameManager manages the game and all modules and controls what happens.
	void Awake ()
    {
        mapManager = new MapManager();

        MapManager.MapData map = mapManager.mapData;
        if (map == null) throw new NullReferenceException("map data");

        // Setup player
        entityPlayer.transform.position = map.playerSpawn;
        playerController = entityPlayer.GetComponent<PlayerController>();
        playerController.minX = map.minX;
        playerController.maxX = map.maxX;
        playerController.minY = map.minY;
        playerController.gameManager = this;
        playerController.totalCoinCount = GameObject.FindGameObjectsWithTag(GameTag.Coin).Length;

        // RefreshCoinSpawns(map);

        // Save current game data
        UpdateSession(false);
    }

    // Finds all coin objects and writes them to file. Should only be used for testing.
    public void RefreshCoinSpawns(MapManager.MapData map)
    {
        GameObject[] coins = GameObject.FindGameObjectsWithTag(GameTag.Coin);
        map.coinSpawns = new Vector3[coins.Length];

        for (int i = 0; i < coins.Length; i++)
        {
            map.coinSpawns[i] = coins[i].transform.position;
        }
        mapManager.WriteData(map, mapManager.FullPath(map.mapID));
    }

    // Pauses the game.
    public void PauseGame()
    {
        // Debug.Log("Game paused");
        paused = !paused;
        playerController.Pause();
        pausedMenu.GetComponent<Canvas>().enabled = !pausedMenu.GetComponent<Canvas>().enabled;
    }

    // Event called when the player dies and the fadeout is called.
    public void OnPlayerDeathProcess()
    {
        camera.GetComponent<CameraController>().FadeOutWithDelay(1, new GameEvent(OnPostPlayerDeathEvent));
    }

    // Event called when player dies and the game has faded out.
    public void OnPostPlayerDeathEvent()
    {
        RespawnPlayer();
    }

    // Event called when the player finishes the game.
    public void OnGameFinishWin(int coins, float time, int fadeOut = 1)
    {
        UpdateSession(true);
        camera.GetComponent<CameraController>().FadeOutWithDelay(fadeOut, new GameEvent(OnGameReadyNext));
    }

    // Pre-Event called when game is ready to switch level.
    public void OnGameReadyNext()
    {
        // Debug.Log("Loading next level.");

        if (SceneManager.GetActiveScene().name == "Level_2")
        {
            SceneManager.LoadScene("MainMenu");
        } else SceneManager.LoadScene(MapManager.GetNextLevel(mapManager.mapData.mapID));
    }

    // Called when user quits via the pause menu.
    public void UserQuitGame()
    {
        SceneManager.LoadScene("MainMenu");
    }

    // Respawns player
    public void RespawnPlayer()
    {
        // Reset stats
        playerController.ResetAndShow();
        playerController.AddAttempt();
        entityPlayer.transform.position = mapManager.mapData.playerSpawn;
        // Respawn coins
        RespawnCoins();

        camera.GetComponent<CameraController>().FadingOut = false;

        playerController.allowMove = true;
    }

    // Respawns the coins in the map.
    public void RespawnCoins()
    {
        // Remove old coins
        foreach (Transform child in coinHolder.transform)
        {
            GameObject.Destroy(child.gameObject);
        }
        coinHolder.DetachChildren();

        // Spawn new shiny ones
        for (int i = 0; i < mapManager.mapData.coinSpawns.Length; i++)
        {
            Instantiate(prefabCoin, coinHolder).transform.position = mapManager.mapData.coinSpawns[i];
        }
    }

    void Update()
    {
        // Pause game when they press escape
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            PauseGame();
        }
    }

    // Saves game data.
    private void UpdateSession(bool completed)
    {
        OldGameSession oldGameSession = new OldGameSession
        {
            CompletedLastLevel = completed,
            LastLevel = mapManager.mapData.mapID
        };
        mapManager.WriteData<OldGameSession>(oldGameSession, OldGameSession.FILE_NAME);
    }

}
