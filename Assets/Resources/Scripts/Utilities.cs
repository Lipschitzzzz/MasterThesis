using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;

public class Utilities : MonoBehaviour
{

    private float angle_3_points;

    // calculate the angle between the given 3 points. After convert from proportion to absolute coordinate.
    public float CalculateAngle3Points(float[] a, float[] b, float[] c)
    {
        if (a.Length != 2 || b.Length != 2 || c.Length != 2)
        {
            Debug.Log("calculate_angle_3_points array dimension is incorrect");
            return 0.0f;
        }

        float x_1 = a[0];
        float y_1 = 1f - a[1];

        float x_2 = b[0];
        float y_2 = 1f - b[1];

        float x_3 = c[0];
        float y_3 = 1f - c[1];

        float x_BA = x_1 - x_2;
        float y_BA = y_1 - y_2;

        float x_BC = x_3 - x_2;
        float y_BC = y_3 - y_2;

        float BA_BC = (x_BA * x_BC) + (y_BA * y_BC);

        float BA_norm = Mathf.Sqrt((x_BA * x_BA) + (y_BA * y_BA));
        float BC_norm = Mathf.Sqrt((x_BC * x_BC) + (y_BC * y_BC));

        float alpha = Mathf.Acos(BA_BC / (BA_norm * BC_norm));
        alpha = (alpha / Mathf.PI) * 180f;

        //Debug.Log("Angle(1-2-3): " + alpha);
        //Debug.Log("relative distance(1-2): " + BA_norm);
        //Debug.Log("relative distance(2-3): " + BC_norm);

        return alpha;
    }

    public float CalculateRelativeDistance2Points(float[] a, float[] b)
    {
        float x = a[0] - b[0];
        float y = a[1] - b[1];

        return Mathf.Sqrt(Mathf.Pow(x, 2) + Mathf.Pow(y, 2));
    }

    // for PC Mac Linux UNITY_EDITOR
    public List<string> ReadJson(string path, string key)
    {
        List<string> json_settings = new List<string>();
        using (System.IO.StreamReader file = System.IO.File.OpenText(path))
        {
            using (JsonTextReader reader = new JsonTextReader(file))
            {
                JObject jObject = (JObject)JToken.ReadFrom(reader);
                if (key == "poseName")
                {
                    json_settings.Add(jObject[key].ToString());
                    return json_settings;
                }
                else if (key == "poseDetectValueArray")
                {
                    JArray jArray = JArray.Parse(jObject[key].ToString());
                    foreach (var arr in jArray)
                    {
                        JObject jObj = JObject.Parse(arr.ToString());
                        json_settings.Add(jObj["type"].ToString());
                        json_settings.Add(jObj["points"].ToString());
                        json_settings.Add(jObj["value"].ToString());
                    }
                    if (json_settings.Count % 3 != 0)
                    {
                        Debug.Log("Json settings format error");
                    }
                    return json_settings;
                }
                else
                {
                    Debug.Log("unknown json key");
                    return json_settings;
                }
            }
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }
}
