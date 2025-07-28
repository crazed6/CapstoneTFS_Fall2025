using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuButton : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ChangeScene(string SceneName)
    {
        //SoundManager.Instance.StopAudio();    ---causing errors with new SoundManager.cs
        SceneManager.LoadSceneAsync(SceneName);
    }
}
