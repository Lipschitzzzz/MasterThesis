using System.Threading;
using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using System.Globalization;
using TensorFlowLite;
using Cysharp.Threading.Tasks;
using System.IO;
using System;

[RequireComponent(typeof(WebCamInput))]
public class MoveNetSinglePose : MonoBehaviour
{
    [SerializeField]
    public string fileName;
    public List<string> txtNames;

    [SerializeField]
    protected RawImage cameraView = null;

    [SerializeField, Range(0, 1)]
    private float threshold = 0.3f;

    protected MoveNet moveNet;
    private readonly Vector3[] rtCorners = new Vector3[4];
    protected MoveNet.Result[] results;
    protected List<MoveNet.Result[]> poseInfoSettings;

    protected PrimitiveDraw draw;
    protected Utilities utilities;

    protected List<string> jsonSettings;

    public GameObject figure;
    public List<Texture> textures;
    protected PlayerInfo playerInfo;
    protected bool enableVisualization;


    protected void Start()
    {

        string filePath = Application.streamingAssetsPath + "/" + fileName;
#if UNITY_EDITOR
        filePath = Application.streamingAssetsPath + "/" + fileName;
#elif UNITY_ANDROID
        filePath = "jar:file://" + Application.dataPath + "!/assets/" + fileName;
#else
        filePath = Application.streamingAssetsPath + "/" + fileName;
#endif
        moveNet = new MoveNet(filePath);
        utilities = new Utilities();
        

        poseInfoSettings = new List<MoveNet.Result[]>();

        draw = new PrimitiveDraw(Camera.main, gameObject.layer)
        {
            color = Color.green,
        };
        var webCamInput = GetComponent<WebCamInput>();
        webCamInput.OnTextureUpdate.AddListener(OnTextureUpdate);
        // ReadTxt();
        results = moveNet.GetResults();

    }

    protected void OnDestroy()
    {
        var webCamInput = GetComponent<WebCamInput>();
        webCamInput.OnTextureUpdate.RemoveListener(OnTextureUpdate);
        moveNet?.Dispose();
        draw?.Dispose();
    }

    private void Update()
    {
        // Invoke(cameraView.texture);
        // DrawResult(results);
    }

    protected void ReadTxt()
    {
        foreach(string name in txtNames)
        {
            string[] lines = System.IO.File.ReadAllLines(Directory.GetCurrentDirectory() + @"\Assets\Resources\Configurations\" + name);

            int i = 0;
            MoveNet.Result[] temp = new MoveNet.Result[17];
            foreach (string line in lines)
            {
                string[] words = line.Split(',');
                temp[i] = new MoveNet.Result(y: float.Parse(words[0]), x: float.Parse(words[1]), confidence: float.Parse(words[2]));

                i++;
            }
            poseInfoSettings.Add(temp);
        }
        
    }

    protected void ReadJson(List<PoseConfigurations> poseConfigurations, string jsonName)
    {
        string name = "";
#if UNITY_EDITOR
        name = utilities.ReadJson(Application.dataPath + "/" + jsonName, "poseName")[0];
        jsonSettings = utilities.ReadJson(Application.dataPath + "/" + jsonName, "poseDetectValueArray");
#elif UNITY_IOS || UNITY_IPHONE
        name = utilities.ReadJson("file://" + Application.streamingAssetsPath + "/" + jsonName, "poseName")[0];
        jsonSettings = utilities.ReadJson("file://" + Application.streamingAssetsPath + "/" + jsonName, "poseDetectValueArray");
#elif UNITY_ANDROID
        name = utilities.ReadJson(Application.dataPath + "/" + jsonName, "poseName")[0];
        jsonSettings = utilities.ReadJson(Application.dataPath + "/" + jsonName, "poseDetectValueArray");
#else
        name = utilities.ReadJson(Application.dataPath + "/StreamingAssets" + "/" + jsonName, "poseName")[0];
        jsonSettings = utilities.ReadJson(Application.dataPath + "/StreamingAssets" + "/" + jsonName, "poseDetectValueArray");
#endif

        PoseConfigurations temPoseConfigurations = new PoseConfigurations();
        temPoseConfigurations.poseName = name;
        for (int k = 0; k < jsonSettings.Count; k += 3)
        {
            if (jsonSettings[k] == "angle")
            {
                List<int> temp = new List<int>();

                foreach (var point_index in jsonSettings[k + 1].Split('-'))
                {
                    temp.Add(int.Parse(point_index));
                }
                temPoseConfigurations.angles.Add(temp, jsonSettings[k + 2]);

            }
            else if (jsonSettings[k] == "x_coordinate_tolerance")
            {
                List<int> temp = new List<int>();
                foreach (var point_index in jsonSettings[k + 1].Split('-'))
                {
                    temp.Add(int.Parse(point_index));
                }
                temPoseConfigurations.xCoordinateTolerance.Add(temp, jsonSettings[k + 2]);
            }
            else if (jsonSettings[k] == "y_coordinate_tolerance")
            {
                List<int> temp = new List<int>();
                foreach (var point_index in jsonSettings[k + 1].Split('-'))
                {
                    temp.Add(int.Parse(point_index));
                }
                temPoseConfigurations.yCoordinateTolerance.Add(temp, jsonSettings[k + 2]);
            }
            else if (jsonSettings[k] == "x_relative_distance")
            {
                List<int> temp = new List<int>();
                foreach (var point_index in jsonSettings[k + 1].Split('-'))
                {
                    temp.Add(int.Parse(point_index));
                }
                temPoseConfigurations.xRelativeDistance.Add(temp, jsonSettings[k + 2]);
            }
            else if (jsonSettings[k] == "y_relative_distance")
            {
                List<int> temp = new List<int>();
                foreach (var point_index in jsonSettings[k + 1].Split('-'))
                {
                    temp.Add(int.Parse(point_index));
                }
                temPoseConfigurations.yRelativeDistance.Add(temp, jsonSettings[k + 2]);
            }
            else if (jsonSettings[k] == "vertical")
            {
                List<int> temp = new List<int>();
                foreach (var point_index in jsonSettings[k + 1].Split('-'))
                {
                    temp.Add(int.Parse(point_index));
                }
                temPoseConfigurations.verticalRelation.Add(temp, jsonSettings[k + 2]);
            }
            else if (jsonSettings[k] == "horizontal")
            {
                List<int> temp = new List<int>();
                foreach (var point_index in jsonSettings[k + 1].Split('-'))
                {
                    temp.Add(int.Parse(point_index));
                }
                temPoseConfigurations.horizontalRelation.Add(temp, jsonSettings[k + 2]);
            }
            else
            {
                Debug.Log("unknown type of poseDetectValueArray: " + jsonSettings[k]);
            }
        }
        poseConfigurations.Add(temPoseConfigurations);


    }

    protected void InitializeJson(List<PoseConfigurations> poseConfigurations, string jsonName)
    {

        PoseConfigurations temPoseConfigurations = new PoseConfigurations();
        temPoseConfigurations.poseName = name;
        List<int> temp = new List<int>();
        temp.Add(5);
        temp.Add(7);
        temp.Add(9);
        temPoseConfigurations.angles.Add(temp, "60");

        poseConfigurations.Add(temPoseConfigurations);
    }

    protected virtual void PoseEstimation() { }

    // generic pose estimation function, always parse all data in the PoseConfigurations
    protected bool ParsePoseConfigurations(PoseConfigurations poseConfigurations)
    {
        List<bool> matched = new List<bool>();

        // horizontal
        foreach (List<int> i in poseConfigurations.horizontalRelation.Keys)
        {
            if (results[i[0]].confidence > 0.3f && results[i[1]].confidence > 0.3f)
            {
                float value = float.Parse(poseConfigurations.horizontalRelation[i]);
                if (value == 0)
                {
                    // i[0].x should be left i[1].x but we only need the wrong case to visualization.
                    if (results[i[0]].x > results[i[1]].x)
                    {
                        draw.color = Color.red;
                        if (enableVisualization)
                        {
                            DrawResult(results);
                        }
                        matched.Add(false);
                    }
                    else
                    {
                        matched.Add(true);
                    }
                }
                else if (value == 1)
                {
                    // i[0].x should be right i[1].x but we only need the wrong case to visualization.
                    if (results[i[0]].x < results[i[1]].x)
                    {
                        draw.color = Color.red;
                        if (enableVisualization)
                        {
                            DrawResult(results);
                        }
                        matched.Add(false);
                    }
                    else
                    {
                        matched.Add(true);
                    }
                }
                else
                {
                    Debug.Log("horizontalRelation value error");
                }
            }
            else
            {
                matched.Add(false);
            }

        }

        // vertical
        foreach (List<int> i in poseConfigurations.verticalRelation.Keys)
        {
            if (results[i[0]].confidence > 0.3f && results[i[1]].confidence > 0.3f)
            {
                float value = float.Parse(poseConfigurations.verticalRelation[i]);
                if (value == 0)
                {
                    // i[0].y should be over i[1].y but we only need the wrong case to visualization.
                    if (results[i[0]].y > results[i[1]].y)
                    {
                        draw.color = Color.red;
                        if (enableVisualization)
                        {
                            DrawResult(results);
                        }
                        matched.Add(false);
                    }
                    else
                    {
                        matched.Add(true);
                    }
                }
                else if (value == 1)
                {
                    // i[0].y should be under i[1].y but we only need the wrong case to visualization.
                    if (results[i[0]].y < results[i[1]].y)
                    {
                        draw.color = Color.red;
                        if (enableVisualization)
                        {
                            DrawResult(results);
                        }
                        matched.Add(false);
                    }
                    else
                    {
                        matched.Add(true);
                    }
                }
                else
                {
                    Debug.Log("verticalRelation value error");
                }
            }
            else
            {
                matched.Add(false);
            }
        }

        // xRelativeDistance
        foreach (List<int> i in poseConfigurations.xRelativeDistance.Keys)
        {
            // line0 point0 - point1
            int pointIndex0 = i[0];
            int pointIndex1 = i[1];

            // line1 point2 - point3
            int pointIndex2 = i[2];
            int pointIndex3 = i[3];

            // proportion = distance(line0 / line1)
            float proportion = float.Parse(poseConfigurations.xRelativeDistance[i]);

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
                if (enableVisualization)
                {
                    DrawLine(line);
                }
            }
            else
            {
                matched.Add(false);
            }
        }

        // yRelativeDistance
        foreach (List<int> i in poseConfigurations.yRelativeDistance.Keys)
        {
            // line0 point0 - point1
            int pointIndex0 = i[0];
            int pointIndex1 = i[1];

            // line1 point2 - point3
            int pointIndex2 = i[2];
            int pointIndex3 = i[3];

            // proportion = distance(line0 / line1)
            float proportion = float.Parse(poseConfigurations.yRelativeDistance[i]);

            // Debug.Log(pointIndex0.ToString() + ' ' + pointIndex1.ToString() + ' ' +
            //     pointIndex2.ToString() + ' ' + pointIndex3.ToString() + ' ' + proportion.ToString());

            if (results[pointIndex0].confidence > 0.3f && results[pointIndex1].confidence > 0.3f && results[pointIndex2].confidence > 0.3f && results[pointIndex3].confidence > 0.3f)
            {
                float line0 = Mathf.Abs(results[pointIndex0].y - results[pointIndex1].y);
                float line1 = Mathf.Abs(results[pointIndex2].y - results[pointIndex3].y);

                float tolerance = line1 * proportion;
                if (line0 < tolerance)
                {
                    matched.Add(true);
                }
                else
                {
                    matched.Add(false);
                }

                MoveNet.Result[] line = new MoveNet.Result[2];
                line[0] = results[pointIndex0];
                line[1] = results[pointIndex1];


            }
            else
            {
                matched.Add(false);
            }
        }

        // angle
        foreach (List<int> i in poseConfigurations.angles.Keys)
        {
            // point0 point1 point2
            int pointIndex0 = i[0];
            int pointIndex1 = i[1];
            int pointIndex2 = i[2];

            // angle between point0 point1 point2
            float value = float.Parse(poseConfigurations.angles[i]);

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

                if (enableVisualization)
                {
                    DrawAngle(angle);
                }


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

        // vector
        foreach (List<int> i in poseConfigurations.vectors.Keys)
        {
            // point0 point1 point2 point3
            int pointIndex0 = i[0];
            int pointIndex1 = i[1];
            int pointIndex2 = i[2];
            int pointIndex3 = i[3];
            // angle between point0 point1 point2
            float value = float.Parse(poseConfigurations.vectors[i]);

            if (results[pointIndex0].confidence > 0.3f && results[pointIndex1].confidence > 0.3f
                && results[pointIndex2].confidence > 0.3f && results[pointIndex3].confidence > 0.3f)
            {
                MoveNet.Result[] v1 = new MoveNet.Result[2];
                MoveNet.Result[] v2 = new MoveNet.Result[2];

                v1[0] = results[pointIndex0];
                v1[1] = results[pointIndex1];
                v2[0] = results[pointIndex2];
                v2[1] = results[pointIndex3];

                float[] p0 = { results[pointIndex0].x, results[pointIndex0].y };
                float[] p1 = { results[pointIndex1].x, results[pointIndex1].y };
                float[] p2 = { results[pointIndex2].x, results[pointIndex2].y };
                float[] p3 = { results[pointIndex3].x, results[pointIndex3].y };

                float angle_normalization = Mathf.Abs((utilities.CalculateAngle4Points(p0, p1, p2, p3) - value)) / (value - 0);

                if (angle_normalization <= 0.05 && angle_normalization > 0.0)
                {
                    draw.color = Color.green;
                    matched.Add(true);
                }
                else
                {
                    draw.color = Color.red;
                    matched.Add(false);
                }

                if (enableVisualization)
                {
                    DrawLine(v1);
                    // DrawLine(v2);
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

    private void OnTextureUpdate(Texture texture)
    {
        Invoke(texture);
    }

    private void Invoke(Texture texture)
    {
        moveNet.Invoke(texture);
        results = moveNet.GetResults();
        cameraView.material = moveNet.transformMat;
    }

    // draw the angles between points[0]-points[1]-points[2]
    protected void DrawAngle(MoveNet.Result[] points)
    {
        if (points == null || points.Length != 3)
        {
            Debug.Log("DrawAngle points size error");
            return;
        }

        var rect = cameraView.GetComponent<RectTransform>();
        rect.GetWorldCorners(rtCorners);
        Vector3 min = rtCorners[0];
        Vector3 max = rtCorners[2];
        draw.Line3D(MathTF.Lerp(min, max, new Vector3(points[0].x, 1f - points[0].y, 0)),
                    MathTF.Lerp(min, max, new Vector3(points[1].x, 1f - points[1].y, 0)),
                    1
                );
        draw.Line3D(MathTF.Lerp(min, max, new Vector3(points[1].x, 1f - points[1].y, 0)),
                    MathTF.Lerp(min, max, new Vector3(points[2].x, 1f - points[2].y, 0)),
                    1
                );

        draw.Apply();
    }

    protected void DrawLine(MoveNet.Result[] points)
    {
        if (points == null || points.Length != 2)
        {
            Debug.Log("DrawLine points size error");
            return;
        }

        var rect = cameraView.GetComponent<RectTransform>();
        rect.GetWorldCorners(rtCorners);
        Vector3 min = rtCorners[0];
        Vector3 max = rtCorners[2];
        draw.Line3D(MathTF.Lerp(min, max, new Vector3(points[0].x, 1f - points[0].y, 0)),
                    MathTF.Lerp(min, max, new Vector3(points[1].x, 1f - points[1].y, 0)),
                    1
                );

        draw.Apply();
    }

    private Color float2rgb(float zero2one)
    {
        float x = zero2one * 256 * 256;
        float r = x % 256;
        float g = ((1 - r) / 256 % 256);
        float b = 0;
        r = r / 256;
        g = g / 256;
        Color color = new Color(r, g, b);
        return color;
    }
    

    protected void DrawResult(MoveNet.Result[] results)
    {
        if (results == null || results.Length == 0)
        {
            return;
        }

        var rect = cameraView.GetComponent<RectTransform>();
        rect.GetWorldCorners(rtCorners);
        Vector3 min = rtCorners[0];
        Vector3 max = rtCorners[2];

        var connections = MoveNet.Connections;
        int len = connections.GetLength(0);
        for (int i = 0; i < len; i++)
        {
            var a = results[(int)connections[i, 0]];
            var b = results[(int)connections[i, 1]];
            if (a.confidence >= threshold && b.confidence >= threshold)
            {
                draw.Line3D(
                    MathTF.Lerp(min, max, new Vector3(a.x, 1f - a.y, 0)),
                    MathTF.Lerp(min, max, new Vector3(b.x, 1f - b.y, 0)),
                    1
                );
            }
        }

        draw.Apply();
    }
}
