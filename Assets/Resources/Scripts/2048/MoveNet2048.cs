using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveNet2048 : MoveNetSinglePose
{
    public int currentPoseIndex = -1;
    public List<PoseConfigurations> poseConfigurations;
    public bool matched = false;

    public bool left = false;
    public bool right = false;
    public bool up = false;
    public bool down = false;

    // Start is called before the first frame update
    new void Start()
    {
        base.Start();
        enableVisualization = false;
        // playerInfo = GameObject.Find("PlayerInfo").GetComponent<PlayerInfo>();
        poseConfigurations = new List<PoseConfigurations>();

        // StartCoroutine(RandomPoseFigure());

        JsonConfig jsonConfig = new JsonConfig();
        jsonConfig.ReadJson(poseConfigurations, "2048 Body/body_slide_up");
        jsonConfig.ReadJson(poseConfigurations, "2048 Body/body_slide_down");
        jsonConfig.ReadJson(poseConfigurations, "2048 Body/body_slide_left_1");
        jsonConfig.ReadJson(poseConfigurations, "2048 Body/body_slide_right_1");
        jsonConfig.ReadJson(poseConfigurations, "2048 Body/body_slide_left_2");
        jsonConfig.ReadJson(poseConfigurations, "2048 Body/body_slide_right_2");

    }

    // left right controller set 1
    private IEnumerator CalculateDirection1()
    {
        if (SlideDown())
        {
            down = true;
        }
        else if (LatissimusDorsiMuscleStretchRight())
        {
            right = true;
        }
        else if (LatissimusDorsiMuscleStretchLeft())
        {
            left = true;
        }
        else if (SlideUp())
        {
            up = true;
        }


        yield return null;
    }

    // left right controller set 2
    private IEnumerator CalculateDirection2()
    {
        if (SlideDown())
        {
            down = true;
        }
        else if (UpperTrapStretchRight())
        {
            right = true;
        }
        else if (UpperTrapStretchLeft())
        {
            left = true;
        }
        else if (SlideUp())
        {
            up = true;
        }


        yield return null;
    }

    // Update is called once per frame
    void Update()
    {
        // Invoke(cameraView.texture);
        // DrawResult(results);
        left = false;
        right = false;
        up = false;
        down = false;

        StartCoroutine(CalculateDirection1());
        StartCoroutine(CalculateDirection2());
        // PoseEstimation();
    }

    new void OnDestroy()
    {
        base.OnDestroy();
    }

    protected override void PoseEstimation()
    {
        // count the times of each pose if wishes.

        //if (ArmPrayerStretch())
        //{
        //    matched = true;
        //    currentPoseIndex = 0;
        //}

    }

    private bool SlideUp()
    {
        return base.ParsePoseConfigurations(poseConfigurations[0]);

    }

    private bool SlideDown()
    {
        return base.ParsePoseConfigurations(poseConfigurations[1]);

    }

    private bool LatissimusDorsiMuscleStretchLeft()
    {
        return base.ParsePoseConfigurations(poseConfigurations[2]);

    }

    private bool LatissimusDorsiMuscleStretchRight()
    {
        return base.ParsePoseConfigurations(poseConfigurations[3]);

    }

    private bool UpperTrapStretchLeft()
    {
        return base.ParsePoseConfigurations(poseConfigurations[4]);

    }

    private bool UpperTrapStretchRight()
    {
        return base.ParsePoseConfigurations(poseConfigurations[5]);

    }



    public void SwitchVisualization()
    {
        enableVisualization = !enableVisualization;
    }
}
