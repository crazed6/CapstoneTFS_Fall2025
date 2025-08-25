using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ConfirmDialog_UI : MonoBehaviour
{
    [SerializeField] private TMP_Text messageText;
    [SerializeField] private Button confirmButton;
    [SerializeField] private Button cancelButton;

    private TaskCompletionSource<bool> _tcs;

    private void Awake()
    {
        confirmButton.onClick.AddListener(() => Close(true));
        cancelButton.onClick.AddListener(() => Close(false));
        gameObject.SetActive(false); // Keeps Hiden Until Shown.
    }

    public Task<bool> ShowAsync(string message)
    {
        messageText.text = message;
        gameObject.SetActive(true);
        _tcs = new TaskCompletionSource<bool>();
        return _tcs.Task;
    }

    private void Close(bool result)
    {
        gameObject.SetActive(false);
        _tcs?.TrySetResult(result);
    }
}
