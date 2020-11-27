using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

public class SettingMenuController : MonoBehaviour
{
    #region Fields

    [Tooltip("Main Audio Mixer")]
    [SerializeField]
    private AudioMixer _audioMixer = null;

    [SerializeField]
    private Slider _volumeSlider = null;

    [SerializeField]
    private Toggle _fullscreenToggle = null;

    [SerializeField]
    private TMP_Dropdown _qualityDropdown = null;

    [SerializeField]
    private TMP_Dropdown _resolutionDropdown = null;

    private Resolution[] _resolutions;

    private int ResolutionIndex
    {
        get => PlayerPrefs.GetInt("ResolutionIndex", int.MinValue);
        set => PlayerPrefs.SetInt("ResolutionIndex", value);
    }

    private int QualityIndex
    {
        get => PlayerPrefs.GetInt("QualityIndex", int.MinValue);
        set => PlayerPrefs.SetInt("QualityIndex", value);
    }

    private float MainVolumeLevel
    {
        get => PlayerPrefs.GetFloat("MainVolume", float.MinValue);
        set => PlayerPrefs.SetFloat("MainVolume", value);
    }

    private bool FullscreenMode
    {
        get => PlayerPrefs.GetInt("FullscreenMode") == 1;
        set => PlayerPrefs.SetInt("FullscreenMode", value ? 1 : 0);
    }

    #endregion

    #region Methods

    public void LoadLastSavedSettings()
    {
        ConfigureUI();
        SetResolution(ResolutionIndex);
        SetGraphicsQuality(QualityIndex);
        SetVolume(MainVolumeLevel);
    }

    private void ConfigureUI()
    {
        DefineDefaultResolution();
        _resolutionDropdown?.ClearOptions();

        List<string> resolutions = GetResolutions();
        SetCurrentResolutionAsDefaultInDropdown(resolutions);
        SetCurrentQualityLevel();
        SetCurrentFullscreenLevel();
        SetCurrentVolumeLevel();
    }

    public void DefineDefaultResolution() => _resolutions = Screen.resolutions;


    private List<string> GetResolutions() => _resolutions.Select(r => $"{r.width} x {r.height}").ToList();

    private void SetCurrentResolutionAsDefaultInDropdown(List<string> resolutions)
    {
        AddResolutionsToDropdown(resolutions);
        _resolutionDropdown.value = GetLastResolutionSavedOrDefault(resolutions);
        _resolutionDropdown.RefreshShownValue();
    }

    private void AddResolutionsToDropdown(List<string> resolutions) => _resolutionDropdown.AddOptions(resolutions);

    private int GetLastResolutionSavedOrDefault(List<string> resolutions)
    {
        var currentResolutionFormated = resolutions.First(r => r == $"{Screen.currentResolution.width} x {Screen.currentResolution.height}");
        if (string.IsNullOrEmpty(currentResolutionFormated))
        {
            throw new MissingReferenceException("No resolution was found");
        }

        if (ResolutionIndex > int.MinValue)
        {
            return ResolutionIndex;
        }

        int resolutionIndex = resolutions.IndexOf(currentResolutionFormated);
        ResolutionIndex = resolutionIndex;

        return resolutionIndex;
    }

    private void SetCurrentQualityLevel()
    {
        _qualityDropdown.value = QualityIndex;
        _qualityDropdown.RefreshShownValue();
    }

    public void SetCurrentFullscreenLevel() => _fullscreenToggle.isOn = FullscreenMode;

    public void SetCurrentVolumeLevel() => _volumeSlider.value = MainVolumeLevel;

    /// <summary>
    /// MainVolume is the exposed variable used to control the volume in the audio mixer.
    /// </summary>
    /// <param name="volume"></param>
    public void SetVolume(float volume)
    {
        _audioMixer.SetFloat("MainVolume", volume);
        MainVolumeLevel = volume;
    }

    /// <summary>
    /// Should be based on the Project Settings > Quality.
    /// </summary>
    public void SetGraphicsQuality(int qualityIndex)
    {
        QualitySettings.SetQualityLevel(qualityIndex);
        QualityIndex = qualityIndex;
    }

    public void IsFullscreen(bool isFullscreen)
    {
        Screen.fullScreen = isFullscreen;
        FullscreenMode = isFullscreen;
    }

    /// <summary>
    /// Given the index of the list of resolutions in the dropdown, set the choosen value.
    /// </summary>
    /// <param name="resolutionIndex"></param>
    public void SetResolution(int resolutionIndex)
    {
        var resolution = _resolutions[resolutionIndex];
        Screen.SetResolution(resolution.width, resolution.height, FullscreenMode);
        ResolutionIndex = resolutionIndex;
    }

    #endregion
}
