using UnityEngine;
using UnityEngine.UI;
using Wattle.Wild;
using Wattle.Wild.Infrastructure;
public class UIMainMenu : MonoBehaviour
{
    [Header("Buttons")]
    [SerializeField] private Button playbutton;
    // [SerializeField] private Button optionsButton;
    [SerializeField] private Button creditsButton;
    [SerializeField] private Button quitButton;
    [SerializeField] private Button backButton;

    [Header("Containers")]
    [SerializeField] private RectTransform titleScreenContainer;
    [SerializeField] private RectTransform optionsScreenContainer;
    [SerializeField] private RectTransform creditsScreenContainer;
    [SerializeField] private RectTransform backContainer;

    private void OnEnable()
    {
        playbutton.onClick.AddListener(Play_OnClick);
        // optionsButton.onClick.AddListener(Options_OnClick);
        creditsButton.onClick.AddListener(Credits_OnClick);
        quitButton.onClick.AddListener(Quit_OnClick);
        backButton.onClick.AddListener(Back_OnClick);
    }

    private void OnDisable()
    {
        playbutton.onClick.RemoveListener(Play_OnClick);
        // optionsButton.onClick.RemoveListener(Options_OnClick);
        creditsButton.onClick.RemoveListener(Credits_OnClick);
        quitButton.onClick.RemoveListener(Quit_OnClick);
        backButton.onClick.RemoveListener(Back_OnClick);
    }

    private void Play_OnClick()
    {
        Initialiser.LoadGame();
    }

    private void Options_OnClick()
    {
        titleScreenContainer.ToggleActive(false);
        creditsScreenContainer.ToggleActive(false);

        optionsScreenContainer.ToggleActive(true);
        backContainer.ToggleActive(true);
    }

    private void Credits_OnClick()
    {
        titleScreenContainer.ToggleActive(false);
        optionsScreenContainer.ToggleActive(false);

        creditsScreenContainer.ToggleActive(true);

        backContainer.ToggleActive(true);
    }

    private void Quit_OnClick()
    {
        Initialiser.Quit();
    }

    private void Back_OnClick()
    {
        optionsScreenContainer.ToggleActive(false);
        creditsScreenContainer.ToggleActive(false);
        backContainer.ToggleActive(false);

        titleScreenContainer.ToggleActive(true);
    }
}
