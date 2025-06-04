using System.Collections.Generic;
using UnityEngine;
using Oculus.Interaction;

[RequireComponent(typeof(Rigidbody), typeof(Grabbable), typeof(InteractableUnityEventWrapper))]
public class MultiSocketInteractable : MonoBehaviour
{
    private List<SocketInteractable> interactables;
    private HashSet<SocketInteractable> socketed = new();

    private Grabbable grabbable;
    private Rigidbody rb;
    private Transform originalParent;
    private InteractableUnityEventWrapper eventWrapper;

    private bool recentlyGrabbed = false;

    private void Awake()
    {
        grabbable = GetComponent<Grabbable>();
        rb = GetComponent<Rigidbody>();
        eventWrapper = GetComponent<InteractableUnityEventWrapper>();
        originalParent = transform.parent;

        interactables = new List<SocketInteractable>(GetComponentsInChildren<SocketInteractable>());
    }

    private void OnEnable()
    {
        eventWrapper.WhenSelect.AddListener(OnGrabbed);
        eventWrapper.WhenUnselect.AddListener(OnReleased);
    }

    private void OnDisable()
    {
        eventWrapper.WhenSelect.RemoveListener(OnGrabbed);
        eventWrapper.WhenUnselect.RemoveListener(OnReleased);
    }

    public void RegisterSocket(SocketInteractable interactable)
    {
        socketed.Add(interactable);
        TryAttach();
    }

    public void UnregisterSocket(SocketInteractable interactable)
    {
        socketed.Remove(interactable);
        CheckDetach();
    }

    private void TryAttach()
    {
        if (recentlyGrabbed) return;

        if (socketed.Count == interactables.Count)
        {
            transform.SetParent(interactables[0].CurrentSocket.transform.root, true);
            rb.isKinematic = true;
        }
    }

    private void CheckDetach()
    {
        if (socketed.Count < interactables.Count)
        {
            Detach();
        }
    }

    private void Detach()
    {
        transform.SetParent(originalParent, true);
        socketed.Clear();
    }

    private void OnGrabbed()
    {
        recentlyGrabbed = true;
        Detach();
        Invoke(nameof(ResetGrabCooldown), 0.2f);
    }

    private void OnReleased()
    {
        TryAttach();
    }

    private void ResetGrabCooldown()
    {
        recentlyGrabbed = false;
    }
}
