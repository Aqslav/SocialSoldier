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
    public float jumpForce = 5f;
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
        inputMode = InputType.Movement;
    }
    private void HandleMovement()
    {
        transform.position += transform.forward * InputManager.Vertical() * Time.deltaTime * speed;
        transform.position += transform.right * InputManager.Horizontal() * Time.deltaTime * speed;
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
