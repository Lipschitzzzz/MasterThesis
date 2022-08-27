using System.Collections;
using System.Collections.Generic;
using TensorFlowLite;
using UnityEngine;

public class ChallengeMoveNet : MoveNetSinglePose
{
    public string jsonNamePushUp;
    private int currentPoseIndex;
    private List<PoseConfigurations> poseConfigurations;

    // 0 -> push-up-0
    // 1 -> push-up-1
    // -1 -> neither
    private int status;


    // Start is called before the first frame update
    new void Start()
    {
        base.Start();
        enableVisualization = true;
        // playerInfo = GameObject.Find("PlayerInfo").GetComponent<PlayerInfo>();
        poseConfigurations = new List<PoseConfigurations>();
        status = -1;

        // StartCoroutine(RandomPoseFigure());

        JsonConfig jsonConfig = new JsonConfig();
        jsonConfig.ReadJson(poseConfigurations, "push_up-0");
        jsonConfig.ReadJson(poseConfigurations, "push_up-1");
    }

    // Update is called once per frame
    void Update()
    {
        results = moveNet.GetResults();
        // DrawResult(results);
        PoseEstimation();
        
    }

    private bool PushUp0()
    {
        return base.ParsePoseConfigurations(poseConfigurations[0]);

    }

    private bool PushUp1()
    {
        return base.ParsePoseConfigurations(poseConfigurations[1]);

    }

    private IEnumerator PushUpReady()
    {
        while (true)
        {
            if (PushUp0())
            {
                status = 0;
            }
        }
    }

    private IEnumerator PushUpInProgress()
    {
        while (true)
        {
            if (PushUp1())
            {
                status = 1;
            }
        }
    }

    private IEnumerator PushUpDone()
    {
        status = -1;
        yield return null;
    }

    protected override void PoseEstimation()
    {
        // PushUp Start
        if (status == -1)
        {
            StopAllCoroutines();
            StartCoroutine(PushUpReady());
        }
        // PushUp0 Ready
        else if (status == 0)
        {
            StopAllCoroutines();
            StartCoroutine(PushUpInProgress());
        }
        else if (status == 1)
        {
            StopAllCoroutines();
            StartCoroutine(PushUpDone());

            // one full push-up has been done
            // do some bonus here




        }


    }



}
