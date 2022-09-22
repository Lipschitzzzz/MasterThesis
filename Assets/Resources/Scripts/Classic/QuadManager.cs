using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuadManager : MonoBehaviour
{
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.transform.parent.name == "Boundaries")
        {
            this.gameObject.GetComponent<Rigidbody2D>().gravityScale = 0.0f;
        }
    }

    private void DestroyDelay(float delay)
    {
        Destroy(this.gameObject, delay);
    }

    // Start is called before the first frame update
    void Start()
    {
        DestroyDelay(3.0f);
    }

    // Update is called once per frame
    void Update()
    {

    }
}
