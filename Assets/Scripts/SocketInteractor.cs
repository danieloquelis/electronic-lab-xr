using UnityEngine;

[RequireComponent(typeof(Collider))]
public class SocketInteractor : MonoBehaviour
{
    public bool IsOccupied => occupyingInteractable != null;
    private SocketInteractable occupyingInteractable;

    private void OnTriggerEnter(Collider other)
    {
        var interactable = other.GetComponent<SocketInteractable>();
        if (interactable != null && occupyingInteractable == null)
        {
            occupyingInteractable = interactable;
            interactable.NotifyEnteredSocket(this);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        var interactable = other.GetComponent<SocketInteractable>();
        if (interactable != null && occupyingInteractable == interactable)
        {
            // Debounce: Check if still overlapping before considering exited
            Collider socketCollider = GetComponent<Collider>();
            Collider interactorCollider = interactable.GetComponent<Collider>();

            if (!socketCollider.bounds.Intersects(interactorCollider.bounds))
            {
                interactable.NotifyExitedSocket(this);
                occupyingInteractable = null;
            }
        }
    }
}