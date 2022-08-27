using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInfo : MonoBehaviour
{
    public int level;

    public float score;

    public float totalTime;

    private static PlayerInfo instance = null;

    public static PlayerInfo Instance
    {
        get { return instance; }
    }
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this);
        }

        else if (instance != this)
        {
            Destroy(gameObject);
        }

    }
    IEnumerator TotalTime()
    {
        while (true)
        {
            totalTime += 0.1f;
            yield return new WaitForSeconds(0.1f);
        }

    }

    // Start is called before the first frame update
    void Start()
    {
        totalTime = 0.0f;
        StartCoroutine(TotalTime());
    }

    // Update is called once per frame
    void Update()
    {

    }



}
