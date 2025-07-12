using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed;
    public float destroyDelay;

    void Start()
    {
        Invoke("DestroyBullet", destroyDelay);
    }

    void Update()
    { 
        transform.Translate(Vector3.right * Time.deltaTime * speed);
    }

    private void DestroyBullet()
    {
        Destroy(gameObject);
    }
}
