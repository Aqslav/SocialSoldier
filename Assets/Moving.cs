using UnityEngine;

public class Moving : MonoBehaviour
{
    public float speed = 1f;
    public float jumpForce = 5f;
    private Rigidbody rb;
    private Collider col;
    public LayerMask groundMask;
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        col = GetComponent<Collider>();
    }

    void Update()
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
    bool IsGrounded()
    {
        Vector3 origin = new Vector3(col.bounds.center.x, col.bounds.min.y + 0.1f, col.bounds.center.z);
        return Physics.Raycast(origin, Vector3.down, 0.2f, groundMask, QueryTriggerInteraction.Ignore);
    }
}
