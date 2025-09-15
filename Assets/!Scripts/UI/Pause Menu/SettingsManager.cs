using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio; // For audio management Mixer reference needed
using UnityEngine.UI; // For UI elements like Dropdown
using TMPro; // For TextMeshPro Dropdown
using System.IO; // For file
using UnityEngine.SceneManagement;
using System.Collections; // For scene to hook into sceneLoaded event

public class SettingsManager : MonoBehaviour
{
    public static SettingsManager Instance { get; private set; }

    [Header("Turn Off/On GameObjects")]
    public GameObject displayMenu; // The display menu panel to toggle
    public GameObject settingsMenu; // The options menu panel to toggle

    [Header("UI References")]
    public TMP_Dropdown resolutionDropdown;
    public TMP_Dropdown qualityDropdown;
    public Toggle fullscreenToggle;

    [Header("Audio Sliders")]
    public Slider masterSlider;
    public Slider musicSlider;
    public Slider sfxSlider;
    public Slider voiceSlider;

    private Dictionary<string, object> settings = new Dictionary<string, object>();

    [Header("Audio")]
    [SerializeField] private AudioMixer mixer; // Drag your AudioMixer asset here in Inspector

    // [OLD CODE BELOW - Keeping for reference / potential future use / Testing]
    // Current state values
    public float masterVolume = 1f;
    public float musicVolume = 1f;
    public float sfxVolume = 1f;
    public float voiceVolume = 1f;

    [Header("Graphics - Settings State")]
    public bool isFullScreen = true;
    public int qualityLevel = 2; // Default quality level index
    public int resolutionIndex = 0; // Default resolution index

    private Resolution[] resolutions;

    private string settingsFile => Application.persistentDataPath + "/settings.json";

    // [OLD CODE BELOW - Keeping for reference / potential future use / Testing]
    //[Header("Settings State")]
    //public float masterVolume = 1f;
    //public bool isFullScreen = true;
    //public int qualityLevel = 2; // Default quality level index
    //public int resolutionIndex = 0; // Default resolution index
    //public AudioSource audioSource = null;

    public void RegisterUIObject(string key, object uiElement)
    {
        if (settings.ContainsKey(key))
        {
            settings[key] = uiElement;
            return;
        }

        Debug.Log("UI Element not found, did you use the wrong key?");
    }   

    private void Awake()
    {
        // [SINGLETON PATTERN]: If an instance already exists and it's not this -> destroy duplicate
        if (Instance != null && Instance != this)
        {
            Debug.LogWarning("Duplicate SettingsManager detected. Destroying extra instance.");
            Destroy(gameObject);
            return;
        }

        // Assign this as the instance
        Instance = this;

        // Persist across scene loads
        DontDestroyOnLoad(gameObject);

        // Initialize resolutions before loading
        resolutions = Screen.resolutions;

        // Load Settings if available.
        LoadSettings(); // Try Loadfing settings from file or PlayerPrefs. If none exist, defaults are used.


        // Store UI references in dictionary for easy access - also allows for dynamic assignment if needed
        settings.Add("DisplayMenu", displayMenu);
        settings.Add("SettingsMenu", settingsMenu);
        settings.Add("ResolutionDropdown", resolutionDropdown);
        settings.Add("QualityDropdown", qualityDropdown);
        settings.Add("MasterSlider", masterSlider);
        settings.Add("MusicSlider", musicSlider);
        settings.Add("SFXSlider", sfxSlider);
        settings.Add("VoiceSlider", voiceSlider);
    }

    public void UpdateRefences()
    {
        for (int i = 0; i < settings.Count; i++)
        {
            var key = new List<string>(settings.Keys)[i];
            GameObject value = (GameObject)settings[key];
            if (value != null)
            {
                // Attempt to find the UI element in the scene by name
                switch (key)
                {
                    case "DisplayMenu":
                        displayMenu = value;
                        break;
                    case "SettingsMenu":
                        settingsMenu = value;
                        break;
                    case "ResolutionDropdown":
                        resolutionDropdown = value.GetComponent<TMP_Dropdown>();
                        break;
                    case "QualityDropdown":
                        qualityDropdown = value.GetComponent<TMP_Dropdown>();
                        break;
                    case "MasterSlider":
                        masterSlider = value.GetComponent<Slider>();
                        break;
                    case "MusicSlider":
                        musicSlider = value.GetComponent<Slider>();
                        break;
                    case "SFXSlider":
                        sfxSlider = value.GetComponent<Slider>();
                        break;
                    case "VoiceSlider":
                        voiceSlider = value.GetComponent<Slider>();
                        break;
                }
            }
            else
            {
                Debug.LogWarning($"UI element for key '{key}' is null. Please ensure it is assigned correctly.");
            }
        }

        // Populate Resolution Dropdown
        SetupResolutionDropdown();
        SetupQualityDropdown();
        SetupVolumeSlider(); // Master volume slider setup - hanldes all (4) sliders
        SetupFullscreenToggle();

        LoadSettings();
    }

    private void Start()
    {
        // Populate Resolution Dropdown
        SetupResolutionDropdown();
        SetupQualityDropdown();
        SetupVolumeSlider(); // Master volume slider setup - hanldes all (4) sliders
        SetupFullscreenToggle();
    }

    // NEW: hook into scene load events so settings re-apply after any scene load
    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        ApplyAllSettings(); // re-apply settings after new scene loads
    }


    private void OnDestroy()
    {
        // Clear reference if this instance is destroyed
        if (Instance == this)
        {
            Instance = null;
        }
    }

    #region UI Setup
    private void SetupResolutionDropdown()
    {
        if (!resolutionDropdown) return;

        resolutionDropdown.ClearOptions();
        List<string> options = new List<string>();
        for (int i = 0; i < resolutions.Length; i++)
            options.Add(resolutions[i].width + " x " + resolutions[i].height);

        resolutionDropdown.AddOptions(options);
        resolutionDropdown.value = resolutionIndex;
        resolutionDropdown.RefreshShownValue();

        resolutionDropdown.onValueChanged.AddListener(SetResolutionFromDropdown);
    }

    private void SetupQualityDropdown()
    {
        if (!qualityDropdown) return;

        qualityDropdown.ClearOptions();
        qualityDropdown.AddOptions(new List<string>(QualitySettings.names));
        qualityDropdown.value = qualityLevel;
        qualityDropdown.RefreshShownValue();

        qualityDropdown.onValueChanged.AddListener(SetQuality);
    }

    private void SetupVolumeSlider()
    {

        if (masterSlider)
        {
            masterSlider.value = masterVolume;
            masterSlider.onValueChanged.AddListener(SetMasterVolume);
        }

        if (musicSlider)
        {
            musicSlider.value = musicVolume;
            musicSlider.onValueChanged.AddListener(SetMusicVolume);
        }

        if (sfxSlider)
        {
            sfxSlider.value = sfxVolume;
            sfxSlider.onValueChanged.AddListener(SetSFXVolume);
        }

        if (voiceSlider)
        {
            voiceSlider.value = voiceVolume;
            voiceSlider.onValueChanged.AddListener(SetVoiceVolume);
        }
    }

    private void SetupFullscreenToggle()
    {
        if (!fullscreenToggle) return;

        fullscreenToggle.isOn = isFullScreen;
        fullscreenToggle.onValueChanged.AddListener(ToggleFullScreen);
    }
    #endregion

    #region Audio
    // AUDIO Settings Methods
    private const string MasterVolParam = "MasterVolume";
    private const string MusicVolParam = "BGMVolume";
    private const string SFXVolParam = "SFXVolume";
    private const string VoiceVolParam = "VoiceVolume";
    public void SetMasterVolume(float value)
    {
        masterVolume = Mathf.Clamp01(value);
        mixer.SetFloat("MasterVolume", Mathf.Log10(Mathf.Max(value, 0.0001f)) * 20);
        SetVolume(MasterVolParam, masterVolume);
        SaveSettings();
    }

    public void SetMusicVolume(float value)
    {
        musicVolume = Mathf.Clamp01(value);
        SetVolume(MusicVolParam, musicVolume);
        SaveSettings();
    }

    public void SetSFXVolume(float value)
    {
        sfxVolume = Mathf.Clamp01(value);
        SetVolume(SFXVolParam, sfxVolume);
        SaveSettings();
    }

    public void SetVoiceVolume(float value)
    {
        voiceVolume = Mathf.Clamp01(value);
        SetVolume(VoiceVolParam, voiceVolume);
        SaveSettings();
    }

    private void SetVolume(string parameter, float value)
    {
        float dB = Mathf.Log10(Mathf.Max(value, 0.0001f)) * 20f;
        mixer.SetFloat(parameter, dB);
    }

    #endregion

    #region Graphics
    // Graphics settings methods
    public void SetQuality(int index)
    {
        qualityLevel = index;
        QualitySettings.SetQualityLevel(index);
        SaveSettings(); // Save settings after change
        Debug.Log("Quality level set to: " + QualitySettings.names[index]);
    }

    // [OLD CODE BELOW - Keeping for reference / potential future use / Testing]
    // Audio settings methods
    //public void SetMasterVolume(float value) 
    //{ 
    //    masterVolume = Mathf.Clamp01(value);
    //    mixer.SetFloat("MasterVolume", Mathf.Log10(value) * 20); 
    //    Debug.Log("Master Volume set to: " + masterVolume);
    //}

    public void SetResolutionFromDropdown(int index)
    {
        if (resolutions.Length == 0) return;
        Resolution res = resolutions[index];
        SetResolution(res.width, res.height, isFullScreen);
    }

    public void SetResolution(int width, int height, bool fullscreen)
    {
        Screen.SetResolution(width, height, fullscreen);
        isFullScreen = fullscreen;

        // Find and store matching index
        for (int i = 0; i < resolutions.Length; i++)
        {
            if (resolutions[i].width == width && resolutions[i].height == height)
            {
                resolutionIndex = i;
                break;
            }
        }

        SaveSettings(); // Save settings after change
    }

    public void ToggleFullScreen(bool enable)
    {
        isFullScreen = enable;
        Screen.fullScreen = enable;
        SaveSettings(); // Save settings after change
    }
    #endregion

    #region Save/Load
    // SAVE/LOAD Settings Methods
    [System.Serializable]
    private class SettingsData
    {
        public float masterVolume;
        public float musicVolume;
        public float sfxVolume;
        public float voiceVolume;
        public bool isFullScreen;
        public int qualityLevel;
        public int resolutionIndex;
    }

    private void SaveSettings()
    {
        SettingsData data = new SettingsData
        {
            masterVolume = masterVolume,
            musicVolume = musicVolume,
            sfxVolume = sfxVolume,
            voiceVolume = voiceVolume,
            isFullScreen = isFullScreen,
            qualityLevel = qualityLevel,
            resolutionIndex = resolutionIndex
        };

        File.WriteAllText(settingsFile, JsonUtility.ToJson(data, true));
    }

    private void LoadSettings()
    {
        if (File.Exists(settingsFile))
        {
            SettingsData data = JsonUtility.FromJson<SettingsData>(File.ReadAllText(settingsFile));

            masterVolume = data.masterVolume;
            musicVolume = data.musicVolume;
            sfxVolume = data.sfxVolume;
            voiceVolume = data.voiceVolume;
            isFullScreen = data.isFullScreen;
            qualityLevel = data.qualityLevel;
            resolutionIndex = data.resolutionIndex;

            ApplyAllSettings();
        }
        else
        {
            // First launch detect native monitor resolution
            Resolution native = Screen.currentResolution;
            Screen.SetResolution(native.width, native.height, true);

            for (int i = 0; i < resolutions.Length; i++)
            {
                if (resolutions[i].width == native.width && resolutions[i].height == native.height)
                {
                    resolutionIndex = i;
                    break;
                }
            }
            ApplyAllSettings();
            SaveSettings();
        }
    }

    public void ApplyAllSettings()
    {
        Debug.LogWarning($"Applying all settings... Master Vol: {masterVolume}, Music Vol: {musicVolume}, SFX Vol: {sfxVolume}, Voice Vol: {voiceVolume}");

        // Audio
        SetMasterVolume(masterVolume);
        SetMusicVolume(musicVolume);
        SetSFXVolume(sfxVolume);
        SetVoiceVolume(voiceVolume);

        // Quality
        QualitySettings.SetQualityLevel(qualityLevel);

        // Resolution
        if (resolutions.Length > resolutionIndex)
        {
            Resolution res = resolutions[resolutionIndex];
            Screen.SetResolution(res.width, res.height, isFullScreen);
        }
    }
    #endregion

    public void DisplayMenuOpen()
    {
        if (displayMenu) displayMenu.SetActive(true);
        if (settingsMenu) settingsMenu.SetActive(false);
    }

    public void OptionsMenuOpen()
    {
        if (settingsMenu) settingsMenu.SetActive(true);
        if (displayMenu) displayMenu.SetActive(false);
    }

}