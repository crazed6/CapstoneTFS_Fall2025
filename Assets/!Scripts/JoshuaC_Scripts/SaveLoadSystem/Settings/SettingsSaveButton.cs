using UnityEngine;
using UnityEngine.Audio;

public class SettingsSaveButton : MonoBehaviour
{
    public MenuSettings menuSettings;
    public AudioMixer audioMixer;

    public void SaveSettings()
    {
        SettingsSaveSystem.Save(menuSettings, audioMixer);
    }
}
