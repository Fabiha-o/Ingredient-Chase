using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

public class SettingsManager : MonoBehaviour
{
    public AudioMixer audioMixer;
    public Slider musicSlider;
    public Slider sfxSlider;
    public Dropdown textSizeDropdown;
    public Dropdown contrastDropdown;

    void Start()
    {
        // Load saved settings
        musicSlider.value = PlayerPrefs.GetFloat("MusicVolume", 0.5f);
        sfxSlider.value = PlayerPrefs.GetFloat("SFXVolume", 0.5f);
        textSizeDropdown.value = PlayerPrefs.GetInt("TextSize", 1);
        contrastDropdown.value = PlayerPrefs.GetInt("ContrastMode", 0);
    }

    public void SetMusicVolume(float volume)
    {
        audioMixer.SetFloat("MusicVolume", Mathf.Log10(volume) * 20);
        PlayerPrefs.SetFloat("MusicVolume", volume);
    }

    public void SetSFXVolume(float volume)
    {
        audioMixer.SetFloat("SFXVolume", Mathf.Log10(volume) * 20);
        PlayerPrefs.SetFloat("SFXVolume", volume);
    }

    public void SetTextSize(int index)
    {
        PlayerPrefs.SetInt("TextSize", index);
        // Apply text size change to UI elements dynamically
    }

    public void SetContrastMode(int index)
    {
        PlayerPrefs.SetInt("ContrastMode", index);
        // Change UI colors based on the selected contrast mode
    }
}