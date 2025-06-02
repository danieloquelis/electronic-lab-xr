using System;
using UnityEngine;

public class PowerSource : MonoBehaviour, ISimulatableComponent
{
    [SerializeField] private ConnectionPoint vccPin;
    [SerializeField] private ConnectionPoint gndPin;
    public Guid PinA => vccPin.GetConnectionId(); // +5V
    public Guid PinB => gndPin.GetConnectionId(); // GND
    public float Resistance => 0f;
    public float VoltageDrop => 0f;
    public bool IsCurrentAllowed(float vA, float vB) => true;
    public void Simulate(float current) { }
}
