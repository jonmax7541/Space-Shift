using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Interactions;
using UnityEngine.UI;

public class Player_Movement : MonoBehaviour
{
    [Header("Movement")]
    public float baseSpeed = 1.0f;
    public float speedFactor = 0.25f;
    public float moveSpeed;

    [Header("Shooting")]
    public GameObject standardBulletPrefab;
    public GameObject chargedBulletPrefab;
    public Transform bulletOrigin;
    public float shootForce = 50;
    public int maxEnergy = 30;
    public int standardBulletCost = 1;
    public int chargedBulletCost = 3;
    public int remainingEnergy;

    private GameObject bullet;
    private GameObject stdbullet;
    private int cBulletCost;
    private int bulletCost;

    public Camera fpsCam;

    [Header("Damage")]
    public int HitDamage = 10;
    public int damageDealt;

    [Header("Points")]
    public int score;

    [Header("GUI")]
    public Color normalColor;
    public Color deceleratedColor;
    public Color shieldedColor;
    public Color chargedColor;
    public Button btnShoot;
    public TMP_Text energyCounter;
    public TMP_Text scoreCounter;
    public TMP_Text finalScoreCounter;
    public Image energyBar;
    public GameObject GameDisplay;
    public GameObject GameOverMenu;

    [Header("Powerup")]
    public int energyCost = 5;
    public int powerupDuration = 10;
    static public PowerUp power;
    public enum PowerUp
    {
        Normal,
        Decelerated,
        Shielded,
        Charged
    }

    private PlayerInput playerInput;
    private Vector3 direction;
    private bool normalSetting = false;

    // Start is called before the first frame update
    void Start()
    {
        playerInput = GetComponent<PlayerInput>();
        power = PowerUp.Normal;
        cBulletCost = chargedBulletCost;
        remainingEnergy = maxEnergy;
        GameDisplay.SetActive(true);
        GameOverMenu.SetActive(false);
        score = 0;
    }

    private void Update()
    {
        Vector2 input = playerInput.actions["Move"].ReadValue<Vector2>();
        direction = (Vector3)input;

        playerInput.actions["Shoot"].performed += context =>
        {
            if (context.interaction is TapInteraction)
            {
                bullet = stdbullet;
                bulletCost = standardBulletCost;
                Shooting();
            }
            else if (context.interaction is HoldInteraction)
            {
                bullet = chargedBulletPrefab;
                bulletCost = chargedBulletCost;
                Shooting();
            }
        };
        PowerUpHandler();
        EnergyUpdate();
    }

    private void FixedUpdate()
    {
        this.transform.Translate(moveSpeed * Time.deltaTime * direction);
    }

    private void Shooting()
    {
        if (remainingEnergy >= bulletCost)
        {
            Vector3 targetPoint;
            if (Physics.Raycast(fpsCam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0.5f)), out RaycastHit hit))
            {
                targetPoint = hit.point;
            }
            else
            {
                targetPoint = fpsCam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0.5f)).GetPoint(75);
            }

            Vector3 bulletDirection = targetPoint - bulletOrigin.position;

            GameObject currentBullet = Instantiate(bullet, bulletOrigin.position, Quaternion.identity);
            currentBullet.transform.forward = bulletDirection.normalized;

            currentBullet.GetComponent<Rigidbody>().AddForce(bulletDirection.normalized * shootForce, ForceMode.Impulse);

            remainingEnergy -= bulletCost;
        }
    }

    private void EnergyUpdate()
    {
        if (remainingEnergy > maxEnergy)
        {
            remainingEnergy = maxEnergy;
        }
        else if (remainingEnergy < 0)
        {
            remainingEnergy = 0;
        }

        energyBar.fillAmount = (float)remainingEnergy / (float)maxEnergy;
        energyCounter.text = remainingEnergy.ToString();
    }

    private void PowerUpHandler()
    {
        switch (power)
        {
            // Effects of all power ups are removed.
            case PowerUp.Normal:
                moveSpeed = baseSpeed;
                damageDealt = HitDamage;
                chargedBulletCost = cBulletCost;
                stdbullet = standardBulletPrefab;
                btnShoot.image.color = normalColor;
                break;

            // Player slows down a certain ammount
            case PowerUp.Decelerated:
                moveSpeed = baseSpeed / speedFactor;
                btnShoot.image.color = deceleratedColor;
                if (!normalSetting)
                    StartCoroutine(SetNormal());
                break;

            // Player is invincible for a limited time
            case PowerUp.Shielded:
                damageDealt = 0;
                btnShoot.image.color = shieldedColor;
                if (!normalSetting)
                    StartCoroutine(SetNormal());
                break;

            // Player shoots charged bullets only
            case PowerUp.Charged:
                stdbullet = chargedBulletPrefab;
                chargedBulletCost = standardBulletCost;
                btnShoot.image.color = chargedColor;
                if (!normalSetting)
                    StartCoroutine(SetNormal());
                break;
        }
    }

    IEnumerator SetNormal()
    {
        normalSetting = true;
        AddPoints(200);
        yield return new WaitForSeconds(powerupDuration);
        power = PowerUp.Normal;
        normalSetting = false;
    }

    public void AddEnergy()
    {
        remainingEnergy += energyCost;
        AddPoints(100);
    }

    public void AddPoints(int point)
    {
        score = int.Parse(scoreCounter.text);
        score += point;
        scoreCounter.text = score.ToString();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Asteroid_Rock") || collision.gameObject.CompareTag("Asteroid_Metal") || collision.gameObject.CompareTag("Asteroid_RockS") || collision.gameObject.CompareTag("Asteroid_Ice"))
        {
            if (remainingEnergy == 0)
            {
                Time.timeScale = 0f;
                GameDisplay.SetActive(false);
                GameOverMenu.SetActive(true);
                finalScoreCounter.text = scoreCounter.text;
            }
            remainingEnergy -= damageDealt;
        }
    }
}
