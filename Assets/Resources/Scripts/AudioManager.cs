using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public AudioClip[] audioClips;

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

}
