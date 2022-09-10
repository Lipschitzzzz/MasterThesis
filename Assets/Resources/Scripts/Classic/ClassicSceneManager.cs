using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class ClassicSceneManager : MySceneManager
{
    // -8.5 <= x <= 8.5
    // -2.0 <= y <= 5.0

    public List<GameObject> poseQuadObjects;

    public GameObject quad;
    public GameObject poseManager;
    public List<Sprite> poseTextures;
    public GameObject moveNet;

    private List<GameObject> poseArmPrayerStretch = new List<GameObject>();
    private List<GameObject> poseLatissimusDorsiMuscleStretch = new List<GameObject>();
    private List<GameObject> poseUpperTrapStretchRight = new List<GameObject>();

    IEnumerator GeneratePoseQuad()
    {
        for (; ; )
        {
            float x = Random.Range(-800.0f, 300.0f);
            float y = Random.Range(-160.0f, 300.0f);

            GameObject tem = Instantiate(quad, new Vector3(-50f, 0f, 0.0f), Quaternion.identity, poseManager.transform);
            tem.transform.Rotate(new Vector3(0, 0, 31), Space.Self);
            int poseIndex = Random.Range(0, poseTextures.Count);
            tem.GetComponent<SpriteRenderer>().sprite = poseTextures[poseIndex];
            if (poseIndex == 0)
            {
                poseArmPrayerStretch.Add(tem);
            }
            else if (poseIndex == 1)
            {
                poseLatissimusDorsiMuscleStretch.Add(tem);
            }
            else if (poseIndex == 2)
            {
                poseUpperTrapStretchRight.Add(tem);
            }
            else
            {
                Debug.Log("Unknown Pose Index");
            }
            yield return new WaitForSeconds(3.0f);
        }

    }

    private IEnumerator MatchPose()
    {
        for(;;)
        {
            if (moveNet.GetComponent<ClassicMoveNet>().matched && poseArmPrayerStretch.Count > 0 && moveNet.GetComponent<ClassicMoveNet>().currentPoseIndex == 0)
            {
                Destroy(poseArmPrayerStretch[0]);
                poseArmPrayerStretch.RemoveAt(0);
            }
            if (moveNet.GetComponent<ClassicMoveNet>().matched && poseArmPrayerStretch.Count > 0 && moveNet.GetComponent<ClassicMoveNet>().currentPoseIndex == 1)
            {
                Destroy(poseLatissimusDorsiMuscleStretch[0]);
                poseLatissimusDorsiMuscleStretch.RemoveAt(0);
            }
            if (moveNet.GetComponent<ClassicMoveNet>().matched && poseArmPrayerStretch.Count > 0 && moveNet.GetComponent<ClassicMoveNet>().currentPoseIndex == 2)
            {
                Destroy(poseUpperTrapStretchRight[0]);
                poseUpperTrapStretchRight.RemoveAt(0);
            }

            yield return new WaitForSeconds(0.5f);

        }
    }

    // Start is called before the first frame update
    new void Start()
    {
        base.Start();

        audioManager = GameObject.Find("AudioManager");
        audioManager.GetComponent<AudioManager>().muteUnmuteButton = GameObject.Find("Mute");

        // double click the button. to correctly show the mute/unmute icon
        Mute();
        Mute();

        StartCoroutine(GeneratePoseQuad());
        StartCoroutine(MatchPose());
    }

    // Update is called once per frame
    new void Update()
    {
        base.Update();
        //if (Input.GetKeyDown("1") && poseArmPrayerStretch.Count > 0)
        //{
        //    Destroy(poseArmPrayerStretch[0]);
        //    poseArmPrayerStretch.RemoveAt(0);
        //}
        //if (Input.GetKeyDown("2") && poseLatissimusDorsiMuscleStretch.Count > 0)
        //{
        //    Destroy(poseLatissimusDorsiMuscleStretch[0]);
        //    poseLatissimusDorsiMuscleStretch.RemoveAt(0);
        //}
        //if (Input.GetKeyDown("3") && poseUpperTrapStretchRight.Count > 0)
        //{
        //    Destroy(poseUpperTrapStretchRight[0]);
        //    poseUpperTrapStretchRight.RemoveAt(0);
        //}

        
    }
}
