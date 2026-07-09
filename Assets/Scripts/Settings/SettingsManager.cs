using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;

public class SettingsManager : MonoBehaviour
{
    public Slider musicSlider;
    public Slider sfxSlider;
    public TMP_Dropdown resolutionDropdown;

    private readonly Vector2Int[] resolutions =
    {
        new Vector2Int(1280, 720),
        new Vector2Int(1920, 1080),
        new Vector2Int(2560, 1440)
    };

    void Start()
    {
        LoadSettings();
        SetupResolution();
    }

    // Музыка
   
    public void SetMusicVolume(float value)
    {
        PlayerPrefs.SetFloat("MusicVolume", value);
    }

    // SFX
 
    public void SetSFXVolume(float value)
    {
        PlayerPrefs.SetFloat("SFXVolume", value);
    }

    // Загрузка настроек
    
    void LoadSettings()
    {
        musicSlider.value = PlayerPrefs.GetFloat("MusicVolume", 1f);
        sfxSlider.value = PlayerPrefs.GetFloat("SFXVolume", 1f);
    }

    // Разрешение экрана
    
    void SetupResolution()
{
    resolutionDropdown.ClearOptions();

    List<string> options = new List<string>();

    foreach (Vector2Int resolution in resolutions)
    {
        options.Add($"{resolution.x} x {resolution.y}");
    }

    resolutionDropdown.AddOptions(options);

    int savedIndex = PlayerPrefs.GetInt("Resolution", 1);

    if (savedIndex < 0 || savedIndex >= resolutions.Length)
    {
        savedIndex = 1;
    }

    resolutionDropdown.value = savedIndex;
    resolutionDropdown.RefreshShownValue();

    SetResolution(savedIndex);
}

    public void SetResolution(int index)
    {
        Vector2Int resolution = resolutions[index];

        Screen.SetResolution(
            resolution.x,
            resolution.y,
            Screen.fullScreen
        );

        PlayerPrefs.SetInt("Resolution", index);
        PlayerPrefs.Save();
    }

    // Назад
    
    public void Back()
    {
        PlayerPrefs.Save();
        SceneManager.LoadScene("MainMenu");
    }
}