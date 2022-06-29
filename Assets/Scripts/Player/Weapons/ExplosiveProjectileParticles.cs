using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosiveProjectileParticles : MonoBehaviour {
    public float damage;

    public ParticleSystem attackParticles;

    private bool canDealDamage = true;

    private void OnParticleCollision(GameObject other) {
        if (other.tag == "Enemy" && canDealDamage) {
            other.GetComponent<EnemyHealth>().TakeDamage(damage);
            canDealDamage = false;
        }
    }
}
