using UnityEngine;

public class LookTowardsUser : MonoBehaviour
{
    public bool locked = false;
    public float rotationSpeed = 5f;
    public float maxAngle = 0f;
    /// Странна фігня, знаю. Множиться на позицію камери, лиши y = 0 якщо не хочеш щоб об'єкт дивився вгору/вниз. Множення на 1,1,1 - це дефолтне значення, дивиться всюди
    /// Якщо лишити x = 1, y = 0, z = 1 - дивиться тільки по горизонталі, не дивиться вгору/вниз або
    /// x = 1, y = 1, z = 0 - дивиться тільки по вертикалі але в твоїй оцій американській системі де вгору це z, а не y
    public Vector3 Multiplier = new Vector3(1f, 1f, 1f);
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
            Vector3 direction = targetPosition - transform.position;
            targetPosition = transform.position + new Vector3(direction.x * Multiplier.x, direction.y * Multiplier.y, direction.z * Multiplier.z);
            direction = targetPosition - transform.position;
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
