using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHealth : MonoBehaviour {
    public float health;

    public void TakeDamage(float amount) {
        Debug.Log("enemy took damage");
        health -= amount;
        if (health <=0) {
            Destroy(gameObject);
        }
    }

    /*
    private void OnTriggerEnter2D(Collider2D other) {
        if (other.tag == "Player Projectile") {
            health -= other.GetComponent<ProjectileBase>().damage;
            Destroy(other.gameObject);

            if (health <= 0) {
                Destroy(gameObject);
            }
        }
    }
    */
}
