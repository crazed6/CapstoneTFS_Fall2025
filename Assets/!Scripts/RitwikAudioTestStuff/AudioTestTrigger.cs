using UnityEngine;

public class AudioTestTrigger : MonoBehaviour
{
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
            AudioManager.instance.PlaySFX("Jump");
    }
}
