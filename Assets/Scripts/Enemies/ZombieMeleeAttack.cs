using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombieMeleeAttack : MonoBehaviour {
    public float damage;
    public float attackCooldown;
    public EnemyMovementController movementScript;

    private ParticleSystem attackParticles;

    private float attackTimer = 0;
    private bool canDealDamage = true;

    private void Awake() {
        attackParticles = GetComponent<ParticleSystem>();
    }

    private void OnParticleCollision(GameObject other) {
        if (other.tag == "Player" && canDealDamage) {
            other.GetComponent<PlayerController>().TakeDamage(damage);
            canDealDamage = false;
        }
    }

    private void FixedUpdate() {
        if (attackTimer < 0 && movementScript.inDesiredRange) {
            attackTimer = attackCooldown;
            attackParticles.Play();
            canDealDamage = true;
        }

        attackTimer -= Time.fixedDeltaTime;
    }
}
