using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerStatesManager : MonoBehaviour
{
    //Jaxson Vignal

    public static PlayerStatesManager instance;

    // Attach the player script & all the UI elements below in the editor
    public CharacterController player;

    [Header("UI - Speed")]
    public TextMeshProUGUI speedText;
    public TextMeshProUGUI velocityText;

    [Header("UI - States")]
    public TextMeshProUGUI groundedText;
    public TextMeshProUGUI movingText;
    public TextMeshProUGUI slidingText;
    public TextMeshProUGUI wallRunningText;

    float updateUITimer = 0.1f;

    void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
    }

    void Update()
    {
        updateUITimer -= Time.deltaTime;
        if (updateUITimer <= 0)
        {
            updateUITimer = 0.1f;

            float currSpeed = player.rb.linearVelocity.magnitude;

            SetSpeed(currSpeed);
            SetVelocity(player.rb.linearVelocity);

            SetMovingState(player.rb.linearVelocity.magnitude > 0.1f);
        }
    }

    void SetSpeed(float speed)
    {
        speedText.text = "speed: " + speed.ToString("F2");
    }

    void SetVelocity(Vector3 velocity)
    {
        velocityText.text = "velocity: " + velocity.ToString("F1");
    }

    public void SetGroundedState(bool state) { groundedText.color = state ? Color.green : Color.white; }
    void SetMovingState(bool state) { movingText.color = state ? Color.green : Color.white; }
    public void SetSlidingState(bool state) { slidingText.color = state ? Color.green : Color.white; }
    public void SetWallRunningState(bool state) { wallRunningText.color = state ? Color.green : Color.white; }
}
