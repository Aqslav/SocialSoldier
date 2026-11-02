using UnityEngine;

public class LookTowardsUser : MonoBehaviour
{
    public bool locked = false;
    public float rotationSpeed = 5f;
    public float maxAngle = 0f;
    public bool OnlyYAxis = false;
    private GameObject userCamera;
    private Quaternion initialRotation;
    public bool returnToInitialRotation = false;
    public bool allowTwisting = false;
    public void Lock()
    {
        locked = true;
    }
    public void Unlock()
    {
        locked = false;
    }
    void Start()
    {
        userCamera = Camera.main.gameObject;
        initialRotation = transform.localRotation;
    }
    bool isTwisting(Quaternion from, Quaternion to)
    {
        if (transform.parent == null)
            return false;

        Vector3 fromForward = from * Vector3.forward;
        Vector3 toForward = to * Vector3.forward;
        Vector3 oppositeParentForward = -transform.parent.forward;

        float totalAngle = Vector3.SignedAngle(fromForward, toForward, transform.up);
        float parentAngle = Vector3.SignedAngle(
            fromForward,
            oppositeParentForward,
            transform.up
        );

        return parentAngle * totalAngle >= 0f &&
            Mathf.Abs(parentAngle) <= Mathf.Abs(totalAngle);
    }
    Quaternion AvoidTwist(Quaternion targetRotation)
    {
        if (allowTwisting || !isTwisting(transform.rotation, targetRotation))
            return targetRotation;
        float totalAngle = Vector3.SignedAngle(transform.forward, targetRotation * Vector3.forward, transform.up);
        return Quaternion.AngleAxis(-Mathf.Sign(totalAngle) * 90f, transform.up) * transform.rotation;
    }
    void TurnTo(Quaternion targetRotation)
    {
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);
    }
    void Update()
    {
        if(locked && userCamera != null)
        {
            Vector3 targetPosition = userCamera.transform.position;
            if(OnlyYAxis)
            {
                targetPosition = new Vector3(targetPosition.x, transform.position.y, targetPosition.z);
            }
            Vector3 direction = targetPosition - transform.position;
            float angle = Vector3.Angle(transform.forward, direction);
            if (direction != Vector3.zero && angle > maxAngle)
            {
                Quaternion targetRotation = Quaternion.LookRotation(direction);
                TurnTo(AvoidTwist(targetRotation));
            }
        }
        else if(!locked && returnToInitialRotation)
        {
            Quaternion localRotation = transform.parent != null ? transform.parent.rotation * initialRotation : initialRotation;
            TurnTo(AvoidTwist(localRotation));
        }
    }
}
