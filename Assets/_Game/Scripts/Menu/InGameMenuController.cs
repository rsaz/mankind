using UnityEngine;

public class InGameMenuController : MonoBehaviour
{
    #region Fields

    [SerializeField]
    private GameObject _playButton;

    [SerializeField]
    private GameObject _resumeButton;

    [SerializeField]
    private GameObject _menuContainer;

    private bool _menuOpen = false;

    #endregion

    #region Unity Events

    void Update()
    {
        if (IsMainMenuActiveAndOpen && Input.GetKeyDown(KeyCode.Escape))
        {
            ToggleInGameMenu();
        }
    }

    #endregion

    #region Methods

    /// <summary>
    /// This avoid active the menu when options or credits is open
    /// </summary>
    private bool IsMainMenuActiveAndOpen => _resumeButton.activeInHierarchy == _menuOpen;

    /// <summary>
    /// Invert the current visible state of the main menu and resume button
    /// </summary>
    public void ToggleInGameMenu()
    {
        _playButton.SetActive(_menuOpen);
        _menuOpen = !_menuOpen;
        _menuContainer.SetActive(_menuOpen);
        _resumeButton.SetActive(_menuOpen);
    }

    #endregion
}
