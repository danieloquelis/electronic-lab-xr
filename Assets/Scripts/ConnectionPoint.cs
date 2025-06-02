using System;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Events;

public class ConnectionPoint : MonoBehaviour
{
    [SerializeField] private bool isNode;
    [SerializeField, HideInInspector]
    private string connectionIdStr;
    
    public UnityEvent<ConnectionPoint, ConnectionPoint> onConnect;
    public UnityEvent<ConnectionPoint, ConnectionPoint> onDisconnect;
    
    private Guid connectionId;
    public Guid GetConnectionId() => connectionId;

    private void Awake()
    {
        if (string.IsNullOrEmpty(connectionIdStr))
        {
            connectionId = Guid.NewGuid();
            connectionIdStr = connectionId.ToString();
        }
        else
        {
            connectionId = Guid.Parse(connectionIdStr);
        }
        
        if (isNode)
        {
            CheckSetup();
        }
    }

    private void CheckSetup()
    {
        var connectorCollider =  GetComponent<Collider>();
        Assert.IsNotNull(connectorCollider, "Collider not set");
        Assert.IsTrue(connectorCollider.isTrigger, "Collider must contains a triggered collider");

        var circuitManager = FindFirstObjectByType<CircuitManager>();
        Assert.IsNotNull(circuitManager, "Circuit has not been found in the scene");
        onConnect.AddListener(circuitManager.RegisterConnection);
        onDisconnect.AddListener(circuitManager.DeregisterConnection);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!isNode) return;
        
        if (!other.TryGetComponent(out ConnectionPoint point)) return;
        if (point.isNode)
        {
            Debug.LogWarning($"{point.connectionId} is a node point. It can create a raise condition");
        }
        
        onConnect.Invoke(this, point);
    }
    
    private void OnTriggerExit(Collider other)
    {
        if (!isNode) return;

        if (!other.TryGetComponent(out ConnectionPoint point)) return;

        onDisconnect.Invoke(this, point);
    }
}
