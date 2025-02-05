using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections.Generic;
using UnityEngine.Audio;
using UnityEngine.InputSystem;

public class MenuSettings : MonoBehaviour
{
    [Header("Resolution Settings")]
    public TMP_Dropdown resolutionDropdown;
    Resolution[] resolutions;

    [Header("Audio Settings")]
    public AudioMixer audioMixer;

    [Header("Key Rebinding")]
    public InputActionAsset inputActions; // Assign Input Actions asset in Inspector
    public Button forwardRebindButton;
    public TMP_Text forwardKeyText;
    public Button backwardRebindButton;
    public TMP_Text backwardKeyText;
    public Button leftRebindButton;
    public TMP_Text leftKeyText;
    public Button rightRebindButton;
    public TMP_Text rightKeyText;
    public Button jumpRebindButton;
    public TMP_Text jumpKeyText;
    public Button sprintRebindButton;
    public TMP_Text sprintKeyText;
    public Button interactRebindButton;
    public TMP_Text interactKeyText;
    public Button crouchRebindButton;
    public TMP_Text crouchKeyText;
    public Button attackRebindButton;
    public TMP_Text attackKeyText;
    public Button resetButton;

    private InputActionRebindingExtensions.RebindingOperation rebindingOperation;
    private bool isRebinding = false; // Prevent multiple rebinds

    private void Start()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        SetupResolutionDropdown();
        LoadRebindings();
        SetupRebindButtons();
        resetButton.onClick.AddListener(ResetKeybindings);
    }

    // ============================
    // RESOLUTION & GRAPHICS SETTINGS
    // ============================
    private void SetupResolutionDropdown()
    {
        resolutions = Screen.resolutions;
        resolutionDropdown.ClearOptions();
        List<string> options = new List<string>();

        int currentResolutionIndex = 0;
        for (int i = 0; i < resolutions.Length; i++)
        {
            string option = resolutions[i].width + "x" + resolutions[i].height;
            options.Add(option);

            if (resolutions[i].width == Screen.currentResolution.width &&
                resolutions[i].height == Screen.currentResolution.height)
            {
                currentResolutionIndex = i;
            }
        }

        resolutionDropdown.AddOptions(options);
        resolutionDropdown.value = currentResolutionIndex;
        resolutionDropdown.RefreshShownValue();
    }

    public void SetResolution(int resolutionIndex)
    {
        Resolution resolution = resolutions[resolutionIndex];
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
    }

    public void SetQuality(int qualityIndex)
    {
        QualitySettings.SetQualityLevel(qualityIndex);
    }

    public void SetFullscreen(bool isFullscreen)
    {
        Screen.fullScreen = isFullscreen;
    }

    // ============================
    // AUDIO SETTINGS
    // ============================
    public void SetMasterVolume(float volume)
    {
        audioMixer.SetFloat("MasterVolume", Mathf.Log10(volume) * 20);
    }

    public void SetMusicVolume(float volume)
    {
        audioMixer.SetFloat("MusicVolume", Mathf.Log10(volume) * 20);
    }

    public void SetSFXVolume(float volume)
    {
        audioMixer.SetFloat("SFXVolume", Mathf.Log10(volume) * 20);
    }

    public void SetVoiceVolume(float volume)
    {
        audioMixer.SetFloat("VoiceVolume", Mathf.Log10(volume) * 20);
    }

    // ============================
    // KEY REBINDING FUNCTIONALITY
    // ============================
    private void LoadRebindings()
    {
        if (PlayerPrefs.HasKey("KeyBindings"))
        {
            inputActions.LoadBindingOverridesFromJson(PlayerPrefs.GetString("KeyBindings"));
        }
        else
        {
            SaveDefaultBindings(); // Save default bindings if no saved data exists
        }

        UpdateAllBindingTexts();
    }

    private void SaveDefaultBindings()
    {
        PlayerPrefs.SetString("DefaultKeyBindings", inputActions.SaveBindingOverridesAsJson());
        PlayerPrefs.Save();
    }

    private void UpdateAllBindingTexts()
    {
        UpdateBindingText("Move", 2, forwardKeyText);
        UpdateBindingText("Move", 4, backwardKeyText);
        UpdateBindingText("Move", 6, leftKeyText);
        UpdateBindingText("Move", 8, rightKeyText);
        UpdateBindingText("Jump", 0, jumpKeyText);
        UpdateBindingText("Sprint", 0, sprintKeyText);
        UpdateBindingText("Interact", 0, interactKeyText);
        UpdateBindingText("Crouch", 1, crouchKeyText);
        UpdateBindingText("Attack", 1, attackKeyText);
    }

    private void UpdateBindingText(string actionName, int bindingIndex, TMP_Text textElement)
    {
        var action = inputActions.FindAction(actionName);
        if (action != null && bindingIndex < action.bindings.Count)
        {
            string bindingString = action.GetBindingDisplayString(bindingIndex);

            // Remove "Hold " if it appears in the text
            if (bindingString.StartsWith("Hold "))
            {
                bindingString = bindingString.Replace("Hold ", "");
            }

            textElement.text = string.IsNullOrEmpty(bindingString) ? "Not Bound" : bindingString;
        }
        else
        {
            textElement.text = "Not Bound";
        }
    }

    public void StartRebinding(string actionName, int bindingIndex, TMP_Text textElement)
    {
        if (isRebinding) return; // Prevent multiple simultaneous rebinding
        isRebinding = true;

        var action = inputActions.FindAction(actionName);
        if (action == null || bindingIndex >= action.bindings.Count) return;

        textElement.text = "Press Any Key...";
        action.Disable(); // Disable the action while rebinding

        rebindingOperation = action.PerformInteractiveRebinding(bindingIndex)
            .OnComplete(operation =>
            {
                string newBindingPath = action.bindings[bindingIndex].effectivePath;
                Debug.Log($"Rebinding to: {newBindingPath}"); // Add this debug log

                // Resolve Conflicts: Clear previous binding if another action used the new key
                ResolveConflictingBindings(newBindingPath);

                // Apply the new binding
                action.ApplyBindingOverride(bindingIndex, newBindingPath);

                // Update UI text with new key
                textElement.text = action.GetBindingDisplayString(bindingIndex);

                rebindingOperation.Dispose();
                action.Enable(); // Re-enable action
                SaveRebindings();

                isRebinding = false; // Allow new rebinding
            })
            .Start();
    }

    private void ResolveConflictingBindings(string newBindingPath)
    {
        foreach (var action in inputActions)
        {
            for (int i = 0; i < action.bindings.Count; i++)
            {
                if (action.bindings[i].effectivePath == newBindingPath)
                {
                    Debug.Log($"Clearing {action.name}'s binding {newBindingPath} due to conflict");

                    action.ApplyBindingOverride(i, string.Empty);
                    action.Enable(); // Ensure it's updated in runtime
                }
            }
        }
    }


    private void SaveRebindings()
    {
        string rebinds = inputActions.SaveBindingOverridesAsJson();
        PlayerPrefs.SetString("KeyBindings", rebinds);
        PlayerPrefs.Save();

        UpdateAllBindingTexts(); // Make sure UI updates!
    }

    private void ResetKeybindings()
    {
        if (PlayerPrefs.HasKey("DefaultKeyBindings"))
        {
            inputActions.LoadBindingOverridesFromJson(PlayerPrefs.GetString("DefaultKeyBindings"));
        }
        else
        {
            inputActions.RemoveAllBindingOverrides(); // Fallback if no default exists
        }

        PlayerPrefs.DeleteKey("KeyBindings");
        LoadRebindings();
    }

    private void SetupRebindButtons()
    {
        forwardRebindButton.onClick.AddListener(() => StartRebinding("Move", 2, forwardKeyText));
        backwardRebindButton.onClick.AddListener(() => StartRebinding("Move", 4, backwardKeyText));
        leftRebindButton.onClick.AddListener(() => StartRebinding("Move", 6, leftKeyText));
        rightRebindButton.onClick.AddListener(() => StartRebinding("Move", 8, rightKeyText));
        jumpRebindButton.onClick.AddListener(() => StartRebinding("Jump", 0, jumpKeyText));
        sprintRebindButton.onClick.AddListener(() => StartRebinding("Sprint", 0, sprintKeyText));
        interactRebindButton.onClick.AddListener(() => StartRebinding("Interact", 0, interactKeyText));
        crouchRebindButton.onClick.AddListener(() => StartRebinding("Crouch", 1, crouchKeyText));
        attackRebindButton.onClick.AddListener(() => StartRebinding("Attack", 1, attackKeyText));
    }

    // ============================
    // SCENE MANAGEMENT
    // ============================
    public void MainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
