using UnityEngine;
using UnityEngine.InputSystem;

using Vector3 = UnityEngine.Vector3;
using Vector2 = UnityEngine.Vector2;
using Quaternion = UnityEngine.Quaternion;
using System;

public class CameraMovement : MonoBehaviour
{
    public float mouseSensitivity = 0.1f;
    public Transform playerTransform;
    public Rigidbody playerRigidbody;

    [Header("Head Bob Settings")]
    public float bobFrequency = 15f;
    public float bobHorizontalAmplitude = 0.05f;
    public float bobVerticalAmplitude = 0.05f;
    public float headRollAngle = 0.75f;

    private float yRotation = 0;
    private float bobTimer = 0;
    private float bobFade = 0; // New variable to track smoothing
    private Vector3 initialCameraPos;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        initialCameraPos = transform.localPosition;

        if (playerRigidbody == null)
            playerRigidbody = playerTransform.GetComponent<Rigidbody>();
    }

    void Update()
    {
        // 1. Mouse Look
        Vector2 mouseDelta = Mouse.current.delta.ReadValue();
        yRotation -= mouseDelta.y * mouseSensitivity;
        yRotation = Mathf.Clamp(yRotation, -90, 90);

        transform.localRotation = Quaternion.Euler(yRotation, 0, 0);
        playerTransform.Rotate(Vector3.up * (mouseDelta.x * mouseSensitivity));

        HandleHeadBob();
    }

    private void HandleHeadBob()
    {
        Vector3 horizontalVelocity = new Vector3(playerRigidbody.linearVelocity.x, 0, playerRigidbody.linearVelocity.z);
        float speed = horizontalVelocity.magnitude;
        bool isGrounded = Physics.Raycast(playerTransform.position, Vector3.down, 1.1f);

        // Instead of bobTimer = 0, we move 'bobFade' toward 1 or 0
        if (speed > 0.1f && isGrounded)
        {
            bobFade = Mathf.Lerp(bobFade, 1f, Time.deltaTime * 5f);
            // Increment timer based on speed but capped to prevent "diagonal turbo"
            bobTimer += Time.deltaTime * bobFrequency * Mathf.Clamp(speed / 5f, 0.5f, 1.5f);
        }
        else
        {
            bobFade = Mathf.Lerp(bobFade, 0f, Time.deltaTime * 5f);
            // Let the timer continue to run slightly so the "wave" finishes naturally
            bobTimer += Time.deltaTime * (bobFrequency * 0.5f * bobFade);
        }

        // Apply bobbing multiplied by the fade value
        float xOffset = Mathf.Cos(bobTimer * 0.5f) * bobHorizontalAmplitude * bobFade;
        float yOffset = Mathf.Sin(bobTimer) * bobVerticalAmplitude * bobFade;
        float zTilt = Mathf.Cos(bobTimer * 0.5f) * headRollAngle * bobFade;

        transform.localPosition = initialCameraPos + new Vector3(xOffset, yOffset, 0);

        // Final Rotation: Mouse Look Pitch + Head Roll
        transform.localRotation = Quaternion.Euler(yRotation, 0, zTilt);
    }
}
