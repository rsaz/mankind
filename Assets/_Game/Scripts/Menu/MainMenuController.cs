using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    #region Fields

    [Tooltip("Elements of the menu that will be disabled on start")]
    public List<GameObject> DisableOnStart;

    [Tooltip("Setting that will be load when game starts")]
    [SerializeField]
    private SettingMenuController _settings;

    #endregion

    #region Unity Events

    private void Start()
    {
        _settings.LoadLastSavedSettings();
        DisableOnStart?.ForEach(obj => obj.SetActive(false));
    }

    #endregion

    #region Methods

    public void LoadNextScene(string scene) => SceneManager.LoadScene(scene);

    public void Quit() => Application.Quit();

    #endregion
}
