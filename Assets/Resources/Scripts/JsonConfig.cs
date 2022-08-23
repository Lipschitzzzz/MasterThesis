using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine.Networking;
using System.Text;
using System;
using UnityEngine;


[System.Serializable]
public class SubData
{
    public string type;
    public string points;
    public float value;
    public string comment;
}

[System.Serializable]
public class JsonConfig
{
    public int poseIndex;
    public string poseName;
    public List<SubData> poseDetectValueArray;
    public JsonConfig()
    {
        poseDetectValueArray = new List<SubData>();
    }

    public static JsonConfig CreateFromJSON(string jsonString)
    {
        return JsonUtility.FromJson<JsonConfig>(jsonString);
    }
    public void ReadJson(List<PoseConfigurations> poseConfigurations, string jsonName)
    {
        PoseConfigurations temPoseConfigurations = new PoseConfigurations();
        TextAsset textFile = Resources.Load<TextAsset>("JsonConfigs/" + jsonName);
        string content = textFile.ToString();
        JsonConfig result = CreateFromJSON(content);
        
        foreach(SubData i in result.poseDetectValueArray)
        {
            if (i.type == "angle")
            {
                List<int> temp = new List<int>();
                foreach (string point_index in i.points.Split('-'))
                {
                    temp.Add(int.Parse(point_index));
                }
                temPoseConfigurations.angles.Add(temp, i.value.ToString());
            }
            else if (i.type == "x_coordinate_tolerance")
            {
                List<int> temp = new List<int>();
                foreach (string point_index in i.points.Split('-'))
                {
                    temp.Add(int.Parse(point_index));
                }
                temPoseConfigurations.xCoordinateTolerance.Add(temp, i.value.ToString());
            }
            else if (i.type == "y_coordinate_tolerance")
            {
                List<int> temp = new List<int>();
                foreach (string point_index in i.points.Split('-'))
                {
                    temp.Add(int.Parse(point_index));
                }
                temPoseConfigurations.yCoordinateTolerance.Add(temp, i.value.ToString());
            }
            else if (i.type == "x_relative_distance")
            {
                List<int> temp = new List<int>();
                foreach (string point_index in i.points.Split('-'))
                {
                    temp.Add(int.Parse(point_index));
                }
                temPoseConfigurations.xRelativeDistance.Add(temp, i.value.ToString());
            }
            else if (i.type == "y_relative_distance")
            {
                List<int> temp = new List<int>();
                foreach (string point_index in i.points.Split('-'))
                {
                    temp.Add(int.Parse(point_index));
                }
                temPoseConfigurations.yRelativeDistance.Add(temp, i.value.ToString());
            }
            else if (i.type == "vertical")
            {
                List<int> temp = new List<int>();
                foreach (string point_index in i.points.Split('-'))
                {
                    temp.Add(int.Parse(point_index));
                }
                temPoseConfigurations.verticalRelation.Add(temp, i.value.ToString());
            }
            else if (i.type == "horizontal")
            {
                List<int> temp = new List<int>();
                foreach (string point_index in i.points.Split('-'))
                {
                    temp.Add(int.Parse(point_index));
                }
                temPoseConfigurations.horizontalRelation.Add(temp, i.value.ToString());
            }
            else
            {
                Debug.Log("unknown type of poseDetectValueArray: " + i.type);
            }

        }
        poseConfigurations.Add(temPoseConfigurations);

    }

}