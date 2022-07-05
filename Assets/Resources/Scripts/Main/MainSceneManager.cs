using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainSceneManager : MonoBehaviour
{

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
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
