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
        GameObject gameObject = new GameObject("Utilities");
        utilities = gameObject.AddComponent<Utilities>();

        poseInfoSettings = new List<MoveNet.Result[]>();

        var webCamInput = GetComponent<WebCamInput>();
        webCamInput.OnTextureUpdate.AddListener(OnTextureUpdate);

        draw = new PrimitiveDraw(Camera.main, gameObject.layer)
        {
            color = Color.red,
        };

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
