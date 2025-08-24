using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LoadSaveUIView : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private Image selectionIcon;
    [SerializeField] private TMP_Text fileNameTxt;
    [SerializeField] private TMP_Text dateTxt;
    [SerializeField] private TMP_Text sceneTxt; // Not used in this script, but can be used to display the scene name if needed
    [SerializeField] private Button deleteGameDataBtn; // Button to delete game data
    [SerializeField] private Button selectBtn; // Button to select this save slot

    // References to Josh’s scripts
    //private SaveButton _saveButtonRef;
    //private LoadButton _loadButtonRef;

    private SaveSlotsManager _manager; // Reference to the parent manager that handles this view
    private SaveMeta _meta;
    //private Action<LoadSaveUIView> _onSelected;
    //private Action<SaveMeta> _onDelete;

    void Awake()
    {
        if (selectionIcon != null)
            selectionIcon.enabled = false;
    }

    // Initializes this slot with its data + manager callbacks.
    public void Bind(SaveMeta meta, SaveSlotsManager manager)
    {
        _meta = meta;
        _manager = manager;

        // Display info
        if (fileNameTxt != null) fileNameTxt.text = meta.FileName;
        if (dateTxt != null) dateTxt.text = meta.SavedUtc.ToLocalTime().ToString("yyyy/MM/dd");
        if (sceneTxt != null) sceneTxt.text = $"Scene: {meta.SceneName}";

        // Hook UI buttons
        if (deleteGameDataBtn != null)
        {
            deleteGameDataBtn.onClick.RemoveAllListeners();
            deleteGameDataBtn.onClick.AddListener(() => _manager.OnRequestDelete(_meta));
        }

        if (selectBtn != null)
        {
            selectBtn.onClick.RemoveAllListeners();
            selectBtn.onClick.AddListener(() => _manager.OnSelectedSlot(this));
        }
    }
    // Highlights this slot as selected.
    public void SetSelected(bool selected)
    {
        if (selectionIcon != null)
            selectionIcon.enabled = selected;
    }

    public SaveMeta Meta => _meta;
}
