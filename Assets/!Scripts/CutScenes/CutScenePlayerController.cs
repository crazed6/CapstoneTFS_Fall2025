using UnityEngine;

public class CutScenePlayerController : MonoBehaviour
{
    public static CutScenePlayerController Instance;
    [SerializeField] private CharacterController _CharacterController;
    [SerializeField] private GameObject _Player;
    [SerializeField] private CinemachineCameraController _CinemachineCameraController;


    private void Awake()
    {
        Instance = this;
    }

    public void Activate()
    {
        _CharacterController.enabled = true;
        _Player.SetActive(true);
        _CinemachineCameraController.enabled = true;

    }
    public void Deactivate()
    {
        _CharacterController.enabled = false;
        _Player.SetActive(false);
        _CinemachineCameraController.enabled = false;
    }
}
