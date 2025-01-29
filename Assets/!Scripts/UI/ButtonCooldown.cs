using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ButtonCountdown : MonoBehaviour
{
    public Button buttonC, buttonV, buttonB; 
    public TextMeshProUGUI textC, textV, textB; 

    private bool isCountingC = false, isCountingV = false, isCountingB = false;

    void Start()
    {
        // Add listeners to buttons (optional if clicked with a mouse)
        buttonC.onClick.AddListener(() => StartCountdown(buttonC, textC, "C", "C"));
        buttonV.onClick.AddListener(() => StartCountdown(buttonV, textV, "V", "V"));
        buttonB.onClick.AddListener(() => StartCountdown(buttonB, textB, "B", "B"));
    }

    void Update()
    {
        // Check for keyboard input and start countdown if the key is pressed
        if (Input.GetKeyDown(KeyCode.C) && !isCountingC)
        {
            StartCountdown(buttonC, textC, "C", "C");
        }
        if (Input.GetKeyDown(KeyCode.V) && !isCountingV)
        {
            StartCountdown(buttonV, textV, "V", "V");
        }
        if (Input.GetKeyDown(KeyCode.B) && !isCountingB)
        {
            StartCountdown(buttonB, textB, "B", "B");
        }
    }

    void StartCountdown(Button button, TextMeshProUGUI text, string originalLetter, string identifier)
    {
        if (identifier == "C" && isCountingC) return;
        if (identifier == "V" && isCountingV) return;
        if (identifier == "B" && isCountingB) return;

        StartCoroutine(CountdownRoutine(button, text, originalLetter, identifier));
    }

    IEnumerator CountdownRoutine(Button button, TextMeshProUGUI text, string originalLetter, string identifier)
    {
        // Set the appropriate counting flag
        if (identifier == "C") isCountingC = true;
        if (identifier == "V") isCountingV = true;
        if (identifier == "B") isCountingB = true;

        button.interactable = false; // Disable the button visually
        for (int i = 5; i > 0; i--)
        {
            text.text = i.ToString(); // Show countdown
            yield return new WaitForSeconds(1f); // Wait 1 second
        }

        text.text = originalLetter; // Reset to the original letter
        button.interactable = true; // Re-enable the button

        // Reset the appropriate counting flag
        if (identifier == "C") isCountingC = false;
        if (identifier == "V") isCountingV = false;
        if (identifier == "B") isCountingB = false;
    }
}