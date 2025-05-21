using UnityEngine;

public class ESCtoExit : MonoBehaviour
{
   

    // Update is called once per frame
    void Update()
    {
        closeGame();
    }

    void closeGame()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }
    }
}
