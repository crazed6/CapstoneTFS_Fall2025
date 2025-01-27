using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseManager : MonoBehaviour
{
    private static PauseManager instance; 
    

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
            if (SceneManager.GetActiveScene().name == "PauseMenu")
            {
                LoadingForSampleScene();
            }
            
            {
                GoToPauseMenu(); 
            }
        }
    }

    
    private void GoToPauseMenu()
    {
        
        SceneManager.LoadScene("PauseMenu");
    }

    


    
    public void LoadingForSampleScene()
    {
        SceneManager.LoadScene("SampleScene");
    }

    

}
