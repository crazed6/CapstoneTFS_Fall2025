// Colton Reed
using UnityEngine;
using UnityEngine.Timeline;

[RequireComponent(typeof(BoxCollider))]
[RequireComponent(typeof(SignalReceiver))]
public class CutSceneStart : Interactable
{
    [SerializeField] private GameObject _cutSceneToStart;
    public override void Activate()
    {
        base.Activate();
        _cutSceneToStart.SetActive(true);
        CharacterController.instance.cutSceneCamera.SetActive(true);
        CharacterController.instance.cutScenePlayerCamera.SetActive(true);
        CharacterController.instance.thirdPersonCamera.SetActive(false);
    }
    public override void Deactivate()
    {
        base.Deactivate();
        _cutSceneToStart.SetActive(false);
        CharacterController.instance.cutSceneCamera.SetActive(false);
        CharacterController.instance.cutScenePlayerCamera.SetActive(false);
        CharacterController.instance.thirdPersonCamera.SetActive(true);
    }
}

