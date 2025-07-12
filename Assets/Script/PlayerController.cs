using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    public GameObject bullet;
    public GameObject bullet_k;
    public GameObject smoke;
    private Rigidbody2D playerRb;
    private SpriteRenderer playerSprite;
    public RawImage[] bulletUI;
    public GameObject gameOverWindow;

    private int playerHp = 3;
    public TextMeshProUGUI hp;

    public GameObject endingDoorOpen;
    public RawImage endingFade;

    public Camera c;

    //이동
    public float horizentalInput;
    public float speed;
    public int direction = 1;

    //점프
    public float jumpForce;
    public float gravityModifier;
    public int jumpMax;
    private int jumpCnt;

    //공격
    private bool isShoot;
    private bool isWaiting;
    public float WaitMaxTime;
    private float WaitTimer;
    public float shootDlay = 0.5f;
    public int bulletMax;
    private int bulletCnt;
    private Quaternion bulletQnion;

    //넉백
    public float knockForce;
    public bool isKnock;
    public bool isDown;

    public Transform smokePos;
    public Transform smokePos_op;

    public bool isEndingOrGameOver;

    public float fadeRate;
    public float fadeBeforeDelay;
    public float fadeDelay;

    private bool isAttack;

    void Start()
    {
        playerRb = GetComponent<Rigidbody2D>();
        playerSprite = GetComponent<SpriteRenderer>();
        Physics2D.gravity *= gravityModifier;
        jumpCnt = jumpMax;
        bulletCnt = bulletMax;

        hp.text = "HP: " + playerHp;
    }

    void Update()
    {
        horizentalInput = Input.GetAxis("Horizontal");

        if (!isEndingOrGameOver)
        {
            if (!isKnock)
            {
                transform.Translate(Vector3.right * speed * Time.deltaTime * horizentalInput * direction);
            }

            if (horizentalInput > 0)
            {
                direction = 1;
                transform.eulerAngles = new Vector3(0, 0, 0);
            }
            else
            {
                direction = -1;
                transform.eulerAngles = new Vector3(0, 180, 0);
            }

            bulletQnion = transform.rotation;

            if (Input.GetKeyDown(KeyCode.Space) && jumpCnt > 0)
            {
                jumpCnt--;
                playerRb.velocity = new Vector2(playerRb.velocity.x, 0);
                playerRb.AddForce(Vector3.up * jumpForce, ForceMode2D.Impulse);
            }

            if(isWaiting)
            {
                WaitTimer -= Time.deltaTime;

                if (Input.GetKey(KeyCode.DownArrow))
                {
                    isDown = true;
                    bulletQnion = Quaternion.Euler(transform.eulerAngles.x, transform.eulerAngles.y, -90);
                }

                if (WaitTimer <= 0f)
                {
                    isShoot = true;
                    isWaiting = false;
                    StartCoroutine(Shoot(bulletCnt, bulletQnion));
                }
            }

            if (Input.GetKeyDown(KeyCode.X) && !isShoot)
            {
                isWaiting = true;
                WaitTimer = WaitMaxTime;
            }
        }
    }

    IEnumerator Shoot(int bulletCount, Quaternion bulletRotation)
    {
        bulletCnt--;
        Color bulletColor = bulletUI[bulletCnt].color;
        bulletColor.a = 0.3f;
        bulletUI[bulletCnt].color = bulletColor;

        if(bulletCnt == 0)
        {
            //StartCoroutine(ZoomIn());
            bulletCnt = bulletMax;
            for(int i = 0; i < bulletUI.Length; i++)
            {
                bulletColor = bulletUI[i].color;
                bulletColor.a = 1.0f;
                bulletUI[i].color = bulletColor;
            }
            Instantiate(bullet_k, transform.position, bulletRotation);
            isKnock = true;
            StartCoroutine(Knock());
        }
        else
        {
            Instantiate(bullet, transform.position, bulletRotation);
        }

        yield return new WaitForSeconds(shootDlay);
        isShoot = false;
        isDown = false;
    }

    IEnumerator Knock()
    {
        if (isDown)
        {
            playerRb.AddForce(Vector3.up * knockForce, ForceMode2D.Impulse);
        }
        else
        {
            playerRb.AddForce(Vector3.right * knockForce * -direction, ForceMode2D.Impulse);
        }
        yield return new WaitForSeconds(0.5f);
        isKnock = false;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.CompareTag("ground"))
        {
            jumpCnt = jumpMax;
        }
        else if (collision.gameObject.CompareTag("smokeStart"))
        {
            Destroy(collision.gameObject);
            if(horizentalInput > 0)
            {
                Instantiate(smoke, smokePos.position, Quaternion.identity);
            }
            else
            {
                Instantiate(smoke, smokePos_op.position, Quaternion.identity);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("gameover"))
        {
            GameOver();
        }
        else if (collision.CompareTag("obstacle"))
        {
            if (!isAttack)
            {
                playerHp--;
                StartCoroutine(Attacked());
            }
            if (playerHp == 0)
            {
                GameOver();
            }
            hp.text = "HP: " + playerHp;
        }
        else if (collision.gameObject.CompareTag("smoke"))
        {
            GameOver();
        }
        else if (collision.gameObject.CompareTag("ending"))
        {
            StartCoroutine(Ending());
        }
    }

    void GameOver()
    {
        isEndingOrGameOver = true;
        gameOverWindow.SetActive(true);
    }

    IEnumerator Attacked()
    {
        isAttack = true;
        Color tmp = playerSprite.color;
        for(int i = 0; i < 3; i++)
        {
            tmp.a = 0.5f;
            playerSprite.color = tmp;
            yield return new WaitForSeconds(0.2f);
            tmp.a = 1.0f;
            playerSprite.color = tmp;
            yield return new WaitForSeconds(0.3f);
        }
        isAttack = false;
    }

    IEnumerator ZoomIn()
    {
        c.orthographicSize = 3.0f;
        yield return new WaitForSeconds(0.3f);
        c.orthographicSize = 5.0f;
    }

    IEnumerator Ending()
    {
        isEndingOrGameOver = true;
        endingDoorOpen.SetActive(true);
        yield return new WaitForSeconds(fadeBeforeDelay);
        Color tmp = endingFade.color;
        while(tmp.a < 1)
        {
            Debug.Log("Fade");
            tmp = endingFade.color;
            tmp.a += fadeRate;
            endingFade.color = tmp;
            yield return new WaitForSeconds(fadeDelay);
        }
        SceneManager.LoadScene("Title");
    }
}
