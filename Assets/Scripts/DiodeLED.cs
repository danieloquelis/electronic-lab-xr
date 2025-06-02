using System;
using UnityEngine;
using UnityEngine.Assertions;

public class DiodeLED : MonoBehaviour, ISimulatableComponent
{   
    
    [SerializeField] private Light lightVisual;
    [SerializeField] private Color color;
    
    [SerializeField] private ConnectionPoint pinA;
    [SerializeField] private ConnectionPoint pinB;

    public Guid PinA => pinA.GetConnectionId();
    public Guid PinB => pinB.GetConnectionId();
    public float Resistance => 10f;
    public float VoltageDrop => 2.0f;
    
    public bool IsCurrentAllowed(float voltageA, float voltageB)
    {
        return (voltageA - voltageB) >= VoltageDrop; 
    }

    public void Simulate(float current)
    {
        lightVisual.gameObject.SetActive(current > 0.01f);
    }

    private void Awake()
    {
        Assert.IsNotNull(lightVisual, "Light needs to be assigned for diode LED component");
    }
}
