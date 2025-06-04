using UnityEngine;

public class SocketInteractable : MonoBehaviour
{
    public MultiSocketInteractable Parent { get; private set; }
    public SocketInteractor CurrentSocket { get; private set; }

    private void Awake()
    {
        Parent = GetComponentInParent<MultiSocketInteractable>();
    }

    public void NotifyEnteredSocket(SocketInteractor socket)
    {
        if (CurrentSocket == null)
        {
            CurrentSocket = socket;
            Parent.RegisterSocket(this);
        }
    }

    public void NotifyExitedSocket(SocketInteractor socket)
    {
        if (CurrentSocket == socket)
        {
            CurrentSocket = null;
            Parent.UnregisterSocket(this);
        }
    }

    public bool IsSocketed => CurrentSocket != null;
}