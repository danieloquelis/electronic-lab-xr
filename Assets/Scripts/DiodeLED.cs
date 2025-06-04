using System;
using UnityEngine;
using UnityEngine.Assertions;

public class DiodeLED : MonoBehaviour, ISimulatableComponent
{   
    
    [SerializeField] private Material ledOnMaterial;
    [SerializeField] private Material ledOffMaterial;
    [SerializeField] private MeshRenderer ledMeshRenderer;
    
    [SerializeField] private ConnectionPoint vcc;
    [SerializeField] private ConnectionPoint gnd;

    public Guid PinA => vcc.GetConnectionId();
    public Guid PinB => gnd.GetConnectionId();
    public float Resistance => 10f;
    public float VoltageDrop => 2.0f;

    private void Start()
    {
        ledMeshRenderer.material = ledOffMaterial;
    }

    public bool IsCurrentAllowed(float voltageA, float voltageB)
    {
        return voltageA - voltageB >= VoltageDrop; 
    }

    public void Simulate(float current)
    {
        ledMeshRenderer.material = current > 0.01f ? ledOnMaterial : ledOffMaterial;
    }
}
