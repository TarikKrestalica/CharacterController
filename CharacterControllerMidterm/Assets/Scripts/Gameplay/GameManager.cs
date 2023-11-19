using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

// Understanding the purpose and integrating the system: Aria Feeney's Callback System
public class GameManager : MonoBehaviour
{
    //TODO: have a public reference to commonlly called components like statsystem
    //So you aren't using as many get component calls in the rest of your project - Aria
    public static GameManager gameManager;
    [SerializeField] private int sceneIndex = 0;
    public static bool ontoNewScene;
    private static bool gamePaused;

    private void Awake()
    {
        if(gameManager != null)   // If it's not empty: Have a different manager
        {
            Destroy(this.gameObject);
        }
        gameManager = this;
        DontDestroyOnLoad(gameManager);
    }

    private void Start()
    {
        gamePaused = false;
        ontoNewScene = false;
    }

    public static GameObject player
    {
        get
        {
            if (gameManager.m_player == null)
            {
                gameManager.m_player = GameObject.FindGameObjectWithTag("Player");
            }

            return gameManager.m_player;
        }
    }

    private GameObject m_player;

    public static GameObject environment
    {
        get
        {
            return GameObject.FindGameObjectWithTag("Environment");
        }

    }

    public static PlayerController playerController
    {
        get
        {
            if (gameManager.m_controller == null)
            {
                gameManager.m_controller = player.GetComponent<PlayerController>();
            }

            return gameManager.m_controller;
        }
    }

    private PlayerController m_controller;

    public static UserInterface UI
    {
        get
        {
            if(gameManager.m_UI == null)
            {
                gameManager.m_UI = GameObject.FindGameObjectWithTag("UI").GetComponent<UserInterface>();
            }

            return gameManager.m_UI;
        }
    }

    private UserInterface m_UI;

    private void Update()
    {
        if (ontoNewScene)
        {
            if (gameManager.m_player != null)
            {
                gameManager.m_player.transform.position = new Vector3(-7.25f, -1f, 0f);    
                ontoNewScene = false;
            }
        }
    }

    public void GoToScene(int index)
    {
        SceneManager.LoadSceneAsync(index, LoadSceneMode.Single);
        ontoNewScene = true;
    }

    public static void SetPauseState(bool toggle)
    {
        gamePaused = toggle;
    }

    public static bool IsPaused()
    {
        return gamePaused;
    }

    public static void PausePerformed()
    {
        if (!IsPaused())
        {
            SetPauseState(true);
            UI.EnablePauseScreen();
            GameObject.FindGameObjectWithTag("Inventory").GetComponent<InventoryUI>().GenerateUI(playerController.playerInventory.items);
            Time.timeScale = 0f;
        }
        else
        {
            SetPauseState(false);
            GameObject.FindGameObjectWithTag("Inventory").GetComponent<InventoryUI>().RemoveUI();
            Time.timeScale = 1f;
            UI.DisablePauseScreen();
        }
    }

    public void TryToGoToNextScene()
    {
        // Do I have any more scenes left: https://discussions.unity.com/t/how-do-i-get-unity-to-return-the-total-number-of-scenes-in-my-build/182541
        if (sceneIndex >= SceneManager.sceneCountInBuildSettings - 1)
        {
            sceneIndex = 0;
        }
        else  // Exceeded scene limit
        {
            ++sceneIndex;
        }

        GoToScene(sceneIndex);
    }
}
