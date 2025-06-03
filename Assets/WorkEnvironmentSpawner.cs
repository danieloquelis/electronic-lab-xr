using Meta.XR;
using Meta.XR.MRUtilityKit;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

public class WorkEnvironmentSpawner : MonoBehaviour
{
    [Header("References to Existing GameObjects")]
    public GameObject menuPanel;
    public GameObject instructionsPanel;

    [Header("Dependencies")]
    public EnvironmentRaycastManager raycastManager;
    public Transform cameraTransform;

    [Header("Placement Offsets")]
    public float panelOffsetHeight = 0.2f;
    public float panelSideOffset = 0.4f;

    private async void Start()
    {
        await WaitForMRUKReady();
        PlaceWorkEnvironment();
    }

    private async Task WaitForMRUKReady()
    {
        while (MRUK.Instance == null || !MRUK.Instance.IsInitialized)
        {
            await Task.Yield(); // Wait for MRUK to initialize
        }
    }

    private void PlaceWorkEnvironment()
    {
        if (raycastManager == null || cameraTransform == null)
        {
            Debug.LogWarning("RaycastManager or CameraTransform is not assigned.");
            return;
        }

        // Perform raycast from head gaze
        Ray ray = new Ray(cameraTransform.position, cameraTransform.forward);
        if (!raycastManager.Raycast(ray, out var hit))
        {
            Debug.LogWarning("Raycast did not hit any surface.");
            return;
        }

        // Retrieve table anchors from the current room
        MRUKRoom currentRoom = MRUK.Instance.GetCurrentRoom();
        if (currentRoom == null)
        {
            Debug.LogWarning("Current room is not available.");
            return;
        }

        List<MRUKAnchor> tableAnchors = currentRoom.Anchors
            .Where(anchor => anchor.Label == MRUKAnchor.SceneLabels.TABLE)
            .ToList();

        if (tableAnchors.Count == 0)
        {
            Debug.LogWarning("No table anchors found in the scene.");
            return;
        }

        // Find the closest table anchor to the hit point
        MRUKAnchor closestTable = tableAnchors
            .OrderBy(anchor => Vector3.Distance(anchor.transform.position, hit.point))
            .First();

        Vector3 tableCenter = closestTable.GetComponent<Renderer>()?.bounds.center ?? closestTable.transform.position;
        Vector3 tableNormal = closestTable.transform.up;

        // Calculate right and left offsets based on the table's orientation
        Vector3 rightOffset = Vector3.Cross(tableNormal, Vector3.up).normalized * panelSideOffset;

        // Position the menu panel to the right
        if (menuPanel != null)
        {
            Vector3 menuPosition = tableCenter + rightOffset + tableNormal * panelOffsetHeight;
            menuPanel.transform.position = menuPosition;
            OrientTowardsCamera(menuPanel.transform);
        }

        // Position the instructions panel to the left
        if (instructionsPanel != null)
        {
            Vector3 instructionsPosition = tableCenter - rightOffset + tableNormal * panelOffsetHeight;
            instructionsPanel.transform.position = instructionsPosition;
            OrientTowardsCamera(instructionsPanel.transform);
        }
    }

    private void OrientTowardsCamera(Transform panelTransform)
    {
        Vector3 directionToCamera = cameraTransform.position - panelTransform.position;
        directionToCamera.y = 0; // Keep the panel upright
        if (directionToCamera.sqrMagnitude > 0.001f)
        {
            panelTransform.rotation = Quaternion.LookRotation(directionToCamera.normalized, Vector3.up);
        }
    }
}
