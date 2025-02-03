using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseManager : MonoBehaviour
{
    private static PauseManager instance;
    private bool isPaused = false;
    private string previousSceneName = ""; 

    
    public static PauseManager Instance
    {
        get
        {
            if (instance == null)
            {
                Debug.LogError("PauseManager instance is null!");
            }
            return instance;
        }
    }

    void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
            {
                ResumeGame();
            }
            else
            {
                PauseGame();
            }
        }
    }

    public void PauseGame()
    {
        previousSceneName = SceneManager.GetActiveScene().name;

        isPaused = true;
        Time.timeScale = 0f;
        SceneManager.LoadScene("PauseMenu", LoadSceneMode.Additive);
    }

    public void ResumeGame()
    {
        isPaused = false;
        Time.timeScale = 1f;
        SceneManager.UnloadSceneAsync("PauseMenu");
    }
}
