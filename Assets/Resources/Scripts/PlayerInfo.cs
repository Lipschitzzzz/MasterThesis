using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInfo : MonoBehaviour
{
    public int level;

    public float score;


    private static PlayerInfo instance = null;

    public static PlayerInfo Instance
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

        else if (instance != this)
        {
            Destroy(gameObject);
        }

    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }



}
