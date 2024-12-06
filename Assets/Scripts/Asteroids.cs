using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;
using UnityEngine.UI;

public class Asteroids : MonoBehaviour
{
    public GameObject smallAsteroid;
    private float life = 3;
    public float moveSpeed = 1.0f;
    private int point;
    public GameObject player;

    private void Start()
    {
        player = GameObject.FindWithTag("Player");
    }

    private void FixedUpdate()
    {
        this.transform.Translate(moveSpeed * Time.deltaTime * -transform.forward);

        if (this.transform.position.z <= -4f)
        {
            Destroy(gameObject);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("BulletC"))
        {
            if (gameObject.CompareTag("Asteroid_Rock"))
                point = 200;
            else if (gameObject.CompareTag("Asteroid_Metal"))
                point = 500;
            else
                point = 100;

            Destroy(gameObject);
        }
        else if (collision.gameObject.CompareTag("BulletS"))
        {
            if (gameObject.CompareTag("Asteroid_Rock"))
            {
                Vector3 origin = this.transform.position;
                origin += Random.insideUnitSphere * 0.5f;

                point = 200;
                Destroy(gameObject);

                Instantiate(smallAsteroid, origin, this.transform.rotation);
                Instantiate(smallAsteroid, origin, this.transform.rotation);
            }
            else if (gameObject.CompareTag("Asteroid_Metal"))
            {
                life--;
                point = 0;
                if (life <= 0)
                {
                    point = 500;
                    Destroy(gameObject);
                }
            }
            else
            {
                point = 100;
                Destroy(gameObject);
            }
        }
        else if (collision.gameObject.CompareTag("Player"))
        {
            point = 0;
            Destroy(gameObject);
        }

        player.SendMessage("AddPoints", point);
    }
}
