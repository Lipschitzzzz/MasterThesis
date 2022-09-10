using System.Collections;
using System.Collections.Generic;
using UnityEngine.Video;
using UnityEngine.Android;
using UnityEngine.SceneManagement;
using UnityEngine;

public class MainSceneManager : MySceneManager
{
    public GameObject videoPlayer;
    public GameObject canvas;
    public GameObject playerInfo;

    // Start is called before the first frame update
    new void Start()
    {
        //audioManager = GameObject.Find("AudioManager");
        videoPlayer = GameObject.Find("VideoPlayer");
        playerInfo = GameObject.Find("PlayerInfo");
        //audioManager.GetComponent<AudioManager>().muteUnmuteButton = GameObject.Find("Mute");

        base.Start();

        // the first time enter game
        if (playerInfo.GetComponent<PlayerInfo>().totalTime < 2.0f)
        {
            canvas.SetActive(false);
            audioManager.SetActive(false);
        }
        // other scenes are loading to main scene
        else
        {
            Mute();
            Mute();
        }
        

        if (Permission.HasUserAuthorizedPermission(Permission.Camera))
        {
            // The user authorized use of the microphone.
        }
        else
        {
            bool useCallbacks = false;
            if (!useCallbacks)
            {
                // We do not have permission to use the microphone.
                // Ask for permission or proceed without the functionality enabled.
                Permission.RequestUserPermission(Permission.Camera);
            }
            else
            {
                PermissionCallbacks callbacks = new PermissionCallbacks();
                callbacks.PermissionDenied += PermissionCallbacks_PermissionDenied;
                callbacks.PermissionGranted += PermissionCallbacks_PermissionGranted;
                callbacks.PermissionDeniedAndDontAskAgain += PermissionCallbacks_PermissionDeniedAndDontAskAgain;
                Permission.RequestUserPermission(Permission.Microphone, callbacks);
            }
        }
    }

    // only play at the first time loading main scene. After the playing this object will be SetActive(false) so that it can not
    // play anymore when we load it from other scenes.
    private void PlayVideoOnlyOnce()
    {
        // other scenes are loading to main scene
        if (videoPlayer == null)
        {
            return;
        }
        // the first time enter game
        else if (playerInfo.GetComponent<PlayerInfo>().totalTime > 2.0f && !videoPlayer.GetComponent<VideoPlayer>().isPlaying)
        {
            canvas.SetActive(true);
            audioManager.SetActive(true);
            videoPlayer.gameObject.SetActive(false);
        }

    }
    
    // Update is called once per frame
    new void Update()
    {
        base.Update();
        PlayVideoOnlyOnce();
    }

    internal void PermissionCallbacks_PermissionDeniedAndDontAskAgain(string permissionName)
    {
        Debug.Log($"{permissionName} PermissionDeniedAndDontAskAgain");
    }

    internal void PermissionCallbacks_PermissionGranted(string permissionName)
    {
        Debug.Log($"{permissionName} PermissionCallbacks_PermissionGranted");
    }

    internal void PermissionCallbacks_PermissionDenied(string permissionName)
    {
        Debug.Log($"{permissionName} PermissionCallbacks_PermissionDenied");
    }


}
