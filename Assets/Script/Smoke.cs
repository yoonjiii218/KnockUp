using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Smoke : MonoBehaviour
{
    public float speed;
    public float knockForce;
    private bool isWait = false;
    public float waitTime;

    void Start()
    {

    }

    void Update()
    {
        if (!isWait)
        {
            transform.Translate(Vector3.right * Time.deltaTime * speed);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("bullet") || collision.CompareTag("bullet_k"))
        {
            Debug.Log("bullet");
            if (!isWait)
            {
                StartCoroutine(Wait());
            }
        }
        else if (collision.CompareTag("Stop"))
        {
            isWait = true;
        }
    }

    IEnumerator Wait()
    {
        isWait = true;
        yield return new WaitForSeconds(waitTime);
        isWait = false;
    }
}
