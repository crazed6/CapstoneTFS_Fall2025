using UnityEngine;
using UnityEngine.VFX;

public class PlayerAnimationVFX : MonoBehaviour
{
    [Header("Slide VFX")]
    [SerializeField] private VisualEffect slideVFX;

    public void BeginSlideVFX()
    {
        if (slideVFX != null)
        {
            slideVFX.gameObject.SetActive(true); // enable the object
            slideVFX.Play(); // play the VFX
            Debug.Log("BeginSlideVFX called");
        }
    }

    public void EndSlideVFX()
    {
        if (slideVFX != null)
        {
            slideVFX.Stop(); // stop emission
            slideVFX.gameObject.SetActive(false); // hide the object
            Debug.Log("EndSlideVFX called");
        }
    }
}
