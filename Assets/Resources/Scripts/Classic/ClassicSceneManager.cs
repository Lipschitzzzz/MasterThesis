using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ClassicSceneManager : MonoBehaviour
{
    public GameObject audioManager;
    // Start is called before the first frame update
    void Start()
    {
        audioManager = GameObject.Find("AudioManager");
        audioManager.GetComponent<AudioManager>().muteUnmuteButton = GameObject.Find("Mute");

        // double click the button. to correctly show the mute/unmute icon
        Mute();
        Mute();
    }

    // Update is called once per frame
    void Update()
    {

    }
    public void Mute()
    {
        audioManager.GetComponent<AudioManager>().Mute();
    }
}
