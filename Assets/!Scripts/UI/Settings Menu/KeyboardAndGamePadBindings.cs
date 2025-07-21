using UnityEngine;
using UnityEngine.SceneManagement;

public class KeyboardAndGamePadBindings : MonoBehaviour
{
    public void GoToKeyboardBindings()
    {
        // This method will be used to switch to the Keyboard bindings section in the settings menu.
        // Implementation will depend on how the UI is structured.
        SceneManager.LoadSceneAsync("Kadeem_KeyBoardBindings"); 
    }

    public void GoToGamePadBindings()
    {
        // This method will be used to switch to the GamePad bindings section in the settings menu.
        // Implementation will depend on how the UI is structured.
        SceneManager.LoadSceneAsync("Kadeem_GamePadBindings"); 
    }
}
