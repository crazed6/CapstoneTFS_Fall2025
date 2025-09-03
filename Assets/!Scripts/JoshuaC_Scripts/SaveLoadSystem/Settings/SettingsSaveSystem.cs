using UnityEngine;
using UnityEngine.Audio;
using System.IO;

public static class SettingsSaveSystem
{
    private static string path = Application.persistentDataPath + "/settings.json";
    private static SettingsSaveData cachedSettings;

    public static void Save(MenuSettings menuSettings, AudioMixer audioMixer)
    {
        SettingsSaveData data = new SettingsSaveData();

        // Grab resolution index + fullscreen
        data.resolutionIndex = menuSettings.resolutionDropdown.value;
        data.isFullscreen = Screen.fullScreen;

        // Grab mixer values (convert dB back to linear)
        audioMixer.GetFloat("MasterVolume", out float masterVol);
        audioMixer.GetFloat("BGMVolume", out float bgmVol);
        audioMixer.GetFloat("SFXVolume", out float sfxVol);
        audioMixer.GetFloat("VoiceVolume", out float voiceVol);

        data.masterVolume = Mathf.Pow(10, masterVol / 20f);
        data.bgmVolume = Mathf.Pow(10, bgmVol / 20f);
        data.sfxVolume = Mathf.Pow(10, sfxVol / 20f);
        data.voiceVolume = Mathf.Pow(10, voiceVol / 20f);

        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(path, json);
        cachedSettings = data;

        Debug.Log("Settings saved to " + path);
    }

    public static void Load(MenuSettings menuSettings, AudioMixer audioMixer)
    {
        if (!File.Exists(path))
        {
            Debug.Log("No settings file found, using defaults.");
            return;
        }

        string json = File.ReadAllText(path);
        SettingsSaveData data = JsonUtility.FromJson<SettingsSaveData>(json);
        cachedSettings = data;

        // Apply to system
        menuSettings.SetResolution(data.resolutionIndex);
        menuSettings.SetFullscreen(data.isFullscreen);
        menuSettings.SetMasterVolume(data.masterVolume);
        menuSettings.SetBGMVolume(data.bgmVolume);
        menuSettings.SetSFXVolume(data.sfxVolume);
        menuSettings.SetVoiceVolume(data.voiceVolume);

        // Sync UI
        menuSettings.resolutionDropdown.value = data.resolutionIndex;
        menuSettings.resolutionDropdown.RefreshShownValue();
    }

    public static SettingsSaveData GetSettings() => cachedSettings;
}

//In MenuSettings->Start() just drop the following
//SettingsSaveSystem.Load(this, audioMixer);

