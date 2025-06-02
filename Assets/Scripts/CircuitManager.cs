using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CircuitManager : MonoBehaviour
{
    private readonly Dictionary<Guid, List<Guid>> _edges = new();

    public void RegisterConnection(Guid node, Guid newPoint)
    {
        if (!_edges.ContainsKey(node))
            _edges[node] = new List<Guid>();

        if (!_edges[node].Contains(newPoint))
            _edges[node].Add(newPoint);

        if (!_edges.ContainsKey(newPoint))
            _edges[newPoint] = new List<Guid>();

        if (!_edges[newPoint].Contains(node))
            _edges[newPoint].Add(node);

        Debug.Log($"Registered connection between {node} and {newPoint}");
        Simulate();
    }

    public void DeregisterConnection(Guid node, Guid point)
    {
        if (_edges.TryGetValue(node, out var connections))
        {
            connections.Remove(point);
            Debug.Log($"Deregistered {point} from {node}");
        }

        if (_edges.TryGetValue(point, out var reverseConnections))
        {
            reverseConnections.Remove(node);
            Debug.Log($"Deregistered {node} from {point}");
        }

        Simulate();
    }

    private void Simulate()
    {
        Invoke(nameof(SimulateCircuit), 0.1f);
    }
    
    private void SimulateCircuit()
    {
        Debug.Log("Simulating circuit...");

        // STEP 1: Find all components
        var allComponents = FindObjectsByType<MonoBehaviour>(FindObjectsSortMode.None)
            .OfType<ISimulatableComponent>()
            .ToList();

        // STEP 2: Find the Power Source
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

        // STEP 3: Traverse the circuit graph to find all reachable nodes
        var reachable = new HashSet<Guid>();
        var voltageMap = new Dictionary<Guid, float>();
        var queue = new Queue<(Guid, float)>();

        // Start BFS from power source's +5V and 0V terminals
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

        // STEP 4: Simulate all components
        foreach (var component in allComponents)
        {
            var a = component.PinA;
            var b = component.PinB;

            // Only simulate if both pins are part of reachable graph
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
}
