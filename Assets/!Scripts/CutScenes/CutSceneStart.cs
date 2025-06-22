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
    }
    public override void Deactivate()
    {
        base.Deactivate();
    }
}

