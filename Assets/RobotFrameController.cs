using System;
using UnityEngine;
using UnityEngine.Assertions;

public class RobotFrameController : MonoBehaviour
{
    private StepsManager _stepsManager;

    private int _motorsAttached;
    private int _wheelsAttached;
    private int _irSensorsAttached;
    
    private void Start()
    {
        _stepsManager = FindFirstObjectByType<StepsManager>();
        Assert.IsNotNull(_stepsManager, "No steps manager found in the scene.");
    }

    public void OnNextStep()
    {
        _stepsManager.NextStep();
    }

    public void OnMotorAttached()
    {
        if (_motorsAttached < 2)
        {
            _motorsAttached++;
            return;
        }
        
        OnNextStep();
    }

    public void OnWheelAttached()
    {
        if (_wheelsAttached < 2)
        {
            _wheelsAttached++;
            return;
        }
        
        OnNextStep();
    }

    public void OnIrSensorAttached()
    {
        if (_irSensorsAttached < 3)
        {
            _irSensorsAttached++;
            return;
        }
        
        OnNextStep();
    }
}
