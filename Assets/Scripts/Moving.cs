using UnityEngine;
[System.Serializable]
public enum InputType
{
    Movement,
    Menu
}
public class Moving : MonoBehaviour
{
    public float speed = 1f;
    public float runMultiplier = 2f;
    public float jumpForce = 5f;
    public float crouchHeight = 0.5f;
    public float crouchSpeedMultiplier = 0.5f;
    private float originalHeight;
    private Rigidbody rb;
    private Collider col;
    public LayerMask groundMask;
    [SerializeField] private InputType inputMode;
    public InputType InputMode
    {
        get { return inputMode; }
        set 
        { 
            inputMode = value; 
            if (inputMode == InputType.Menu)
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
            else if (inputMode == InputType.Movement)
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
        }
    }
    public void SetInputMode(int mode)
    {
        InputMode = (InputType)mode;
    }

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        col = GetComponent<Collider>();
        originalHeight = transform.localScale.y;
        inputMode = InputType.Movement;
    }
    private void HandleMovement()
    {
        float localSpeed = speed;
        if(InputManager.Crouch())
        {
            localSpeed *= crouchSpeedMultiplier;
            transform.localScale = new Vector3(transform.localScale.x, crouchHeight, transform.localScale.z);
        }
        else
        {
            transform.localScale = new Vector3(transform.localScale.x, originalHeight, transform.localScale.z);
            if (InputManager.Run())
            {
                localSpeed *= runMultiplier;
            }
        }

        transform.position += transform.forward * InputManager.Vertical() * Time.deltaTime * localSpeed;
        transform.position += transform.right * InputManager.Horizontal() * Time.deltaTime * localSpeed;
        if (InputManager.Jump() && IsGrounded())
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }
        if(!InputManager.RightClickHeld())
        {
            InputManager.Turn(gameObject, Camera.main.gameObject);
        }
    }
    void Update()
    {
        if (inputMode == InputType.Movement)
        {
            HandleMovement();
        }
        else if (inputMode == InputType.Menu)
        {
            // Nothing to do here
        }

    }
    bool IsGrounded()
    {
        Vector3 origin = new Vector3(col.bounds.center.x, col.bounds.min.y + 0.1f, col.bounds.center.z);
        return Physics.Raycast(origin, Vector3.down, 0.2f, groundMask, QueryTriggerInteraction.Ignore);
    }
}
