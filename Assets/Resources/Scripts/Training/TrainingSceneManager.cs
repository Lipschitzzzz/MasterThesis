using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class TrainingSceneManager : MySceneManager
{
    public GameObject[] gameObjects;

    private float countdown;
    private bool longPressStart = false;
    private float pressedTime = 0;
    private PlayerInfo playerInfo;

    protected IEnumerator UpdateCountdownBar()
    {
        while (true)
        {
            gameObjects[0].GetComponent<Image>().fillAmount = countdown / 300.0f; // countdown bar

            gameObjects[1].GetComponent<Text>().text = countdown.ToString("#0.0") + " s"; // countdown text
            countdown -= 0.1f;
            yield return new WaitForSeconds(0.1f);
        }

    }
    // Start is called before the first frame update
    new void Start()
    {
        base.Start();
        playerInfo = GameObject.Find("PlayerInfo").GetComponent<PlayerInfo>();
        countdown = 300.0f;
        StartCoroutine(UpdateCountdownBar());

    }

    // Update is called once per frame
    new void Update()
    {
        base.Update();
        if (countdown < 0.0f)
        {
            QuitTraining();
        }
        if (longPressStart)
        {
            countdown -= 0.1f * (Time.time - pressedTime);
        }
        UpdateLevelBar();
    }

    private void UpdateLevelBar()
    {

        float level = Mathf.Floor(playerInfo.score / 100);
        float fillAmount = playerInfo.score % 100;

        gameObjects[2].GetComponent<Image>().fillAmount = fillAmount / 100.0f; // level bar

        gameObjects[3].GetComponent<Text>().text = level.ToString("#0"); // countdown text
    }


    public void QuitTraining()
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
