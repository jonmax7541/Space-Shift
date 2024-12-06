using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Spawning : MonoBehaviour
{
    [Header("Asteroids")]
    public GameObject smallAsteroid;
    public GameObject rockAsteroid;
    public GameObject iceAsteroid;
    public GameObject metalAsteroid;

    public float asteroidSpawnRate = 3f;
    public int asteroidSpawnCount = 1;
    private GameObject asteroid;

    [Header("Powerups")]
    public GameObject shield;
    public GameObject decelerator;
    public GameObject supercharger;

    public float powerupSpawnRate = 25f;
    private GameObject powerup;

    [Header("Energy")]
    public GameObject energyRefill;
    public float energySpawnRate = 3f;
    public int energySpawnCount = 1;

    [Header("Spawning")]
    public float spawnRange = 30f;
    private int level = 0;
    private int setLevel = 0;
    public Player_Movement Player_Movement;

    // Start is called before the first frame update
    void Start()
    {
        InvokeRepeating(nameof(SpawnAsteroid), 2.0f, asteroidSpawnRate);
        InvokeRepeating(nameof(SpawnPowerup), 40.0f, powerupSpawnRate);
        InvokeRepeating(nameof(SpawnEnergy), 5.0f, powerupSpawnRate);
        Player_Movement = GetComponent<Player_Movement>();
    }

    private void Update()
    {
        if (Player_Movement.score >= 2000 && Player_Movement.score <= 4999)
            setLevel = 1;
        else if (Player_Movement.score >= 5000 && Player_Movement.score <= 9999)
            setLevel = 2;
    }

    private void SpawnAsteroid()
    {
        for (int i = 0; i < asteroidSpawnCount; i++)
        {
            int asteroidType = UnityEngine.Random.Range(0, 10);
            if (Enumerable.Range(0, 3).Contains(asteroidType))
                asteroid = smallAsteroid;
            else if (Enumerable.Range(4, 7).Contains(asteroidType))
                asteroid = iceAsteroid;
            else if (Enumerable.Range(8, 9).Contains(asteroidType))
                asteroid = rockAsteroid;
            else
                asteroid = metalAsteroid;

            Vector3 playerPos = this.transform.position;
            Vector3 spawnPosition = new(playerPos.x + UnityEngine.Random.Range(-10, 10), playerPos.y + UnityEngine.Random.Range(-10, 10), playerPos.z + spawnRange);

            Instantiate(asteroid, spawnPosition, Quaternion.identity);
            IncreaseDifficulty(setLevel);
        }
    }

    private void SpawnPowerup()
    {
        int powerupType = UnityEngine.Random.Range(0, 10);
        if (Enumerable.Range(0, 3).Contains(powerupType))
        {
            powerup = shield;
        }
        else if (Enumerable.Range(4, 7).Contains(powerupType))
        {
            powerup = supercharger;
        }
        else
        {
            powerup = decelerator;
        }

        Vector3 playerPos = this.transform.position;
        Vector3 spawnPosition = new(playerPos.x + UnityEngine.Random.Range(-10, 10), playerPos.y + UnityEngine.Random.Range(-10, 10), playerPos.z + spawnRange);

        Instantiate(powerup, spawnPosition, Quaternion.identity);
    }

    private void SpawnEnergy()
    {
        Vector3 playerPos = this.transform.position;
        Vector3 spawnPosition = new(playerPos.x + UnityEngine.Random.Range(-10, 10), playerPos.y + UnityEngine.Random.Range(-10, 10), playerPos.z + spawnRange);

        Instantiate(energyRefill, spawnPosition, Quaternion.identity);
    }

    private void IncreaseDifficulty(int levelIncrease)
    {
        if (levelIncrease > level)
        {
            asteroidSpawnCount *= 4;
            asteroid.GetComponent<Asteroids>().moveSpeed++;
        }
    }
}
