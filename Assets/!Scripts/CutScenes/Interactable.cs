using UnityEngine;

public abstract class Interactable : MonoBehaviour
{
    private bool _playerWithinRange;
    [SerializeField] private CanvasGroup _interactableUI;


    private void OnTriggerEnter(Collider col)
    {
        if (col.gameObject.CompareTag("Player"))
        {
            _interactableUI.gameObject.SetActive(true);
            LeanTween.cancel(_interactableUI.gameObject);
            LeanTween.alphaCanvas(_interactableUI, 1f, 1f);
            _playerWithinRange = true;
        }
    }

    private void Update()
    {
        if (_playerWithinRange && Input.GetKeyDown(KeyCode.E))
        {
            Activate();
        }
    }


    public virtual void Activate()
    {
        _interactableUI.gameObject.SetActive(false);
    }
    public virtual void Deactivate()
    {

    }

    private void OnTriggerExit(Collider col)
    {
        if (col.gameObject.CompareTag("Player"))
        {
            _playerWithinRange = false;
            LeanTween.alphaCanvas(_interactableUI, 0f, 1f).setOnComplete(UIHide);
        }
    }

    private void UIHide()
    {
        
    }
}
