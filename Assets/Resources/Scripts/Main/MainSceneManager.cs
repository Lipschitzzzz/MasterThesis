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
        audioManager = GameObject.Find("AudioManager");
        audioManager.GetComponent<AudioManager>().muteUnmuteButton = GameObject.Find("Mute");
        canvas.SetActive(false);
        audioManager.SetActive(false);
        startTime = Time.time;

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

    public void Mute()
    {
        audioManager.GetComponent<AudioManager>().Mute();
    }

    public void ClassicStart()
    {
        SceneManager.LoadScene("Classic");
    }
    public void SpecialStart()
    {
        Debug.Log("The Special Scene is updating");
        // SceneManager.LoadScene("Special");
    }

    public void GameQuit()
    {
        Application.Quit();
    }

}
