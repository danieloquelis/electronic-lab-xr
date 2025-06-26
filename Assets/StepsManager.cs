using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Video;

public class StepsManager : MonoBehaviour
{
    [SerializeField] private VideoPlayer instructionsVideoPlayer;
    [SerializeField] private List<VideoClip> videoInstructions;
    [SerializeField] private TMP_Text instructionsTitle;
    [SerializeField] private TMP_Text instructionsDescription;
    [SerializeField] private AudioSource stepCompletedAudio;
    [SerializeField] private ParticleSystem stepCompletedSfx;
    
    private int _currentStep;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
        Assert.IsNotNull(instructionsVideoPlayer, "Instructions video player is null");
        PlayInstructionVideo(0);
        _currentStep = 0;
    }

    public void PlayInstructionVideo(int i)
    {
        instructionsVideoPlayer.clip = videoInstructions[i];
        var title = i switch
        {
            0 => "Pick Acrilic",
            1 => "Bring Motors",
            2 => "Take Wheels",
            3 => "Add Caster Wheel",
            4 => "Take IR Sensors",
            5 => "Motor Driver",
            6 => "Add Breadboard",
            _ => "Ops! something went wrong"
        };
        
        var description = i switch
        {
            0 => "Find the acrilic on the left of the components",
            1 => "Use DC motors for the movements of the robot",
            2 => "Now pick the yellow wheels and insert into motors",
            3 => "Add the caster wheel at the back of the robot",
            4 => "Add 3 IR sensor in the front of the robot",
            5 => "Take the motor driver and place it in the middle of the bot",
            6 => "Place the bread board in the space left",
            _ => "Ops! something went wrong"
        };
        
        instructionsTitle.text = title;
        instructionsDescription.text = description;
        _currentStep = i;
    }

    private void PrivateNextStep()
    {
        stepCompletedSfx.Play();
        stepCompletedAudio.Play();
        PlayInstructionVideo(_currentStep + 1);
    }

    public void NextStep()
    {
        Invoke(nameof(PrivateNextStep), 1);
    }
}
