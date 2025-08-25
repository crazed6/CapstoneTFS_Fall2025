using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

// Manages a dynamic list of save slots (UI entries).
// Handles refreshing, selection, load, and delete.
public class SaveSlotsManager : MonoBehaviour
{
    [Header("Scene References")]
    [SerializeField] private Transform contentRoot;       // ScrollView/Viewport/Content
    [SerializeField] private LoadSaveUIView saveDataSlotViewPrefab;   // Prefab for each save slot
    [SerializeField] private ConfirmDialog_UI confirmDialog; // Confirm delete dialog
    [SerializeField] private GameObject emptyState;       // Optional "No saves found" placeholder

    [Header("Global Buttons")]
    [SerializeField] private Button loadFileBtn;   // Load button outside the slots
    [SerializeField] private Button deleteFileBtn; // Delete button outside the slots

    private readonly List<LoadSaveUIView> _slots = new();
    private LoadSaveUIView _current;


    private void Awake()
    {
        // Hook global buttons to actions
        if (loadFileBtn != null)
        {
            loadFileBtn.onClick.RemoveAllListeners();
            loadFileBtn.onClick.AddListener(OnLoadClicked);
        }

        if (deleteFileBtn != null)
        {
            deleteFileBtn.onClick.RemoveAllListeners();
            deleteFileBtn.onClick.AddListener(OnDeleteClicked);
        }
    }

    private void OnEnable() => Refresh();

    /// Refreshes the slot list by rebuilding from existing save files.
   public void Refresh()
    {
        // Clear old slots
        foreach (var slot in _slots)
        {
            if (slot != null) Destroy(slot.gameObject);
        }
        _slots.Clear();
        _current = null;

        // Fetch all saves(newest first) and others as a List                   //***Reference Josh's SaveLoadSystem Script code.***//
        //var saves = SaveLoadSystem.GetAllSaves()
        //    .OrderByDescending(m => m.SavedUtc)
        //    .ToList();

        // Wrap Josh's single save file into a "meta" list (Binds meta list)
        var saves = BuildMetaList()
            .OrderByDescending(m => m.SavedUtc)
            .ToList();

        // Toggle "empty" UI if no saves
        if (emptyState != null) emptyState.SetActive(saves.Count == 0);

        // Spawn slot entries of the saved json game data files
        foreach (var meta in saves)
        {
            var slot = Instantiate(saveDataSlotViewPrefab, contentRoot);
            slot.Bind(meta, this); // each slot gets a reference to this manager for callbacks
            _slots.Add(slot);
        }

        // Auto-select first row (optional)
        if (_slots.Count > 0) OnSelectedSlot(_slots[0]);
    }

    /// <summary>
    /// Wraps Josh’s SaveLoadSystem into a single SaveMeta entry.
    /// </summary>
    private IEnumerable<SaveMeta> BuildMetaList()
    {
        string path = SaveLoadSystem.SaveFileName();
        if (!File.Exists(path))
            yield break;

        var info = new FileInfo(path);
        yield return new SaveMeta
        {
            FileName = info.Name,
            AbsolutePath = info.FullName,
            SceneName = SaveLoadSystem.GetSavedSceneName(),
            SavedUtc = info.LastWriteTimeUtc
        };
    }

    /// <summary>
    /// Called when user selects a slot (highlights it).
    /// </summary>
    public void OnSelectedSlot(LoadSaveUIView view)
    {
        if (_current == view) return;

        if (_current != null)
            _current.SetSelected(false);

        _current = view;
        _current.SetSelected(true);
    }

    /// <summary>
    /// Called when user presses "Load".
    /// Uses currently selected slot.
    /// </summary>
    public void OnLoadClicked()
    {
        if (_current == null) return;

        // Josh’s system does not support multiple files — always loads the single save
        SaveLoadSystem.Load();

        // If your UIManager needs to be notified, you can still hook in here
        // UIManager.Instance.LoadFromData(SaveLoadSystem.GetSaveData());
    }

    private void OnDeleteClicked()
    {
        if (_current == null) return;

        OnRequestDelete(_current.Meta);
    }

    /// <summary>
    /// Called when user presses "Delete".
    /// </summary>
    public async void OnRequestDelete(SaveMeta meta)
    {
        var ok = await confirmDialog.ShowAsync($"Delete '{meta.FileName}'?");
        if (!ok) return;

        if (File.Exists(meta.AbsolutePath))
            File.Delete(meta.AbsolutePath);

        Refresh();
    }
}
