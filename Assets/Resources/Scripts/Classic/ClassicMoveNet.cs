using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TensorFlowLite;
using System.Reflection;
using System;
using UnityEngine.Scripting;


public class ClassicMoveNet : MoveNetSinglePose
{
    public int currentPoseIndex = -1;
    public List<PoseConfigurations> poseConfigurations;
    public bool matched = false;

    // Start is called before the first frame update
    new void Start()
    {
        base.Start();
        enableVisualization = false;
        playerInfo = GameObject.Find("PlayerInfo").GetComponent<PlayerInfo>();
        poseConfigurations = new List<PoseConfigurations>();
        
        // StartCoroutine(RandomPoseFigure());

        JsonConfig jsonConfig = new JsonConfig();
        jsonConfig.ReadJson(poseConfigurations, "arm_prayer_stretch");
        jsonConfig.ReadJson(poseConfigurations, "latissimus_dorsi_muscle_stretch");
        jsonConfig.ReadJson(poseConfigurations, "upper_trap_stretch_right");

    }

    // Update is called once per frame
    void Update()
    {
        // Invoke(cameraView.texture);
        // DrawResult(results);
        PoseEstimation();
    }

    new void OnDestroy()
    {
        base.OnDestroy();
    }

    protected override void PoseEstimation()
    {
        if (ArmPrayerStretch())
        {
            matched = true;
            currentPoseIndex = 0;
        }
        else if (LatissimusDorsiMuscleStretch())
        {
            matched = true;
            currentPoseIndex = 1;

        }
        else if (UpperTrapStretchRight())
        {
            matched = true;
            currentPoseIndex = 2;

        }
        else
        {
            matched = false;
            currentPoseIndex = -1;
        }

    }
    
    private bool ArmPrayerStretch()
    {
        return base.ParsePoseConfigurations(poseConfigurations[0]);

    }

    private bool LatissimusDorsiMuscleStretch()
    {
        return base.ParsePoseConfigurations(poseConfigurations[1]);

    }

    private bool UpperTrapStretchRight()
    {
        return base.ParsePoseConfigurations(poseConfigurations[2]);

    }

    public void SwitchVisualization()
    {
        enableVisualization = !enableVisualization;
    }

}