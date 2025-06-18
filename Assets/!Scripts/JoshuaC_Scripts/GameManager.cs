using NUnit.Framework;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    private static GameManager instance;
    public static GameManager Instance
    {
        get
        {
#if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                return null;
            }

            if (instance == null)
            {
                //Instantiate(Resources.Load<GameManager>("GameManager"));
                GameObject go = new GameObject("GameManager");
                instance = go.AddComponent<GameManager>();
                DontDestroyOnLoad(go);
            }
#endif
            return instance;
        }
    }

    public PlayerSave PlayerSave { get; private set; }
    //public CollectibleManager CollectibleManager { get; set; }


    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            SceneManager.sceneLoaded += OnSceneLoaded; //Used to subscribe to Scene Loaded Event
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Update()
    {
        if (Keyboard.current.digit1Key.wasPressedThisFrame)
        {
            SaveLoadSystem.Save();
            Debug.Log("The system was just saved.");
        }

        if (Keyboard.current.digit2Key.wasPressedThisFrame)
        {
            SaveLoadSystem.Load();
            Debug.Log("The system was just loaded.");
        }
    }

    private void OnSceneLoaded(UnityEngine.SceneManagement.Scene BuildTesting, UnityEngine.SceneManagement.LoadSceneMode mode)
    {
        PlayerSave = Object.FindFirstObjectByType<PlayerSave>();

        if (PlayerSave == null)
        {
            Debug.LogWarning("PlayerSave not found in this scene.");
        }
        else
        {
            Debug.Log("PlayerSave assigned successfully!");
        }
    }
}
