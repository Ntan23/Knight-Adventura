using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingsManager : MonoBehaviour
{
    public static SettingsManager instance;
    [HideInInspector] public int qualityLevel;
    [HideInInspector] public float bgmMixerVolume;
    [HideInInspector] public float SFXMixerVolume;
    [HideInInspector] public int fullscreenIndicator;
    [SerializeField] Settings settings;
    private void Awake()
    {
        if(instance==null)
        {
            instance=this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        qualityLevel = PlayerPrefs.GetInt("QualityLevel",2);
        bgmMixerVolume = PlayerPrefs.GetFloat("BGM_Volume",0);
        SFXMixerVolume = PlayerPrefs.GetFloat("SFX_Volume",10);
        fullscreenIndicator = PlayerPrefs.GetInt("IsFullscreen",1);

        QualitySettings.SetQualityLevel(qualityLevel);

        settings.MainMixer.SetFloat("BGMVolume",bgmMixerVolume);
        settings.MainMixer.SetFloat("SFXVolume",SFXMixerVolume);

        if(fullscreenIndicator == 1)
        {
            Screen.fullScreen = true;
        }
        else if(fullscreenIndicator == 0)
        {
            Screen.fullScreen = false;
        }
    }
}
