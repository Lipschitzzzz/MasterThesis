using System.Collections;
using System.Collections.Generic;
using UnityEngine.Video;
using UnityEngine;

public class VideoManager : MonoBehaviour
{
    private static VideoManager instance = null;

    public static VideoManager Instance
    {
        get { return Instance; }
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

    // Start is called before the first frame update
    void Start()
    {
        string filePath = Application.streamingAssetsPath + "/loading.mp4";
#if UNITY_EDITOR
        filePath = Application.streamingAssetsPath + "/loading.mp4";
#elif UNITY_ANDROID
        filePath = "jar:file://" + Application.dataPath + "!/assets/loading.mp4";
#else
        filePath = Application.streamingAssetsPath + "/loading.mp4";
#endif
        this.GetComponent<VideoPlayer>().url = filePath;
        this.GetComponent<VideoPlayer>().Play();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
