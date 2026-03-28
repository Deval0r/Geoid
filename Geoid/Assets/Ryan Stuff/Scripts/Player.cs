using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
public class Player : MonoBehaviour
{
    private Keyboard keyboard;
    public Rigidbody rb;
    private int moveDirZ;
    private int moveDirX;
    private float moveSpd = 3;
    private float sprintSpd = 6;
    private bool sprinting;
    public LayerMask groundLayer;
    private bool canJump;
    public CapsuleCollider capsuleCollider;
    private float jumpSpd = 6;
    public bool isGrounded;
    public BoxCollider boxCollider;
    public bool crouching;
    public float currentSpd;
    public static Player Instance { get; private set;}
    public float Health=100;
    public float Stamina=100;
    public float StaminaRate=6;
    public float StaminaRecharge=2;
    public float enemyKnockbackForce=10;
    public Vector3 knockbackVelocity;
    private void Start()
    {
        keyboard = Keyboard.current;
        Health=100;
        Stamina=100;
    }
    private void Awake()
    {
        Instance=this;
    }
    private void Update()
    {
        moveDirX = 0;
        moveDirZ = 0;
        if (keyboard.wKey.isPressed)
        {
            moveDirZ += 1;
        }
        if (keyboard.sKey.isPressed)
        {
            moveDirZ -= 1;
        }
        if (keyboard.aKey.isPressed)
        {
            moveDirX -= 1;
        }
        if (keyboard.dKey.isPressed)
        {
            moveDirX += 1;
        }
        if (keyboard.shiftKey.isPressed)
        {
            sprinting = true;
        }
        else
        {
            sprinting = false;
        }
        isGrounded = IsGrounded();
        canJump = isGrounded;
        if (keyboard.spaceKey.wasPressedThisFrame && canJump)
        {
            Jump();
        }
        if (Health<=0)
        {SceneManager.LoadScene(1);}
    }
    private void FixedUpdate()
    {
        if (sprinting&&Stamina>0)
        {
            currentSpd = sprintSpd;
            Stamina-=StaminaRate*Time.fixedDeltaTime;
        }
        else
        {
            if (Stamina<100)
            {Stamina+=StaminaRecharge*Time.fixedDeltaTime;}
            currentSpd = moveSpd;
        }
        // set axis
        Vector3 forward = transform.forward;
        Vector3 right = transform.right;
        //stop from going up
        forward.y = 0f;
        right.y = 0f;
        //normalize (stops  going diagonal from being faster)
        forward.Normalize();
        right.Normalize();
        //calculate movement
        Vector3 move = forward * moveDirZ + right * moveDirX;
        rb.linearVelocity = new Vector3(move.x * currentSpd, rb.linearVelocity.y, move.z * currentSpd)+knockbackVelocity;
        if (keyboard.ctrlKey.isPressed)
        {
            capsuleCollider.enabled = false;
            boxCollider.enabled = true;
            crouching = true;
        }
        else
        {
            capsuleCollider.enabled = true;
            boxCollider.enabled = false;
            crouching = false;
        }
    }
    private bool IsGrounded()
    {
        //return false;
        float extraHeightTest = 0.1f;
        if (!crouching)
        { return Physics.BoxCast(capsuleCollider.bounds.center, capsuleCollider.bounds.extents - new Vector3(0.1f, 0.1f, 0f), Vector3.down, Quaternion.identity, extraHeightTest, groundLayer); }
        else
        { return Physics.BoxCast(boxCollider.bounds.center, boxCollider.bounds.extents - new Vector3(0.1f, 0.1f, 0f), Vector3.down, Quaternion.identity, extraHeightTest, groundLayer); }

    }
    private void Jump()
    {
        rb.AddForce(Vector3.up * jumpSpd, ForceMode.Impulse);
    }
    private void OnCollisionEnter(Collision collision)
    {
    }
}
