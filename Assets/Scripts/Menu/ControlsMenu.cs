using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Opens a screen to display the credits.
/// Attach to a ControlsCanvas and add a return button (to main menu) as child.
/// </summary>
public class ControlsMenu : MonoBehaviour
{
    #region Fields and Properties

    private Canvas _controlsCanvas;
    private Button _backButton;

    #endregion

    #region Functions

    private void Awake()
    {
        _controlsCanvas = GetComponent<Canvas>();
        _controlsCanvas.enabled = false;
    }

    private void OnEnable()
    {
        MainMenu.OnControlsMenuOpened += OnControlsOpened;
        MainMenu.OnSettingsMenuOpened += OnOtherMenuOpened;
        MainMenu.OnMainMenuOpened += OnOtherMenuOpened;
        MainMenu.OnCreditsMenuOpened += OnOtherMenuOpened;
    }

    private void Start()
    {
        _backButton = GetComponentInChildren<Button>();
        _backButton.onClick.AddListener(OpenMainMenu);
    }

    private void OnDisable()
    {
        MainMenu.OnControlsMenuOpened -= OnControlsOpened;
        MainMenu.OnSettingsMenuOpened -= OnOtherMenuOpened;
        MainMenu.OnMainMenuOpened -= OnOtherMenuOpened;
        MainMenu.OnCreditsMenuOpened -= OnOtherMenuOpened;
    }

    private void OnControlsOpened()
    {
        _controlsCanvas.enabled = true;
    }

    private void OnOtherMenuOpened()
    {
        _controlsCanvas.enabled = false;
    }

    private void OpenMainMenu()
    {
        AudioManager.Instance.PlayButtonClick();
        MainMenu.RaiseMainMenuOpened();
    }

    #endregion
}
