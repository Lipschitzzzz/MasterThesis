using System.Collections;
using System.Collections.Generic;
using UnityEngine.Video;
using UnityEngine.UI;
using UnityEngine;

public class Loading : MonoBehaviour
{
    public VideoPlayer videoPlayer;
    public GameObject canvas;
    private float startTime;
    private bool isStart = false;

    // Start is called before the first frame update
    void Start()
    {
        startTime = Time.time;

        videoPlayer.playOnAwake = false;

        videoPlayer.renderMode = UnityEngine.Video.VideoRenderMode.CameraNearPlane;

        videoPlayer.targetCameraAlpha = 0.8F;

        //videoPlayer.url = "Assets/Resources/Videos/loading.mp4";
        videoPlayer.url = Application.streamingAssetsPath + "/" + "loading.mp4";

        videoPlayer.frame = 100;

        videoPlayer.isLooping = false;

        videoPlayer.Play();
    }

    IEnumerator isDone()
    {
        if (!videoPlayer.isPlaying)
        {
            canvas.SetActive(true);
            yield return null;
        }
    }
    // Update is called once per frame
    void Update()
    {
        if (!isStart && Time.time - startTime > 2.0f)
        {
            StartCoroutine(isDone());
        }

    }
}
