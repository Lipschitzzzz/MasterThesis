using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ClassicSceneManager : MonoBehaviour
{
    public GameObject audioManager;

    // -8.5 <= x <= 8.5
    // -2.0 <= y <= 5.0

    public List<GameObject> poseQuadObjects;

    public GameObject quad;
    public GameObject poseManager;
    public List<Sprite> poseTextures;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        collision.gameObject.GetComponent<Rigidbody2D>().gravityScale = 0.0f;
    }

    IEnumerator GeneratePoseQuad()
    {
        for (; ; )
        {
            float x = Random.Range(-8.5f, 8.5f);
            float y = Random.Range(-2.0f, 5.0f);

            GameObject tem = Instantiate(quad, new Vector3(x, y, 6.0f), Quaternion.identity, poseManager.transform);
            tem.transform.Rotate(new Vector3(0, 0, 31), Space.Self);
            int poseIndex = Random.Range(0, poseTextures.Count - 1);
            tem.GetComponent<SpriteRenderer>().sprite = poseTextures[poseIndex];
            yield return new WaitForSeconds(3.0f);
        }

    }

    // Start is called before the first frame update
    void Start()
    {
        audioManager = GameObject.Find("AudioManager");
        audioManager.GetComponent<AudioManager>().muteUnmuteButton = GameObject.Find("Mute");

        // double click the button. to correctly show the mute/unmute icon
        Mute();
        Mute();

        StartCoroutine(GeneratePoseQuad());
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
