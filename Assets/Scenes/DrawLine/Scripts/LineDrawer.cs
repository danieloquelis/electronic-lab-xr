using Oculus.Interaction;
using Oculus.Interaction.HandGrab;
using UnityEngine;

public class LineDrawer : MonoBehaviour
{
    [Header("Pen Properties")] 
    [SerializeField] private Transform drawingPoint;
    [SerializeField] private Material drawingMaterial;
    [Range(0.01f, 0.1f)] [SerializeField] private float lineWidth = 0.01f;

    [Header("Hands & Grab Interactors")]
    [SerializeField] private HandGrabInteractor rightHandGrabInteractor;
    [SerializeField] private HandGrabInteractor leftHandGrabInteractor;
    [SerializeField] private HandGrabInteractor rightControllerGrabInteractor;
    [SerializeField] private HandGrabInteractor leftControllerGrabInteractor;
    
    private LineRenderer _currentDrawing;
    private int _index;
    private Mesh _mesh;
    private MeshFilter _meshFilter;
    private MeshRenderer _meshRenderer;
    private MeshCollider _meshCollider;
    
    private bool _isDrawing = false;
    private bool _isPenActivated = false;

    private void Update()
    {
        bool isGrabbed = InteractorState.Select == rightControllerGrabInteractor.State || InteractorState.Select == leftControllerGrabInteractor.State || InteractorState.Select == rightHandGrabInteractor.State || InteractorState.Select == leftHandGrabInteractor.State;
        
        if (isGrabbed && _isPenActivated && !_isDrawing)
        {
            _isDrawing = true;
            StartDrawing();
        }
        else if ((!isGrabbed || !_isPenActivated) && _isDrawing)
        {
            _isDrawing = false;
            EndDrawing();
        }

        if (_isDrawing && _isPenActivated)
        {
            ContinueDrawing();
        }
    }

    public void TogglePen()
    {
        _isPenActivated = !_isPenActivated;
    }
    
    private void StartDrawing()
    {
        if (_currentDrawing == null)
        {
            _index = 0;
            _currentDrawing = new GameObject("DrawnLine").AddComponent<LineRenderer>();
            _currentDrawing.tag = "Follow";
            _currentDrawing.material = drawingMaterial;
            _currentDrawing.startWidth = _currentDrawing.endWidth = lineWidth;
            _currentDrawing.positionCount = 1;
            _currentDrawing.SetPosition(0, drawingPoint.position);
        }
    }

    private void ContinueDrawing()
    {
        if (_currentDrawing != null)
        {
            var currentPos = _currentDrawing.GetPosition(_index);
            if (Vector3.Distance(currentPos, drawingPoint.position) > lineWidth)
            {
                _index++;
                _currentDrawing.positionCount = _index + 1;
                _currentDrawing.SetPosition(_index, drawingPoint.position);
            }
        }
    }

    private void EndDrawing()
    {
        if (_currentDrawing != null)
        {
            AddColliderToLine(_currentDrawing);
            _currentDrawing = null;
        }
    }
    
    public void AddColliderToLine(LineRenderer lineRenderer)
    {
        MeshCollider meshCollider = lineRenderer.GetComponent<MeshCollider>();
        if (!meshCollider)
            meshCollider = lineRenderer.gameObject.AddComponent<MeshCollider>();

        Mesh mesh = new Mesh();
        lineRenderer.BakeMesh(mesh, true);
        meshCollider.sharedMesh = mesh;
    }
}
