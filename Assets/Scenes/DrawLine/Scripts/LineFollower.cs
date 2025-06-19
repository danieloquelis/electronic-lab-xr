using System;
using Oculus.Interaction;
using UnityEngine;

public class LineFollower : MonoBehaviour
{
    [SerializeField] private Rigidbody robotRb;
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float rotationSpeed = 3f;
    [SerializeField] private float reachThreshold = 0.01f;
    
    private LineRenderer drawnPath;
    private int currentIndex;
    private bool isPathReady = false;
    private bool isPathEnd = false;
    
    void Awake() {
        if (!robotRb) robotRb = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        if (isPathReady && !isPathEnd)
        {
            MoveAlongPath();
        } 
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Follow"))
        {
            other.TryGetComponent<LineRenderer>(out LineRenderer lr);
            drawnPath = lr;
            StartFollowing();
        }
    }
    
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Follow"))
        {
            StopFollowing();
        }
    }
    
    public void StartFollowing()
    {
        if (drawnPath != null)
        {
            currentIndex = 0;
            isPathReady = true;
            isPathEnd = false;
            robotRb.position = drawnPath.GetPosition(0);
        }
    }
    
    public void StopFollowing()
    {
        isPathReady = false;
        isPathEnd = true;
    }

    private void MoveAlongPath()
    {
        Vector3 pathPoint = drawnPath.GetPosition(currentIndex);
        Vector3 offset = Vector3.up * 0.05f;
        Vector3 targetPosition = pathPoint + offset;
        
        // Movement
        robotRb.position = Vector3.MoveTowards(
            robotRb.position, 
            targetPosition, 
            moveSpeed * Time.deltaTime
        );

        // Rotation
        Vector3 direction = (targetPosition - robotRb.position).normalized;
        if (direction != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            robotRb.rotation = Quaternion.Slerp(
                robotRb.rotation, 
                targetRotation, 
                rotationSpeed * Time.deltaTime
            );
        }

        // Update target position
        // go back to beginning
        // if (Vector3.Distance(robotRb.position, targetPosition) < reachThreshold)
        // {
        //     currentIndex = (currentIndex + 1) % drawnPath.positionCount;
        // }
        //
        // stop at the end
        if (Vector3.Distance(robotRb.position, targetPosition) < reachThreshold)
        {
            if (currentIndex < drawnPath.positionCount - 1)
            {
                currentIndex++;
            }
            else
            {
                StopFollowing();
            }
        }
    }
}
