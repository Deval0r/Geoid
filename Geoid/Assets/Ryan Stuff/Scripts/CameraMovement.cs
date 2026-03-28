using UnityEngine;
using UnityEngine.InputSystem;
public class CameraMovement : MonoBehaviour
{
   public float mouseSensitivity = 10;
    public Transform playerTransform;
    private float yRotation = 0;
    private Vector3 initialCameraPos;
    public float bobFrequency = 10;
    public float bobAmplitude = 0.05f;
    private float bobTimer = 0;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //lock cursor to game to prevent weird stuff
        Cursor.lockState = CursorLockMode.Locked;
        initialCameraPos = transform.localPosition;
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 mouseDelta = Mouse.current.delta.ReadValue();
        //get the axis and scale it appropiately based on mouse sensitivity
        float mouseX = mouseDelta.x * mouseSensitivity;
        float mouseY = mouseDelta.y * mouseSensitivity;
        yRotation -= mouseY;
        //rotate camera

        //clamp rotation to edges of screen (90 degrees either way)
        yRotation = Mathf.Clamp(yRotation, -90, 90);
        //set camera rotation based on this
        transform.localRotation = Quaternion.Euler(yRotation, 0, 0);

        //rotate player
        playerTransform.Rotate(Vector3.up * mouseX);
        // camera bobbing
        // get players velocity
        Vector3 horizontalMovement = new Vector3(playerTransform.GetComponent<Rigidbody>().linearVelocity.x, 0,
        playerTransform.GetComponent<Rigidbody>().linearVelocity.z);
        //get velocities magnitude
        float speed = horizontalMovement.magnitude;
        //if moving quick enough, bob
        if (speed > 0.1f)
        {
            bobTimer += Time.deltaTime * bobFrequency;
            float speedFactor = Mathf.Clamp(horizontalMovement.magnitude / 6, 0f, 1f);
            float yOffset = Mathf.Sin(bobTimer) * bobAmplitude * speedFactor;
            transform.localPosition = initialCameraPos + new Vector3(0, yOffset, 0);
        }
        else
        {

            bobTimer = 0;
            transform.localPosition = initialCameraPos;
        }
    }
}
