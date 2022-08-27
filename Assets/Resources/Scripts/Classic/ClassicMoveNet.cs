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
    public string jsonNameArmPrayerStretch;
    public string jsonNameLatissimusDorsiMuscleStretch;
    public string jsonNameUpperTrapStretchRight;
    public string jsonNameNeckIsometricExercise;

    private int currentPoseIndex;
    private List<PoseConfigurations> poseConfigurations;

    // Start is called before the first frame update
    new void Start()
    {
        base.Start();
        enableVisualization = true;
        playerInfo = GameObject.Find("PlayerInfo").GetComponent<PlayerInfo>();
        poseConfigurations = new List<PoseConfigurations>();
        
        // StartCoroutine(RandomPoseFigure());

        JsonConfig jsonConfig = new JsonConfig();
        jsonConfig.ReadJson(poseConfigurations, "arm_prayer_stretch");
        jsonConfig.ReadJson(poseConfigurations, "latissimus_dorsi_muscle_stretch");
        jsonConfig.ReadJson(poseConfigurations, "upper_trap_stretch_right");

        //ReadJson(poseConfigurations, jsonNameArmPrayerStretch);
        //ReadJson(poseConfigurations, jsonNameLatissimusDorsiMuscleStretch);
        //ReadJson(poseConfigurations, jsonNameUpperTrapStretchRight);

        // ReadJson(poseConfigurations, jsonNameNeckIsometricExercise);
    }

    // Update is called once per frame
    void Update()
    {
        Invoke(cameraView.texture);
        // DrawResult(results);
        // DrawResult(results);
        PoseEstimation();
    }

    new void OnDestroy()
    {
        base.OnDestroy();
    }
    private void OnTextureUpdate(Texture texture)
    {
        Invoke(texture);
    }
    private void Invoke(Texture texture)
    {
        moveNet.Invoke(texture);
        results = moveNet.GetResults();
    }

    public void NextPoseFigure()
    {
        currentPoseIndex += 1;
        if (currentPoseIndex == textures.Count)
        {
            currentPoseIndex = 0;
        }
        figure.GetComponent<RawImage>().texture = textures[currentPoseIndex];
    }

    private IEnumerator RandomPoseFigure()
    {
        int index = 0;
        while (true)
        {
            if (index == textures.Count)
            {
                index = 0;
            }

            figure.GetComponent<RawImage>().texture = textures[index];
            currentPoseIndex = index;
            index += 1;
            yield return new WaitForSeconds(20.0f);
        }
    }

    protected override void PoseEstimation()
    {
        bool matched = false;

        // arm_prayer_stretch
        if (currentPoseIndex == 0)
        {
            matched = ArmPrayerStretch();

        }
        // neck_isometric_exercise
        else if (currentPoseIndex == 1)
        {
            matched = LatissimusDorsiMuscleStretch();

        }
        else if (currentPoseIndex == 2)
        {
            matched = UpperTrapStretchRight();

        }

        #region add by times
        // add by times, 1 time = 70
        /*
        if (matched)
        {
            timeLine2 += Time.deltaTime;
            if (timeLine2 - timeLine1 > 1.0f)
            {
                playerInfo.score += 70;
                timeLine1 = timeLine2 = 0.0f;
            }
        }
        else
        {
            timeLine1 = timeLine2 = 0.0f;
        }
        */
        #endregion

        // add by duration time
        if (matched)
        {
            playerInfo.score += Time.deltaTime * 10;
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