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


    protected void Start()
    {

        // string filePath = Directory.GetCurrentDirectory() + @"\Assets\Resources\TFLiteModels\" + fileName;
        string filePath = Application.streamingAssetsPath + "/" + fileName;
        Debug.Log(filePath);
        moveNet = new MoveNet(filePath);
        // utilities = new Utilities();
        GameObject gameObject = new GameObject("Utilities");
        utilities = gameObject.AddComponent<Utilities>();

        poseInfoSettings = new List<MoveNet.Result[]>();

        var webCamInput = GetComponent<WebCamInput>();
        webCamInput.OnTextureUpdate.AddListener(OnTextureUpdate);

        draw = new PrimitiveDraw(Camera.main, gameObject.layer)
        {
            color = Color.red,
        };

        ReadTxt();
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
        //results = moveNet.GetResults();
        results = moveNet.GetResults();
        DrawResult(results);
        //PoseEstimation(results);
        //for (int i = 0; i < angles.Count; i += 4)
        //{
        //    // i point1 index, i + 1 point2 index, i + 2 point3 index
        //    // e.g 5 - 7 - 9
        //    int point1_index = angles[i];
        //    int point2_index = angles[i + 1];
        //    int point3_index = angles[i + 2];
        //    if (results[point1_index].confidence < 0.6f && results[point2_index].confidence < 0.6f && results[point3_index].confidence < 0.6f)
        //    {
        //        continue;
        //    }

        //    MoveNet.Result[] angle = new MoveNet.Result[3];
        //    angle[0] = results[point1_index];
        //    angle[1] = results[point2_index];
        //    angle[2] = results[point3_index];

        //    float[] p_1 = { angle[0].y, angle[0].x };
        //    float[] p_2 = { angle[1].y, angle[1].x };
        //    float[] p_3 = { angle[2].y, angle[2].x };

        //    // angle at point1 - 2 - 3
        //    float anglep1p2p3 = utilities.CalculateAngle3Points(p_1, p_2, p_3);

        //    // (255, 0, 0) (0, 255, 0)
        //    // linear interpolation between red and green
        //    // pure red 255, 0, 0
        //    float color_angle = Mathf.Abs(70 - anglep1p2p3);
        //    float color_point_1_diff = Mathf.Abs(angle[0].y - angle[1].y);

        //    // normalization
        //    float color_angle_normalization = color_angle / (50 - 0);
        //    //float color_point_1_diff_normalization = color_point_1_diff / (0.15f - 0);

        //    //Debug.Log(color_point_1_diff_normalization);

        //    // 1 = pure red 0 = pure green others are interpolation
        //    Color color = new Color(1.0f, 0.0f, 0.0f);
        //    float final_color = 0;
        //    final_color = color_angle_normalization;

        //    if (final_color > 1.0f) final_color = 1.0f;
        //    if (final_color < 0.0f) final_color = 0.0f;
        //    color = new Color(final_color, 1.0f - final_color, 0);
        //    Debug.Log(anglep1p2p3);
        //    //draw.color = float2rgb(final_color);
        //    draw.color = color;
        //    DrawAngle(angle);
        //    if (Mathf.Abs(70 - anglep1p2p3) > 45 || Mathf.Abs(results[7].y - results[9].y) > 0.15 || Mathf.Abs(results[8].y - results[10].y) > 0.15 || Mathf.Abs(results[7].y - results[10].y) > 0.15)
        //    {
        //        draw.color = new Color(255, 0, 0);
        //        DrawAngle(angle);
        //    }
        //    //if(final_color < 0.1)
        //    //{
        //    //    draw.color = new Color(0, final_color, 0);
        //    //    DrawAngle(angle);
        //    //}
        //    //else if(final_color < 0.3)
        //    //{
        //    //    draw.color = new Color(77.0f, 155.0f, 0);
        //    //    DrawAngle(angle);
        //    //}
        //    //else if (final_color < 0.5)
        //    //{
        //    //    draw.color = new Color(100.0f, 100.0f, 0);
        //    //    DrawAngle(angle);
        //    //}
        //    //else if (final_color < 0.5)
        //    //{
        //    //    draw.color = new Color(155.0f, 77.0f, 0);
        //    //    DrawAngle(angle);
        //    //}
        //    //else
        //    //{
        //    //    draw.color = new Color(255.0f, 0, 0);
        //    //    DrawAngle(angle);
        //    //}

        //    //if (Mathf.Abs(70 - anglep1p2p3) > 45 || Mathf.Abs(results[7].y - results[9].y) > 0.15 || Mathf.Abs(results[8].y - results[10].y) > 0.15 || Mathf.Abs(results[7].y - results[10].y) > 0.15)
        //    //{
        //    //    draw.color = new Color(255, 0, 0);
        //    //    DrawAngle(angle);
        //    //}

        //    //// interpolation 255, 255, 0
        //    //else if (Mathf.Abs(70 - anglep1p2p3) > 30 || Mathf.Abs(results[7].y - results[9].y) > 0.10 || Mathf.Abs(results[8].y - results[10].y) > 0.10 || Mathf.Abs(results[7].y - results[10].y) > 0.10)
        //    //{
        //    //    draw.color = new Color(255, 255, 0);
        //    //    DrawAngle(angle);
        //    //}
        //    //// pure green 0, 255, 0
        //    //else if (Mathf.Abs(70 - anglep1p2p3) > 15 || Mathf.Abs(results[7].y - results[9].y) > 0.05 || Mathf.Abs(results[8].y - results[10].y) > 0.05 || Mathf.Abs(results[7].y - results[10].y) > 0.05)
        //    //{
        //    //    draw.color = new Color(0, 255, 0);
        //    //    DrawAngle(angle);
        //    //}
        //}


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
        string name = utilities.ReadJson("Assets/Resources/Configurations/" + jsonName, "poseName")[0];

        jsonSettings = utilities.ReadJson("Assets/Resources/Configurations/" + jsonName, "poseDetectValueArray");
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
            else
            {
                Debug.Log("unknown type of poseDetectValueArray");
            }
        }
        poseConfigurations.Add(temPoseConfigurations);


    }

    protected virtual void PoseEstimation() { }

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
