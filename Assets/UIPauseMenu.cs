using UnityEngine;
using UnityEngine.UI;
using Wattle.Wild.Infrastructure;
using Wattle.Wild;

public class UIPauseMenu : MonoBehaviour
{
    [SerializeField] private RectTransform menuContainer;

    [SerializeField] private Button mainMenuButton;
    [SerializeField] private Button quitButton;
    
    private GameState gameState;
    
    private void OnEnable()
    {
        menuContainer.ToggleActive(false);

        Initialiser.OnGameStateChanged += OnGameStateChanged;

        mainMenuButton.onClick.AddListener(MainMenuButton_OnClick);
        quitButton.onClick.AddListener(QuitButton_OnClick);
    }

    private void OnDisable()
    {
        Initialiser.OnGameStateChanged -= OnGameStateChanged;
    }

    private void OnGameStateChanged(GameState gameState)
    {
        this.gameState = gameState;
    }

    private void MainMenuButton_OnClick()
    {
        Initialiser.ChangeGamestate(GameState.MainMenu);
        SceneManager.Instance.LoadScene("MainMenu", UnityEngine.SceneManagement.LoadSceneMode.Single);
    }

    private void QuitButton_OnClick()
    {
        Initialiser.Quit();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            switch (gameState)
            {
                case GameState.MainMenu:
                case GameState.Conversation:
                case GameState.WorldTransition:
                    break;
                case GameState.World:
                    TogglePauseMenu(true);
                    break;
                case GameState.Paused:
                    TogglePauseMenu(false);
                    break;
            }
        }
    }

    private void TogglePauseMenu(bool enabled)
    {
        if (enabled)
        {
            Initialiser.ChangeGamestate(GameState.Paused);
            menuContainer.ToggleActive(enabled);

        }
        else
        {
            Initialiser.ChangeGamestate(GameState.World);
            menuContainer.ToggleActive(enabled);
        }
    }
}
