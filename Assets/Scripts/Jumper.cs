using UnityEngine;

public class Jumper : MonoBehaviour
{
    [SerializeField] private ConnectionPoint pointA;
    [SerializeField] private ConnectionPoint pointB;
    
    private void Start()
    {
        var circuit = FindFirstObjectByType<CircuitManager>();
        if (circuit)
        {
            circuit.RegisterConnection(pointA, pointB); // manually bridge its own pins
        }
    }
}
