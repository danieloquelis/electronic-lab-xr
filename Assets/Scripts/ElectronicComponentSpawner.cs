using System.Collections.Generic;
using System.Linq;
using Meta.XR.MRUtilityKit;
using UnityEngine;
using UnityEngine.Assertions;

public class ElectronicComponentSpawner : MonoBehaviour
{
    [Header("Component Prefabs")]
    public List<GameObject> components;

    [Header("Dependencies")]
    public Transform rightControllerAnchor;

    private void Start()
    {
        Assert.IsNotNull(components, "Components list should not be null.");
        Assert.IsTrue(components.Count > 0, "Components list should contain at least one prefab.");
    }

    public void SpawnComponent(int index)
    {
        if (index < 0 || index >= components.Count)
        {
            Debug.LogWarning("Invalid component index.");
            return;
        }

        if (!MRUK.Instance || !MRUK.Instance.IsInitialized)
        {
            Debug.LogWarning("MRUK is not initialized.");
            return;
        }

        var prefab = components[index];
        TryPlace(prefab);
    }

    private void TryPlace(GameObject prefabToPlace)
    {
        var currentRoom = MRUK.Instance.GetCurrentRoom();
        if (!currentRoom)
        {
            Debug.LogWarning("Current room is not available.");
            return;
        }

        var tableAnchors = currentRoom.Anchors
            .Where(anchor => anchor.Label == MRUKAnchor.SceneLabels.TABLE)
            .ToList();

        if (tableAnchors.Count == 0)
        {
            Debug.LogWarning("No table anchors found in the scene.");
            return;
        }

        var closestTable = tableAnchors
            .OrderBy(anchor => Vector3.Distance(anchor.transform.position, rightControllerAnchor.position))
            .First();

        var tableRenderer = closestTable.GetComponent<Renderer>();
        var tableCenter = tableRenderer ? tableRenderer.bounds.center : closestTable.transform.position;

        var tableUp = closestTable.transform.up;
        var projectedForward = Vector3.ProjectOnPlane(rightControllerAnchor.forward, tableUp).normalized;
        var tableAlignedRotation = Quaternion.LookRotation(projectedForward, tableUp);
        var placementPosition = tableCenter + tableUp * 0.01f;

        var objectToPlace = Instantiate(prefabToPlace, placementPosition, tableAlignedRotation);
        
        if (MRUK.Instance.IsWorldLockActive != true)
        {
            objectToPlace.AddComponent<OVRSpatialAnchor>();
        }
    }
}