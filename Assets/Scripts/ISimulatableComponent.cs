using System;

public interface ISimulatableComponent
{
    Guid PinA { get; }
    Guid PinB { get; }
    float Resistance { get; }
    float VoltageDrop { get; }

    // For direction-dependent elements like diodes or LEDs
    bool IsCurrentAllowed(float voltageA, float voltageB); 

    // For stateful components like switches or LEDs
    void Simulate(float current); 
}
