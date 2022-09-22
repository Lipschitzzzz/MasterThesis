using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using TensorFlowLite;
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
        jsonConfig.ReadJson(poseConfigurations, "Training/arm_prayer_stretch");
        jsonConfig.ReadJson(poseConfigurations, "Training/latissimus_dorsi_muscle_stretch_right");
        jsonConfig.ReadJson(poseConfigurations, "Training/latissimus_dorsi_muscle_stretch_left");
        jsonConfig.ReadJson(poseConfigurations, "Training/upper_trap_stretch_left");
        jsonConfig.ReadJson(poseConfigurations, "Training/upper_trap_stretch_right");

        jsonConfig.ReadJson(poseConfigurations, "2048 Body/body_slide_up");
        jsonConfig.ReadJson(poseConfigurations, "2048 Body/body_slide_down");
        jsonConfig.ReadJson(poseConfigurations, "2048 Body/body_slide_left_1");
        jsonConfig.ReadJson(poseConfigurations, "2048 Body/body_slide_right_1");
        jsonConfig.ReadJson(poseConfigurations, "2048 Body/body_slide_left_2");
        jsonConfig.ReadJson(poseConfigurations, "2048 Body/body_slide_right_2");

    }

    // Update is called once per frame
    void Update()
    {
        // Invoke(cameraView.texture);
        // DrawResult(results);

        #region test head control
        //MoveNet.Result[] angle = new MoveNet.Result[3];

        //angle[0] = results[0];
        //angle[1] = results[2];
        //angle[2] = results[4];
        //float[] p0 = { results[0].x, results[0].y };
        //float[] p1 = { results[2].x, results[2].y };
        //float[] p2 = { results[4].x, results[4].y };
        //float angl2e = utilities.CalculateAngle3Points(p0, p1, p2);
        //Debug.Log(angl2e);
        //DrawAngle(angle);

        //float d = 0.0f;
        //float re = 0.0f;
        //if (results[2].confidence > 0.4f && results[1].confidence > 0.4f &&
        //    results[5].confidence > 0.4f && results[6].confidence > 0.4f)
        //{
        //    re = Mathf.Abs(results[5].y - results[6].y);
        //    d = Mathf.Abs((results[2].y - results[1].y) / re);

        //    if (1.5f < d && d < 2.0f)
        //    {
        //        if (results[2].y > results[1].y)
        //        {
        //            Debug.Log("RRRRRRRRRRRRRR");
        //        }
        //        else if (results[2].y < results[1].y)
        //        {
        //            Debug.Log("LLLLLLLLLLLLLL");
        //        }
        //        else
        //        {
        //            Debug.LogError("321321312312321");
        //        }
        //    }
        //}
        #endregion


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
        else if (currentPoseIndex == 1)
        {
            matched = LatissimusDorsiMuscleStretchRight();

        }
        else if (currentPoseIndex == 2)
        {
            matched = LatissimusDorsiMuscleStretchLeft();

        }
        else if (currentPoseIndex == 3)
        {
            matched = UpperTrapStretchLeft();

        }
        else if (currentPoseIndex == 4)
        {
            matched = UpperTrapStretchRight();

        }
        else if (currentPoseIndex == 5)
        {
            matched = SlideUp();

        }
        else if (currentPoseIndex == 6)
        {
            matched = SlideDown();

        }
        else if (currentPoseIndex == 7)
        {
            matched = SlideLeft1();

        }
        else if (currentPoseIndex == 8)
        {
            matched = SlideRight1();

        }
        else if (currentPoseIndex == 9)
        {
            matched = SlideLeft2();

        }
        else if (currentPoseIndex == 10)
        {
            matched = SlideRight2();

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

    private bool LatissimusDorsiMuscleStretchRight()
    {
        return base.ParsePoseConfigurations(poseConfigurations[1]);

    }

    private bool LatissimusDorsiMuscleStretchLeft()
    {
        return base.ParsePoseConfigurations(poseConfigurations[2]);

    }

    private bool UpperTrapStretchLeft()
    {
        return base.ParsePoseConfigurations(poseConfigurations[3]);

    }

    private bool UpperTrapStretchRight()
    {
        return base.ParsePoseConfigurations(poseConfigurations[4]);

    }

    private bool SlideUp()
    {
        return base.ParsePoseConfigurations(poseConfigurations[5]);

    }

    private bool SlideDown()
    {
        return base.ParsePoseConfigurations(poseConfigurations[6]);

    }

    private bool SlideLeft1()
    {
        return base.ParsePoseConfigurations(poseConfigurations[7]);

    }

    private bool SlideRight1()
    {
        return base.ParsePoseConfigurations(poseConfigurations[8]);

    }

    private bool SlideLeft2()
    {
        return base.ParsePoseConfigurations(poseConfigurations[9]);

    }

    private bool SlideRight2()
    {
        return base.ParsePoseConfigurations(poseConfigurations[10]);

    }

    public void SwitchVisualization()
    {
        enableVisualization = !enableVisualization;
    }
}
