using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ClassicTimeManager : TimeManager
{
    public GameObject[] gameObjects;

    private bool longPressStart = false;
    private float pressedTime = 0;
    private PlayerInfo playerInfo;
    
    protected IEnumerator UpdateCountdownBar()
    {
        while (true)
        {
            gameObjects[0].GetComponent<Image>().fillAmount = countdown / 60.0f; // countdown bar

            gameObjects[1].GetComponent<Text>().text = countdown.ToString("#0.0") + " s"; // countdown text
            countdown -= 0.1f;
            yield return new WaitForSeconds(0.1f);
        }

    }
    // Start is called before the first frame update
    void Start()
    {
        // playerInfo = GameObject.Find("Player Info").GetComponent<PlayerInfo>();
        countdown = 60.0f;
        StartCoroutine(UpdateCountdownBar());
    }

    // Update is called once per frame
    void Update()
    {
        if (countdown < 0.0f)
        {
            QuitClassic();
        }
        if (longPressStart)
        {
            countdown -= 0.5f * 0.05f * (Time.time - pressedTime);
        }
        // UpdateLevelBar();
    }

    private void UpdateLevelBar()
    {

        float level = Mathf.Floor(playerInfo.score / 100);
        float fillAmount = playerInfo.score % 100;

        gameObjects[2].GetComponent<Image>().fillAmount = fillAmount / 100.0f; // level bar

        gameObjects[3].GetComponent<Text>().text = level.ToString("#0"); // countdown text
    }


    public void QuitClassic()
    {
        SceneManager.LoadScene("Main");
    }

    public void AddTime()
    {
        countdown += 10.0f;
    }

    public void ReduceTime(bool start)
    {
        if (start)
        {
            longPressStart = true;
            pressedTime = Time.time;
        }
        else
        {
            longPressStart = false;
        }
    }
}
