using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoseConfigurations : MonoBehaviour
{
    public string poseName;
    public Dictionary<List<int>, string> angles;

    public Dictionary<List<int>, string> xCoordinateTolerance;
    public Dictionary<List<int>, string> yCoordinateTolerance;

    public PoseConfigurations()
    {
        poseName = "undefined";
        angles = new Dictionary<List<int>, string>();
        xCoordinateTolerance = new Dictionary<List<int>, string>();
        yCoordinateTolerance = new Dictionary<List<int>, string>();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Print()
    {
        Debug.Log("poseName: " + poseName);
        Debug.Log("xCoordinateTolerance: ");

        foreach (List<int> i in xCoordinateTolerance.Keys)
        {
            foreach (int j in i)
            {
                Debug.Log(j);
            }
            Debug.Log(xCoordinateTolerance[i]);
        }

        Debug.Log("yCoordinateTolerance: ");

        foreach (List<int> i in yCoordinateTolerance.Keys)
        {
            foreach (int j in i)
            {
                Debug.Log(j);
            }
            Debug.Log(yCoordinateTolerance[i]);
        }

        Debug.Log("angles: ");

        foreach (List<int> i in angles.Keys)
        {
            foreach (int j in i)
            {
                Debug.Log(j);
            }
            Debug.Log(angles[i]);
        }
    }



}
