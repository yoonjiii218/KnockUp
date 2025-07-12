using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleMove : MonoBehaviour
{
    private PlayerController playerScript;
    private SpriteRenderer obstacleSprite; 
    public int hp;
    public float speed;
    public int direction;
    private bool isAttacked;

    void Start()
    {
        playerScript = GameObject.Find("Player").GetComponent<PlayerController>();
        obstacleSprite = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        if (!playerScript.isEndingOrGameOver)
        {
            if (direction > 0)
            {
                transform.Translate(Vector3.right * Time.deltaTime * speed);
            }
            else
            {
                transform.Translate(Vector3.left * Time.deltaTime * speed);
            }

            if (hp == 0)
            {
                DestroyObstacle();
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("shift"))
        {
            direction *= -1;
        }
        else if (collision.CompareTag("bullet"))
        {
            StartCoroutine(Attacked());
            Destroy(collision.gameObject);
        }
        else if (collision.CompareTag("bullet_k"))
        {
            Destroy(collision.gameObject);
            DestroyObstacle();
        }
    }

    private void DestroyObstacle()
    {
        Destroy(gameObject);
    }

    IEnumerator Attacked()
    {
        hp--;
        Color tmp = obstacleSprite.color;
        tmp.a = 0.5f;
        obstacleSprite.color = tmp;
        yield return new WaitForSeconds(0.3f);
        tmp.a = 1.0f;
        obstacleSprite.color = tmp;
        yield return new WaitForSeconds(0.3f);
    }
}
