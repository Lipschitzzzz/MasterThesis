using System.Collections;
using System.Collections.Generic;
using UnityEngine.Sprites;
using UnityEngine.Video;
using UnityEngine.UI;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public AudioClip[] audioClips;
    public Sprite[] muteUnmuteSprites;
    public GameObject muteUnmuteButton;

    public bool isMuted;

    private int currentPlayingIndex;

    private bool isPlaying;

    private AudioSource audioSource;

    private static AudioManager instance = null;

    public static AudioManager Instance
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

        else if(instance != this)
        {
            Destroy(gameObject);
        }

    }

    // Start is called before the first frame update
    void Start()
    {
        audioSource = this.GetComponent<AudioSource>();
        isPlaying = true;
        currentPlayingIndex = 0;

    }

    // Update is called once per frame
    void Update()
    {
        if (isPlaying)
        {
            PlayAudio();
            isPlaying = false;
        }
        if (!audioSource.isPlaying)
        {
            currentPlayingIndex += 1;

            // Complete playing all bgms, return to the first
            if (currentPlayingIndex >= audioClips.Length)
            {
                currentPlayingIndex = 0;
            }
            isPlaying = true;

        }

    }

    private void PlayAudio()
    {
        audioSource.clip = audioClips[currentPlayingIndex];
        audioSource.Play();
    }

    public void Mute()
    {
        if (audioSource.mute)
        {
            muteUnmuteButton.GetComponent<Image>().sprite = muteUnmuteSprites[1];
            audioSource.mute = !audioSource.mute;
            isMuted = true;
        }
        else
        {
            muteUnmuteButton.GetComponent<Image>().sprite = muteUnmuteSprites[0];
            audioSource.mute = !audioSource.mute;
            isMuted = false;
        }
    }

    public string GetCurrentSongName()
    {
        return audioClips[currentPlayingIndex].name;
    }

}
