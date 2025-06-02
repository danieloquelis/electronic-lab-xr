using System.Collections.Generic;
using UnityEngine;

public class WireGenerator : MonoBehaviour
{
    public Material wireMaterial;
    public LayerMask collisionMask;

    [Header("Wire Mesh Settings")]
    public int segments = 20;
    public int radialSegments = 6;
    public float radius = 0.003f;
    public float minHeight = 0.02f;
    public float maxHeight = 0.2f;

    private List<GameObject> wires = new List<GameObject>();

    public void AddWire(Transform from, Transform to)
    {
        GameObject wireGO = new GameObject($"Wire_{from.name}_{to.name}");
        wireGO.transform.parent = transform;

        var mf = wireGO.AddComponent<MeshFilter>();
        var mr = wireGO.AddComponent<MeshRenderer>();
        mr.material = wireMaterial;

        var tube = wireGO.AddComponent<Wire>();
        tube.start = from;
        tube.end = to;
        tube.segments = segments;
        tube.radialSegments = radialSegments;
        tube.radius = radius;
        tube.minHeight = minHeight;
        tube.maxHeight = maxHeight;
        tube.collisionMask = collisionMask;

        wires.Add(wireGO);
    }
}