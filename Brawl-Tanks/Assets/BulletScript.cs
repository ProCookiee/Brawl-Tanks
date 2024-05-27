using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletScript : MonoBehaviour
{
    private Rigidbody2D rb;
    int hits = 0;
    int maxHits = 10;

    public float creationTime;

    public ParticleSystem bulletExplosion;
    public AudioSource bulletBounceSound;

    private Camera mainCamera;
    private Renderer bulletRenderer;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.velocity = transform.up * 4f;
        bulletRenderer = GetComponent<Renderer>();
        mainCamera = Camera.main;
        if (name == "miniBullet")
        {
            maxHits = 3;
        }

        creationTime = Time.time;
    }

    // Update is called once per frame
    void Update()
    {
        if (!IsVisible())
        {
            Destroy(gameObject);
        }
        // Destroy the bullet if it goes off screen
    }

    private bool IsVisible()
    {
        Vector3 screenPoint = mainCamera.WorldToViewportPoint(transform.position);
        return screenPoint.z > 0 && screenPoint.x > 0 && screenPoint.x < 1 && screenPoint.y > 0 && screenPoint.y < 1;
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.tag == "Wall")
        {
            if (name == "fragment")
            {
                StartCoroutine(twoSecondTimer());
            }
            else
            {
                GameObject player;
                playerMovement playerMovement;

                if (name == "P1_Tank_bullet")
                {
                    player = GameObject.Find("P1_Tank");
                }
                else if (name == "P2_Tank_bullet")
                {
                    player = GameObject.Find("P2_Tank");
                }
                else
                {
                    player = null;
                }
                if (player == null)
                {
                    //playerMovement = player.GetComponent<playerMovement>();
                    if (Time.time - creationTime < 0.05f)
                    {
                        Debug.Log("Bullet destroyed before hitting anything");
                        Debug.Log(Time.time - creationTime);
                        Explode();
                        Destroy(gameObject);
                        //playerMovement.bulletCount--;
                    }
                    hits++;

                    if (hits >= maxHits)
                    {
                        Explode();

                        //playerMovement.bulletCount--;
                        Destroy(gameObject);
                    }
                    else
                    {
                        Explode();
                    }
                }
                else
                {
                    playerMovement = player.GetComponent<playerMovement>();
                    if (Time.time - creationTime < 0.05f)
                    {
                        Debug.Log("Bullet destroyed before hitting anything");
                        Debug.Log(Time.time - creationTime);
                        Explode();
                        Destroy(gameObject);
                        playerMovement.bulletCount--;
                    }
                    hits++;

                    if (hits >= maxHits)
                    {
                        Explode();

                        playerMovement.bulletCount--;
                        Destroy(gameObject);
                    }
                    else
                    {
                        Explode();
                    }
                }

            }

        }
    }

    void Explode()
    {
        bulletBounceSound.Play();
        ParticleSystem explosionEffect = Instantiate(bulletExplosion, transform.position, Quaternion.identity);
        explosionEffect.Play();
        Destroy(explosionEffect.gameObject, explosionEffect.main.duration);
    }

    IEnumerator twoSecondTimer()
    {
        StartCoroutine(FadeOutFragment(gameObject, 2f));  // Correct reference to gameObject
        yield return new WaitForSeconds(2);
        Destroy(gameObject);
    }

    IEnumerator FadeOutFragment(GameObject fragment, float fadeDuration)
    {
        SpriteRenderer renderer = fragment.GetComponent<SpriteRenderer>();
        if (renderer != null)
        {
            float startAlpha = renderer.color.a;
            for (float t = 0; t < 1; t += Time.deltaTime / fadeDuration)
            {
                if (renderer == null) break; // Check if renderer still exists
                Color newColor = renderer.color;
                newColor.a = Mathf.Lerp(startAlpha, 0, t);
                renderer.color = newColor;
                yield return null;
            }
            renderer.color = new Color(renderer.color.r, renderer.color.g, renderer.color.b, 0);
        }
    }

}
