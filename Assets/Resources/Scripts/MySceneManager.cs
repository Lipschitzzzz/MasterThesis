using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine;

public class MySceneManager : MonoBehaviour
{
    public GameObject audioManager;
    public GameObject cameraTexture;
    public TextMeshProUGUI fpsLabel;
    public TextMeshProUGUI currentSongNameText;
    float updateInterval = 0.4f;
    float accumulation = 0.0f;
    int frames = 0;
    string fpsString;

    public void Mute()
    {
        audioManager.GetComponent<AudioManager>().Mute();
    }

    public void NextSong()
    {
        audioManager.GetComponent<AudioManager>().NextSong();
    }

    public virtual void LoadScene(string sceneName)
    {
        accumulation = 0.0f;
        frames = 0;
        SceneManager.LoadScene(sceneName);
    }

    public void GameQuit()
    {
        Application.Quit();
    }

    public void CameraTextureOnOff()
    {
        cameraTexture.SetActive(!cameraTexture.activeSelf);

    }

    // Start is called before the first frame update
    protected void Start()
    {
        audioManager = GameObject.Find("AudioManager");
        audioManager.GetComponent<AudioManager>().muteUnmuteButton = GameObject.Find("Mute");
        currentSongNameText.text = audioManager.GetComponent<AudioManager>().GetCurrentSongName();

        // double click the button. to correctly show the mute/unmute icon
        Mute();
        Mute();

        StartCoroutine(RefreshFPSLabel());
    }

    private void UpdateFPS()
    {
        frames++;
        accumulation += Time.deltaTime;

        float fps = frames / accumulation;
        float milliSecond = accumulation * 1000 / frames;
        fpsString = string.Format("{0:0.0} ms / frame {1:0.0} fps", milliSecond, fps);

        frames = 0;
        accumulation = 0.0f;

    }

    public IEnumerator RefreshFPSLabel()
    {
        for(;;)
        {
            fpsLabel.text = fpsString;
            yield return new WaitForSeconds(0.5f);
        }

    }

    // Update is called once per frame
    protected void Update()
    {
        currentSongNameText.text = audioManager.GetComponent<AudioManager>().GetCurrentSongName();
        UpdateFPS();

    }
}
