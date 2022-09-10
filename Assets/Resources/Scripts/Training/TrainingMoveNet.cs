using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class TrainingMoveNet : MoveNetSinglePose
{
    private int currentPoseIndex;
    private float switchToNextPose = 0.0f;
    private List<PoseConfigurations> poseConfigurations;

    // Start is called before the first frame update
    new void Start()
    {
        base.Start();
        enableVisualization = true;
        playerInfo = GameObject.Find("PlayerInfo").GetComponent<PlayerInfo>();
        poseConfigurations = new List<PoseConfigurations>();
        StartCoroutine(CorrectToNextPose());

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

    // ready to switch to next pose after a correct pose last over than 3 seconds.
    private IEnumerator CorrectToNextPose()
    {
        for (;;)
        {
            if (switchToNextPose >= 3.0f)
            {
                currentPoseIndex = (currentPoseIndex + 1) % textures.Count;
                figure.GetComponent<RawImage>().texture = textures[currentPoseIndex];
                switchToNextPose = 0.0f;
            }
            yield return new WaitForSeconds(0.1f);
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

        //if (Input.GetKey("up"))
        //{
        //    switchToNextPose += Time.deltaTime;

        //}
        //else
        //{
        //    switchToNextPose = 0f;

        //}

        // add by duration time
        if (matched)
        {
            switchToNextPose += Time.deltaTime;
            playerInfo.score += Time.deltaTime * 10;
        }
        else
        {
            switchToNextPose = 0f;

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
