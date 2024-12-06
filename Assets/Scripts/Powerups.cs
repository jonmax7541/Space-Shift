using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Powerups : MonoBehaviour
{
    public Player_Movement Player_Movement;
    public float moveSpeed = 1.0f;

    private void Start() => Player_Movement = GetComponent<Player_Movement>();

    private void FixedUpdate()
    {
        this.transform.Translate(moveSpeed * Time.deltaTime * -transform.forward);

        if (this.transform.position.z <= -4f)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            if (gameObject.CompareTag("Shield"))
            {
                Player_Movement.power = Player_Movement.PowerUp.Shielded;
            }
            else if (gameObject.CompareTag("Decelerator"))
            {
                Player_Movement.power = Player_Movement.PowerUp.Decelerated;
            }
            else if (gameObject.CompareTag("Supercharger"))
            {
                Player_Movement.power = Player_Movement.PowerUp.Charged;
            }
            else if (gameObject.CompareTag("Energy_Refill"))
            {
                other.GetComponent<Player_Movement>().AddEnergy();
            }
            Destroy(gameObject);
        }
    }
}
