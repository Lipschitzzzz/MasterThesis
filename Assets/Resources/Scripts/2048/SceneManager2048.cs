using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SceneManager2048 : MonoBehaviour
{
    public GameObject cameraTexture;
    public TextMeshProUGUI currentSongNameText;
    public GameObject audioManager;

    public void CameraTextureOnOff()
    {
        cameraTexture.SetActive(!cameraTexture.activeSelf);
        
    }

    // Start is called before the first frame update
    void Start()
    {
        currentSongNameText.text = audioManager.GetComponent<AudioManager>().GetCurrentSongName();

        audioManager = GameObject.Find("AudioManager");
        audioManager.GetComponent<AudioManager>().muteUnmuteButton = GameObject.Find("Mute");

        // double click the button. to correctly show the mute/unmute icon
        Mute();
        Mute();

    }

    // Update is called once per frame
    void Update()
    {
        currentSongNameText.text = audioManager.GetComponent<AudioManager>().GetCurrentSongName();

    }


    public void Mute()
    {
        audioManager.GetComponent<AudioManager>().Mute();
    }
}
