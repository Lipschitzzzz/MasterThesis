using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TensorFlowLite;
using System.Reflection;
using System;

public class ClassicMoveNet : MoveNetSinglePose
{
    public string jsonNameArmPrayerStretch;
    public string jsonNameLatissimusDorsiMuscleStretch;
    public string jsonNameUpperTrapStretchRight;
    public string jsonNameNeckIsometricExercise;

    private string currentPoseName;
    private int currentPoseIndex;
    // private float timeLine1 = 0.0f;
    // private float timeLine2 = 0.0f;
    private float time;
    private List<PoseConfigurations> poseConfigurations;
    PoseConfigurations currentPoseConfiguration;

    private Type t;
    private MethodInfo methodName;

    // Start is called before the first frame update
    new void Start()
    {
        base.Start();
        playerInfo = GameObject.Find("PlayerInfo").GetComponent<PlayerInfo>();
        poseConfigurations = new List<PoseConfigurations>();
        currentPoseConfiguration = new PoseConfigurations();

        t = typeof(ClassicMoveNet);
        MethodInfo methodName = t.GetMethod("ArmPrayerStretch", BindingFlags.Public);
        if (methodName == null)
        {
            Debug.Log("didn't find this function!");
        }
        else
        {
            bool test = (bool)methodName.Invoke(null, null);
            Debug.Log("test: " + test);
        }

        StartCoroutine(RandomPoseFigure());
        ReadJson(poseConfigurations, jsonNameArmPrayerStretch);
        ReadJson(poseConfigurations, jsonNameLatissimusDorsiMuscleStretch);
        ReadJson(poseConfigurations, jsonNameUpperTrapStretchRight);
        ReadJson(poseConfigurations, jsonNameNeckIsometricExercise);
    }

    // Update is called once per frame
    void Update()
    {
        results = moveNet.GetResults();
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
            yield return new WaitForSeconds(60.0f);
        }
    }

    protected override void PoseEstimation()
    {
        bool matched = false;
        currentPoseConfiguration = poseConfigurations[currentPoseIndex];

        // arm_prayer_stretch
        if (currentPoseIndex == 0)
        {
            matched = ArmPrayerStretch();

        }
        // neck_isometric_exercise
        else if (currentPoseIndex == 1)
        {
            matched = ArmPrayerStretch();

        }

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

        // add by duration time
        if (matched)
        {
            playerInfo.score += Time.deltaTime * 10;
        }

    }

    private bool ArmPrayerStretch()
    {
        if (results[9].confidence > 0.3f && results[10].confidence > 0.3f)
        {
            foreach (List<int> i in poseConfigurations[0].verticalRelation.Keys)
            {
                float value = float.Parse(poseConfigurations[0].verticalRelation[i]);
                if(value == 0)
                {
                    // i[0].y should be < i[1].y but we only need the wrong case to visualization.
                    if (results[i[0]].y > results[i[1]].y)
                    {
                        draw.color = Color.red;
                        DrawResult(results);
                        return false;
                    }
                }
                else if(value == 1)
                {
                    // i[0].y should be > i[1].y but we only need the wrong case to visualization.
                    if (results[i[0]].y < results[i[1]].y)
                    {
                        draw.color = Color.red;
                        DrawResult(results);
                        return false;
                    }
                }
                else
                {
                    Debug.Log("verticalRelation value error");
                }
            }
            //if (results[9].y < results[5].y || results[9].y < results[6].y || results[10].y < results[5].y || results[10].y < results[6].y)
            //{
            //    draw.color = Color.red;
            //    DrawResult(results);
            //    return false;
            //}
        }
        List<bool> matched = new List<bool>();

        // xRelativeDistance
        foreach (List<int> i in poseConfigurations[0].xRelativeDistance.Keys)
        {
            // line0 point0 - point1
            int pointIndex0 = i[0];
            int pointIndex1 = i[1];

            // line1 point2 - point3
            int pointIndex2 = i[2];
            int pointIndex3 = i[3];

            // proportion = distance(line0 / line1)
            float proportion = float.Parse(poseConfigurations[0].xRelativeDistance[i]);

            // Debug.Log(pointIndex0.ToString() + ' ' + pointIndex1.ToString() + ' ' + 
            //     pointIndex2.ToString() + ' ' + pointIndex3.ToString() + ' ' + proportion.ToString());

            if (results[pointIndex0].confidence > 0.3f && results[pointIndex1].confidence > 0.3f && results[pointIndex2].confidence > 0.3f && results[pointIndex3].confidence > 0.3f)
            {
                float line0 = Mathf.Abs(results[pointIndex0].x - results[pointIndex1].x);
                float line1 = Mathf.Abs(results[pointIndex2].x - results[pointIndex3].x);

                // float relativeDistance = line0 / line1;
                // distance 1 - distance 2
                float tolerance = line1 * proportion;

                // float x_normalization = Mathf.Abs(relativeDistance - proportion) / (proportion - 0);

                Color color = new Color(1.0f, 0.0f, 0.0f);

                if (line0 < tolerance)
                {
                    color = new Color(0.0f, 1.0f, 0.0f);
                    matched.Add(true);
                }
                else
                {
                    matched.Add(false);
                }
                // color = new Color(x_normalization, 1 - x_normalization, 0);

                draw.color = color;
                MoveNet.Result[] line = new MoveNet.Result[2];
                line[0] = results[pointIndex0];
                line[1] = results[pointIndex1];
                DrawLine(line);
                
            }
            else
            {
                matched.Add(false);
            }
        }

        // yRelativeDistance
        foreach (List<int> i in poseConfigurations[0].yRelativeDistance.Keys)
        {
            // line0 point0 - point1
            int pointIndex0 = i[0];
            int pointIndex1 = i[1];

            // line1 point2 - point3
            int pointIndex2 = i[2];
            int pointIndex3 = i[3];

            // proportion = distance(line0 / line1)
            float proportion = float.Parse(poseConfigurations[0].yRelativeDistance[i]);

            // Debug.Log(pointIndex0.ToString() + ' ' + pointIndex1.ToString() + ' ' +
            //     pointIndex2.ToString() + ' ' + pointIndex3.ToString() + ' ' + proportion.ToString());

            if (results[pointIndex0].confidence > 0.3f && results[pointIndex1].confidence > 0.3f && results[pointIndex2].confidence > 0.3f && results[pointIndex3].confidence > 0.3f)
            {
                float line0 = Mathf.Abs(results[pointIndex0].y - results[pointIndex1].y);
                float line1 = Mathf.Abs(results[pointIndex2].y - results[pointIndex3].y);

                float tolerance = line1 * proportion;
                Color color = new Color(1.0f, 0.0f, 0.0f);
                if (line0 < tolerance)
                {
                    color = new Color(0.0f, 1.0f, 0.0f);
                    matched.Add(true);
                }
                else
                {
                    matched.Add(false);
                }
                // float relativeDistance = line0 / line1;
                // distance 1 - distance 2 
                // float y_normalization = Mathf.Abs(relativeDistance - proportion) / (proportion - 0);

                // color = new Color(y_normalization, 1 - y_normalization, 0);

                draw.color = color;
                MoveNet.Result[] line = new MoveNet.Result[2];
                line[0] = results[pointIndex0];
                line[1] = results[pointIndex1];

                
            }
            else
            {
                matched.Add(false);
            }
        }

        // xCoordinateTolerance
        #region xCoordinateTolerance
        foreach (List<int> i in poseConfigurations[0].xCoordinateTolerance.Keys)
        {
            int pointIndex0 = i[0];
            int pointIndex1 = i[1];
            float value = float.Parse(poseConfigurations[0].xCoordinateTolerance[i]);
            Debug.Log(pointIndex0.ToString() + ' ' + pointIndex1.ToString() + ' ' + value.ToString());
            if (results[pointIndex0].confidence > 0.3f && results[pointIndex1].confidence > 0.3f)
            {
                float x0 = results[pointIndex0].x;
                float x1 = results[pointIndex1].x;

                float x_normalization = Mathf.Abs(Mathf.Abs(x0 - x1) - value) / (value - 0);

                Color color = new Color(1.0f, 0.0f, 0.0f);
                color = new Color(x_normalization, 1 - x_normalization, 0);

                draw.color = color;
                MoveNet.Result[] line = new MoveNet.Result[2];
                line[0] = results[pointIndex0];
                line[1] = results[pointIndex1];
                DrawLine(line);

                if (x_normalization <= 0.5)
                {
                    matched.Add(true);
                }
                else
                {
                    matched.Add(false);
                }
            }
            else
            {
                matched.Add(false);
            }
        }
        #endregion

        // yCoordinateTolerance
        #region yCoordinateTolerance
        foreach (List<int> i in poseConfigurations[0].yCoordinateTolerance.Keys)
        {
            int pointIndex0 = i[0];
            int pointIndex1 = i[1];
            float value = float.Parse(poseConfigurations[0].yCoordinateTolerance[i]);
            Debug.Log(pointIndex0.ToString() + ' ' + pointIndex1.ToString() + ' ' + value.ToString());
            if (results[pointIndex0].confidence > 0.3f && results[pointIndex1].confidence > 0.3f)
            {
                float y0 = results[pointIndex0].y;
                float y1 = results[pointIndex1].y;

                float y_normalization = Mathf.Abs(Mathf.Abs(y0 - y1) - value) / (value - 0);

                Color color = new Color(1.0f, 0.0f, 0.0f);
                color = new Color(y_normalization, 1 - y_normalization, 0);

                draw.color = color;
                MoveNet.Result[] line = new MoveNet.Result[2];
                line[0] = results[pointIndex0];
                line[1] = results[pointIndex1];
                // DrawLine(line);

                if (y_normalization <= 0.5)
                {
                    matched.Add(true);
                }
                else
                {
                    matched.Add(false);
                }
            }
            else
            {
                matched.Add(false);
            }
        }
        #endregion


        // angle
        foreach (List<int> i in poseConfigurations[0].angles.Keys)
        {
            // point0 point1 point2
            int pointIndex0 = i[0];
            int pointIndex1 = i[1];
            int pointIndex2 = i[2];

            // angle between point0 point1 point2
            float value = float.Parse(poseConfigurations[0].angles[i]);
            // Debug.Log(pointIndex0.ToString() + ' ' + pointIndex1.ToString() + ' ' + pointIndex2.ToString() + ' ' + value.ToString());
            if (results[pointIndex0].confidence > 0.3f && results[pointIndex1].confidence > 0.3f && results[pointIndex2].confidence > 0.3f)
            {
                MoveNet.Result[] angle = new MoveNet.Result[3];

                angle[0] = results[pointIndex0];
                angle[1] = results[pointIndex1];
                angle[2] = results[pointIndex2];

                float[] p0 = { results[pointIndex0].x, results[pointIndex0].y };
                float[] p1 = { results[pointIndex1].x, results[pointIndex1].y };
                float[] p2 = { results[pointIndex2].x, results[pointIndex2].y };

                float angle_normalization = Mathf.Abs(utilities.CalculateAngle3Points(p0, p1, p2) - value) / (value - 0);

                Color color = new Color(1.0f, 0.0f, 0.0f);
                color = new Color(angle_normalization, 1 - angle_normalization, 0);

                draw.color = color;

                DrawAngle(angle);

                if (angle_normalization <= 0.5)
                {
                    matched.Add(true);
                }
                else
                {
                    matched.Add(false);
                }
            }
            else
            {
                matched.Add(false);
            }
        }
        
        // ensure all the actions matched
        
        if(matched.Count == 0)
        {
            return false;
        }

        foreach (bool i in matched)
        {
            if (!i)
            {
                return false;
            }
        }

        return true;
    }

    private bool LatissimusDorsiMuscleStretch()
    {
        if (results[10].confidence > 0.3f && results[6].confidence > 0.3f)
        {
            foreach (List<int> i in poseConfigurations[0].horizontalRelation.Keys)
            {
                float value = float.Parse(poseConfigurations[0].horizontalRelation[i]);
                if (value == 0)
                {
                    // i[0].x should be < i[1].x but we only need the wrong case to visualization.
                    if (results[i[0]].x > results[i[1]].x)
                    {
                        draw.color = Color.red;
                        DrawResult(results);
                        return false;
                    }
                }
                else if (value == 1)
                {
                    // i[0].x should be > i[1].x but we only need the wrong case to visualization.
                    if (results[i[0]].x < results[i[1]].x)
                    {
                        draw.color = Color.red;
                        DrawResult(results);
                        return false;
                    }
                }
                else
                {
                    Debug.Log("horizontalRelation value error");
                }
            }
            //if (results[10].x < results[6].x)
            //{
            //    draw.color = Color.red;
            //    DrawResult(results);
            //    return false;
            //}
        }
        List<bool> matched = new List<bool>();

        // xCoordinateTolerance
        foreach (List<int> i in poseConfigurations[1].xCoordinateTolerance.Keys)
        {
            int pointIndex0 = i[0];
            int pointIndex1 = i[1];
            float value = float.Parse(poseConfigurations[1].xCoordinateTolerance[i]);
            if (results[pointIndex0].confidence > 0.3f && results[pointIndex1].confidence > 0.3f)
            {
                float x0 = results[pointIndex0].x;
                float x1 = results[pointIndex1].x;

                float x_normalization = Mathf.Abs(Mathf.Abs(x0 - x1) - value) / (value - 0);

                Color color = new Color(1.0f, 0.0f, 0.0f);
                color = new Color(x_normalization, 1 - x_normalization, 0);

                draw.color = color;
                MoveNet.Result[] line = new MoveNet.Result[2];
                line[0] = results[pointIndex0];
                line[1] = results[pointIndex1];
                DrawLine(line);

                if (x_normalization <= 0.5)
                {
                    matched.Add(true);
                }
                else
                {
                    matched.Add(false);
                }
            }
            else
            {
                matched.Add(false);
            }
        }

        // yCoordinateTolerance
        foreach (List<int> i in poseConfigurations[1].yCoordinateTolerance.Keys)
        {
            int pointIndex0 = i[0];
            int pointIndex1 = i[1];
            float value = float.Parse(poseConfigurations[1].yCoordinateTolerance[i]);
            if (results[pointIndex0].confidence > 0.3f && results[pointIndex1].confidence > 0.3f)
            {
                float y0 = results[pointIndex0].y;
                float y1 = results[pointIndex1].y;

                float y_normalization = Mathf.Abs(Mathf.Abs(y0 - y1) - value) / (value - 0);

                Color color = new Color(1.0f, 0.0f, 0.0f);
                color = new Color(y_normalization, 1 - y_normalization, 0);

                draw.color = color;
                MoveNet.Result[] line = new MoveNet.Result[2];
                line[0] = results[pointIndex0];
                line[1] = results[pointIndex1];
                // DrawLine(line);

                if (y_normalization <= 0.5)
                {
                    matched.Add(true);
                }
                else
                {
                    matched.Add(false);
                }
            }
            else
            {
                matched.Add(false);
            }
        }

        // angle
        foreach (List<int> i in poseConfigurations[1].angles.Keys)
        {
            int pointIndex0 = i[0];
            int pointIndex1 = i[1];
            int pointIndex2 = i[2];
            float value = float.Parse(poseConfigurations[1].angles[i]);
            if (results[pointIndex0].confidence > 0.3f && results[pointIndex1].confidence > 0.3f && results[pointIndex2].confidence > 0.3f)
            {
                MoveNet.Result[] angle = new MoveNet.Result[3];

                angle[0] = results[pointIndex0];
                angle[1] = results[pointIndex1];
                angle[2] = results[pointIndex2];

                float[] p0 = { results[pointIndex0].x, results[pointIndex0].y };
                float[] p1 = { results[pointIndex1].x, results[pointIndex1].y };
                float[] p2 = { results[pointIndex2].x, results[pointIndex2].y };

                float angle_normalization = Mathf.Abs(utilities.CalculateAngle3Points(p0, p1, p2) - value) / (value - 0);

                Color color = new Color(1.0f, 0.0f, 0.0f);
                color = new Color(angle_normalization, 1 - angle_normalization, 0);

                draw.color = color;

                DrawAngle(angle);

                if (angle_normalization <= 0.3)
                {
                    matched.Add(true);
                }
                else
                {
                    matched.Add(false);
                }
            }
            else
            {
                matched.Add(false);
            }
        }

        if (matched.Count == 0)
        {
            return false;
        }

        foreach (bool i in matched)
        {
            if (!i)
            {
                return false;
            }
        }

        return true;
    }

    private bool UpperTrapStretchRight()
    {
        if (results[10].confidence > 0.3f && results[8].confidence > 0.3f && results[6].confidence > 0.3f)
        {
            foreach (List<int> i in poseConfigurations[0].horizontalRelation.Keys)
            {
                float value = float.Parse(poseConfigurations[0].horizontalRelation[i]);
                if (value == 0)
                {
                    // i[0].x should be < i[1].x but we only need the wrong case to visualization.
                    if (results[i[0]].x > results[i[1]].x)
                    {
                        draw.color = Color.red;
                        DrawResult(results);
                        return false;
                    }
                }
                else if (value == 1)
                {
                    // i[0].x should be > i[1].x but we only need the wrong case to visualization.
                    if (results[i[0]].x < results[i[1]].x)
                    {
                        draw.color = Color.red;
                        DrawResult(results);
                        return false;
                    }
                }
                else
                {
                    Debug.Log("horizontalRelation value error");
                }
            }
            foreach (List<int> i in poseConfigurations[0].verticalRelation.Keys)
            {
                float value = float.Parse(poseConfigurations[0].verticalRelation[i]);
                if (value == 0)
                {
                    // i[0].x should be < i[1].x but we only need the wrong case to visualization.
                    if (results[i[0]].x > results[i[1]].x)
                    {
                        draw.color = Color.red;
                        DrawResult(results);
                        return false;
                    }
                }
                else if (value == 1)
                {
                    // i[0].x should be > i[1].x but we only need the wrong case to visualization.
                    if (results[i[0]].x < results[i[1]].x)
                    {
                        draw.color = Color.red;
                        DrawResult(results);
                        return false;
                    }
                }
                else
                {
                    Debug.Log("horizontalRelation value error");
                }
            }
            //if (results[8].y > results[6].y || results[10].x < results[8].x)
            //{
            //    draw.color = Color.red;
            //    DrawResult(results);
            //    return false;
            //}
        }
        List<bool> matched = new List<bool>();

        // xCoordinateTolerance
        foreach (List<int> i in poseConfigurations[2].xCoordinateTolerance.Keys)
        {
            int pointIndex0 = i[0];
            int pointIndex1 = i[1];
            float value = float.Parse(poseConfigurations[2].xCoordinateTolerance[i]);
            if (results[pointIndex0].confidence > 0.3f && results[pointIndex1].confidence > 0.3f)
            {
                float x0 = results[pointIndex0].x;
                float x1 = results[pointIndex1].x;

                float x_normalization = Mathf.Abs(Mathf.Abs(x0 - x1) - value) / (value - 0);

                Color color = new Color(1.0f, 0.0f, 0.0f);
                color = new Color(x_normalization, 1 - x_normalization, 0);

                draw.color = color;
                MoveNet.Result[] line = new MoveNet.Result[2];
                line[0] = results[pointIndex0];
                line[1] = results[pointIndex1];
                DrawLine(line);

                if (x_normalization <= 0.5)
                {
                    matched.Add(true);
                }
                else
                {
                    matched.Add(false);
                }
            }
            else
            {
                matched.Add(false);
            }
        }

        // yCoordinateTolerance
        foreach (List<int> i in poseConfigurations[2].yCoordinateTolerance.Keys)
        {
            int pointIndex0 = i[0];
            int pointIndex1 = i[1];
            float value = float.Parse(poseConfigurations[2].yCoordinateTolerance[i]);
            if (results[pointIndex0].confidence > 0.3f && results[pointIndex1].confidence > 0.3f)
            {
                float y0 = results[pointIndex0].y;
                float y1 = results[pointIndex1].y;

                float y_normalization = Mathf.Abs(Mathf.Abs(y0 - y1) - value) / (value - 0);

                Color color = new Color(1.0f, 0.0f, 0.0f);
                color = new Color(y_normalization, 1 - y_normalization, 0);

                draw.color = color;
                MoveNet.Result[] line = new MoveNet.Result[2];
                line[0] = results[pointIndex0];
                line[1] = results[pointIndex1];
                // DrawLine(line);

                if (y_normalization <= 0.5)
                {
                    matched.Add(true);
                }
                else
                {
                    matched.Add(false);
                }
            }
            else
            {
                matched.Add(false);
            }
        }

        // angle
        foreach (List<int> i in poseConfigurations[2].angles.Keys)
        {
            int pointIndex0 = i[0];
            int pointIndex1 = i[1];
            int pointIndex2 = i[2];
            float value = float.Parse(poseConfigurations[2].angles[i]);
            if (results[pointIndex0].confidence > 0.3f && results[pointIndex1].confidence > 0.3f && results[pointIndex2].confidence > 0.3f)
            {
                MoveNet.Result[] angle = new MoveNet.Result[3];

                angle[0] = results[pointIndex0];
                angle[1] = results[pointIndex1];
                angle[2] = results[pointIndex2];

                float[] p0 = { results[pointIndex0].x, results[pointIndex0].y };
                float[] p1 = { results[pointIndex1].x, results[pointIndex1].y };
                float[] p2 = { results[pointIndex2].x, results[pointIndex2].y };

                float angle_normalization = Mathf.Abs(utilities.CalculateAngle3Points(p0, p1, p2) - value) / (value - 0);

                Color color = new Color(1.0f, 0.0f, 0.0f);
                color = new Color(angle_normalization, 1 - angle_normalization, 0);

                draw.color = color;

                DrawAngle(angle);

                if (angle_normalization <= 0.1)
                {
                    matched.Add(true);
                }
                else
                {
                    matched.Add(false);
                }
            }
            else
            {
                matched.Add(false);
            }
        }

        if (matched.Count == 0)
        {
            return false;
        }

        foreach (bool i in matched)
        {
            if (!i)
            {
                return false;
            }
        }

        return true;
    }

    private bool NeckIsometricExercise()
    {
        if (results[9].confidence > 0.3f && results[10].confidence > 0.3f)
        {
            if (results[9].y > results[0].y || results[9].y > results[0].y || results[10].y > results[0].y || results[10].y > results[0].y)
            {
                draw.color = Color.red;
                DrawResult(results);
                return false;
            }
        }
        List<bool> matched = new List<bool>();

        // xCoordinateTolerance
        foreach (List<int> i in poseConfigurations[3].xCoordinateTolerance.Keys)
        {
            int pointIndex0 = i[0];
            int pointIndex1 = i[1];
            float value = float.Parse(poseConfigurations[3].xCoordinateTolerance[i]);
            if (results[pointIndex0].confidence > 0.3f && results[pointIndex1].confidence > 0.3f)
            {
                float x0 = results[pointIndex0].x;
                float x1 = results[pointIndex1].x;

                float x_normalization = Mathf.Abs(Mathf.Abs(x0 - x1) - value) / (value - 0);

                Color color = new Color(1.0f, 0.0f, 0.0f);
                color = new Color(x_normalization, 1 - x_normalization, 0);

                draw.color = color;
                MoveNet.Result[] line = new MoveNet.Result[2];
                line[0] = results[pointIndex0];
                line[1] = results[pointIndex1];
                DrawLine(line);

                if (x_normalization <= 0.5)
                {
                    matched.Add(true);
                }
                else
                {
                    matched.Add(false);
                }
            }
            else
            {
                matched.Add(false);
            }
        }

        // yCoordinateTolerance
        foreach (List<int> i in poseConfigurations[3].yCoordinateTolerance.Keys)
        {
            int pointIndex0 = i[0];
            int pointIndex1 = i[1];
            float value = float.Parse(poseConfigurations[3].yCoordinateTolerance[i]);
            if (results[pointIndex0].confidence > 0.3f && results[pointIndex1].confidence > 0.3f)
            {
                float y0 = results[pointIndex0].y;
                float y1 = results[pointIndex1].y;

                float y_normalization = Mathf.Abs(Mathf.Abs(y0 - y1) - value) / (value - 0);

                Color color = new Color(1.0f, 0.0f, 0.0f);
                color = new Color(y_normalization, 1 - y_normalization, 0);

                draw.color = color;
                MoveNet.Result[] line = new MoveNet.Result[2];
                line[0] = results[pointIndex0];
                line[1] = results[pointIndex1];
                // DrawLine(line);

                if (y_normalization <= 0.5)
                {
                    matched.Add(true);
                }
                else
                {
                    matched.Add(false);
                }
            }
            else
            {
                matched.Add(false);
            }
        }

        // angle
        foreach (List<int> i in poseConfigurations[3].angles.Keys)
        {
            int pointIndex0 = i[0];
            int pointIndex1 = i[1];
            int pointIndex2 = i[2];
            float value = float.Parse(poseConfigurations[3].angles[i]);
            if (results[pointIndex0].confidence > 0.3f && results[pointIndex1].confidence > 0.3f && results[pointIndex2].confidence > 0.3f)
            {
                MoveNet.Result[] angle = new MoveNet.Result[3];

                angle[0] = results[pointIndex0];
                angle[1] = results[pointIndex1];
                angle[2] = results[pointIndex2];

                float[] p0 = { results[pointIndex0].x, results[pointIndex0].y };
                float[] p1 = { results[pointIndex1].x, results[pointIndex1].y };
                float[] p2 = { results[pointIndex2].x, results[pointIndex2].y };

                float angle_normalization = Mathf.Abs(utilities.CalculateAngle3Points(p0, p1, p2) - value) / (value - 0);

                Color color = new Color(1.0f, 0.0f, 0.0f);
                color = new Color(angle_normalization, 1 - angle_normalization, 0);

                draw.color = color;

                DrawAngle(angle);

                if (angle_normalization <= 0.5)
                {
                    matched.Add(true);
                }
                else
                {
                    matched.Add(false);
                }
            }
            else
            {
                matched.Add(false);
            }
        }

        if (matched.Count == 0)
        {
            return false;
        }

        foreach (bool i in matched)
        {
            if (!i)
            {
                return false;
            }
        }

        return true;
    }
}