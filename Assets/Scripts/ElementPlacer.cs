using UnityEngine;

public class ElementPlacer : MonoBehaviour
{
    [SerializeField] private Transform cameraTransform;
    [SerializeField] private float followDistance = 2f;
    [SerializeField] private float heightOffset = 0f;
    [SerializeField] private float followSpeed = 5f;
    [SerializeField] private float rotationSpeed = 5f;
    [SerializeField] private bool shouldFollow = true;

    private bool hasPositionedOnce = false;

    private void Awake()
    {
        // Fallback to main camera if not manually assigned
        if (cameraTransform) return;
        cameraTransform = Camera.main?.transform;
        if (cameraTransform) return;
        
        Debug.LogError("ElementPlacer: No cameraTransform assigned and no Main Camera found.");
        enabled = false;
    }

    private void Update()
    {
        if (shouldFollow)
        {
            FollowCamera();
        }
        else if (!hasPositionedOnce)
        {
            PositionOnce();
            hasPositionedOnce = true;
        }
    }

    private void FollowCamera()
    {
        var targetPosition = cameraTransform.position + cameraTransform.forward * followDistance;
        targetPosition.y += heightOffset;

        transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * followSpeed);

        var direction = transform.position - cameraTransform.position;
        direction.y = 0; // Flatten on Y-axis to avoid tilting

        if (direction == Vector3.zero) return;
        var targetRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);
    }

    private void PositionOnce()
    {
        var targetPosition = cameraTransform.position + cameraTransform.forward * followDistance;
        targetPosition.y += heightOffset;

        transform.position = targetPosition;

        var direction = transform.position - cameraTransform.position;
        direction.y = 0; // Flatten on Y-axis to avoid tilting

        if (direction != Vector3.zero)
        {
            transform.rotation = Quaternion.LookRotation(direction);
        }
    }

    public void AnchorElement()
    {
        shouldFollow = false;
    }
}
