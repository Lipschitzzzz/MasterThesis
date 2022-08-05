using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
using UnityEngine.SceneManagement;

public class MainSceneManager : MonoBehaviour
{
    public VideoPlayer videoPlayer;
    public GameObject canvas;
    public GameObject audioManager;
    private float startTime;
    private bool isStart = false;

    // Start is called before the first frame update
    void Start()
    {
        canvas.SetActive(false);
        audioManager.SetActive(false);
        startTime = Time.time;

        videoPlayer.playOnAwake = false;

        videoPlayer.renderMode = UnityEngine.Video.VideoRenderMode.CameraNearPlane;

        videoPlayer.targetCameraAlpha = 0.8F;

        videoPlayer.url = "Assets/Resources/Videos/loading.mp4";

        videoPlayer.frame = 100;

        videoPlayer.isLooping = false;

        videoPlayer.Play();
    }

    IEnumerator isDone()
    {
        if (!videoPlayer.isPlaying)
        {
            canvas.SetActive(true);
            audioManager.SetActive(true);
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

    public void ClassicStart()
    {
        SceneManager.LoadScene("Classic");
    }
    public void SpecialStart()
    {
        SceneManager.LoadScene("Special");
    }

    public void GameQuit()
    {
        Application.Quit();
    }

}
