using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    #region variables
    // prefab of player for spawning
    public GameObject playerPrefab;

    // storing active player
    private Player activePlayer;

    //storing active game manager

    public static GameManager instance;

    // pause game bool 
    private bool gamePaused;

    // death UI
    public GameObject deathCanvas;

    // check if player died
    public static bool playerDied;

    // check if game finished
    public static bool finishedGame = false;

    // end game UI

    public GameObject endCanvas;

    //pause menu
    public GameObject pauseMenu;

    //audio clips
    public AudioClip dead;

    #endregion

    private void Awake()
    {
        //check instance
        if(instance != null && instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            DontDestroyOnLoad(gameObject);
            SceneManager.activeSceneChanged += SpawnPlayerOnLoad;

            instance = this;
        }
    }

    private void Start()
    {
        gamePaused = false;
    }

    private void Update()
    {
        if (playerDied)
        {
            deathCanvas.SetActive(true);
            GetComponent<AudioSource>().PlayOneShot(dead, 1);
        }

        if (finishedGame)
        {
            endCanvas.SetActive(true);
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePause();
        }

        if (gamePaused)
        {
            Time.timeScale = 0f;
            pauseMenu.SetActive(true);
        }
        else
        {
            Time.timeScale = 1f;
            pauseMenu.SetActive(false);
        }
    }
    void SpawnPlayerOnLoad(Scene currentScene, Scene nextScene)
    {
        //check for active player
        if(activePlayer == null)
        {
            //spawn one
            GameObject newPlayer = Instantiate(playerPrefab, Vector3.zero, Quaternion.identity);
            activePlayer = newPlayer.GetComponent<Player>();
        }
        else
        {
            // Find the spawn spot
            PlayerSpawnSpot playerSpot = FindObjectOfType<PlayerSpawnSpot>();

            // Position and rotate the player
            activePlayer.transform.position = playerSpot.transform.position;
            activePlayer.transform.rotation = playerSpot.transform.rotation;
        }
    }

    public void TogglePause()
    {
        if (!gamePaused)
        {
            gamePaused = true;
        }
        else
        {
            gamePaused = false;
        }
    }
    public bool GamePaused()
    {
        return gamePaused;
    }
    public void ToMainMenu()
    {
        Player.firstCollectable = 0;
        Player.finalCollectable = 0;
        Player.mazeScore = 0;

        //load main menu scene
        SceneManager.LoadScene();

        gamePaused = false;
    }
    public void RestartGame()
    {
        Player.firstCollectable = 0;
        Player.finalCollectable = 0;
        Player.mazeScore = 0;

        //load first quest scene
        SceneManager.LoadScene();

        gamePaused = false;
    }
}
