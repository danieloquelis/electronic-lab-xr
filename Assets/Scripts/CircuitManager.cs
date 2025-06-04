using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CircuitManager : MonoBehaviour
{
    private readonly Dictionary<Guid, List<Guid>> _edges = new();
    private readonly Dictionary<Guid, ConnectionPoint> _connections = new();
    
    public void RegisterConnection(ConnectionPoint node, ConnectionPoint newPoint)
    {
        var nodeConnId = node.GetConnectionId();
        var newPointConnId = newPoint.GetConnectionId();
        
        if (!_edges.ContainsKey(nodeConnId))
            _edges[nodeConnId] = new List<Guid>();

        if (!_edges[nodeConnId].Contains(newPointConnId))
            _edges[nodeConnId].Add(newPointConnId);

        if (!_edges.ContainsKey(newPointConnId))
            _edges[newPointConnId] = new List<Guid>();

        if (!_edges[newPointConnId].Contains(nodeConnId))
            _edges[newPointConnId].Add(nodeConnId);
        
        _connections[nodeConnId] = node;
        _connections[newPointConnId] = newPoint;
        
        Simulate();
    }

    public void DeregisterConnection(ConnectionPoint node, ConnectionPoint point)
    {
        var nodeConnId = node.GetConnectionId();
        var pointConnId = point.GetConnectionId();
        
        if (_edges.TryGetValue(nodeConnId, out var connections))
        {
            connections.Remove(pointConnId);
        }

        if (_edges.TryGetValue(pointConnId, out var reverseConnections))
        {
            reverseConnections.Remove(nodeConnId);
        }
        
        Simulate();
    }

    private void Simulate()
    {
        Invoke(nameof(SimulateCircuit), 0.1f);
    }
    
    private void SimulateCircuit()
    {
        Debug.Log($"Simulating circuit with {_edges.Count} edges");
        var allComponents = FindObjectsByType<MonoBehaviour>(FindObjectsSortMode.None)
            .OfType<ISimulatableComponent>()
            .ToList();

        var power = allComponents.FirstOrDefault(c => c is PowerSource);

        if (power == null)
        {
            Debug.LogWarning("No power source found. Aborting simulation.");
            foreach (var component in allComponents)
            {
                component.Simulate(0f);
            }
            return;
        }

        var vcc = power.PinA;
        var gnd = power.PinB;

        var reachable = new HashSet<Guid>();
        var voltageMap = new Dictionary<Guid, float>();
        var queue = new Queue<(Guid, float)>();

        queue.Enqueue((vcc, 5f));
        queue.Enqueue((gnd, 0f));
        reachable.Add(vcc);
        reachable.Add(gnd);
        voltageMap[vcc] = 5f;
        voltageMap[gnd] = 0f;

        while (queue.Count > 0)
        {
            var (current, voltage) = queue.Dequeue();
            var neighbors = _edges
                .GetValueOrDefault(current, new List<Guid>())
                .Where(neighbor => !reachable.Contains(neighbor));
            
            foreach (var neighbor in neighbors)
            {
                reachable.Add(neighbor);
                voltageMap[neighbor] = voltage;
                queue.Enqueue((neighbor, voltage));
            }
        }

        foreach (var component in allComponents)
        {
            var a = component.PinA;
            var b = component.PinB;
            
            if (!reachable.Contains(a) || !reachable.Contains(b))
            {
                component.Simulate(0f);
                continue;
            }

            var vA = voltageMap[a];
            var vB = voltageMap[b];
            
            if (component.IsCurrentAllowed(vA, vB))
            {
                var voltageDrop = vA - vB - component.VoltageDrop;
                var current = voltageDrop / (component.Resistance + 0.0001f);
                component.Simulate(current);
            }
            else
            {
                component.Simulate(0f); // Blocked current (e.g., diode reversed)
            }
        }
    }
    
    private void PrintConnectionGraph()
    {
        Debug.Log("=== Connection Graph ===");
        foreach (var (key, toList) in _edges)
        {
            var from = _connections[key];
            var connections = string.Join(", ", toList.Select(g => _connections[g].gameObject.name));
            Debug.Log($"[{from.gameObject.name}] → {connections}");
        }
        Debug.Log("=========================");
    }
}
