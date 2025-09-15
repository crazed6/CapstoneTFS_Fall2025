using UnityEngine;

public class SettingsRegister : MonoBehaviour
{
    public string[] settingsNames;
    public GameObject[] settingsPanel;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (SettingsManager.Instance != null)
        {
            for (int i = 0; i < settingsNames.Length; i++)
                SettingsManager.Instance.RegisterUIObject(settingsNames[i], settingsPanel[i]);

            SettingsManager.Instance.UpdateRefences();
        }
    }
}
