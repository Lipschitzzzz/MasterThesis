using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine;

public class MySceneManager : MonoBehaviour
{
    public GameObject audioManager;
    public GameObject cameraTexture;
    public TextMeshProUGUI currentSongNameText;
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

    }

    // Update is called once per frame
    protected void Update()
    {
        currentSongNameText.text = audioManager.GetComponent<AudioManager>().GetCurrentSongName();

    }
}
