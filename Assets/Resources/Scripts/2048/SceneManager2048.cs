using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SceneManager2048 : MySceneManager
{
    public GameObject instruction;

    // Start is called before the first frame update
    new void Start()
    {
        base.Start();
    }

    // Update is called once per frame
    new void Update()
    {
        base.Update();
    }

    public void EnableInstruction()
    {
        instruction.SetActive(!instruction.gameObject.activeSelf);
    }

}
