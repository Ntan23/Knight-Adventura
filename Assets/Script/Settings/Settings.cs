using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Rendering;

public class Settings : MonoBehaviour
{
    public AudioMixer MainMixer;
    [SerializeField] TMP_Dropdown QualityDropdown;
    [SerializeField] Slider BGMSlider;
    [SerializeField] Slider SFXSlider;
    [SerializeField] Toggle fullscreenToogle;
    SettingsManager settingsManager;

    private void Awake()
    {
        settingsManager = SettingsManager.instance;
    }
    private void Start()
    {
        BGMSlider.value = settingsManager.bgmMixerVolume;
        SFXSlider.value = settingsManager.SFXMixerVolume;
        
        if(settingsManager.fullscreenIndicator == 1)
        {
            fullscreenToogle.isOn = true;
        }
        else if(settingsManager.fullscreenIndicator == 0)
        {
            fullscreenToogle.isOn = false;
        }

        QualityDropdown.value = settingsManager.qualityLevel;
    }
    
    public void UpdateBGMSound(float value)
    {
        MainMixer.SetFloat("BGMVolume",value);
        PlayerPrefs.SetFloat("BGM_Volume",value);
    }
    public void UpdateSFXSound(float value)
    {
        MainMixer.SetFloat("SFXVolume",value);
        PlayerPrefs.SetFloat("SFX_Volume",value);
    }

    public void UpdateQuality(int qualityIndex)
    {
        QualitySettings.SetQualityLevel(qualityIndex);
        PlayerPrefs.SetInt("QualityLevel",qualityIndex);
    }

    public void FullScreenControl(bool isFullscreen)
    {
        Screen.fullScreen = isFullscreen;

        if(isFullscreen)
        {
            PlayerPrefs.SetInt("IsFullscreen",1);
        }
        else if(!isFullscreen)
        {
            PlayerPrefs.SetInt("IsFullscreen",0);
        }
    }
}
