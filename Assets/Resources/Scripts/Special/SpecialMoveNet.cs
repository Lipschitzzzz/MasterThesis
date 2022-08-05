using System.Collections;
using System.Collections.Generic;
using TensorFlowLite;
using UnityEngine;

public class SpecialMoveNet : MoveNetSinglePose
{

    // Start is called before the first frame update
    new void Start()
    {
        base.Start();
        //playerInfo = GameObject.Find("Player Info").GetComponent<PlayerInfo>();
    }

    // Update is called once per frame
    void Update()
    {
        results = moveNet.GetResults();

        //PoseEstimation();
        
    }
    //public void PoseEstimation()
    //{
    //    if (results[5].confidence > 0.3f && results[11].confidence > 0.3f && results[13].confidence > 0.3f && results[7].confidence > 0.3f)
    //    {
    //        float difference5_11 = Mathf.Abs(results[5].y - results[11].y);
    //        float difference11_13 = Mathf.Abs(results[11].y - results[13].y);
    //        float difference13_15 = Mathf.Abs(results[13].y - results[15].y);
    //        if (Mathf.Abs(difference5_11 - difference11_13) < 0.15f && Mathf.Abs(difference11_13 - difference13_15) < 0.15f && Mathf.Abs(results[7].y - results[5].y) > 0.2f)
    //        {
    //            timeLine2 += Time.deltaTime;
    //            if (timeLine2 - timeLine1 > 0.5f)
    //            {
    //                playerInfo.score += 5;
    //                timeLine1 = timeLine2 = 0.0f;
    //            }
    //            draw.color = Color.green;

    //            MoveNet.Result[] line1 = new MoveNet.Result[2];
    //            line1[0] = results[5];
    //            line1[1] = results[11];

    //            MoveNet.Result[] line2 = new MoveNet.Result[2];
    //            line2[0] = results[11];
    //            line2[1] = results[13];

    //            MoveNet.Result[] line3 = new MoveNet.Result[2];
    //            line3[0] = results[7];
    //            line3[1] = results[5];
    //            DrawLine(line1);
    //            DrawLine(line2);
    //            DrawLine(line3);
    //        }
    //    }
    //    else
    //    {
    //        timeLine1 = timeLine2 = 0.0f;
    //    }
    //}


}
