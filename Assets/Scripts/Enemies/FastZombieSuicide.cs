using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FastZombieSuicide : MonoBehaviour {
    public float damage;
    public EnemyMovementController movementScript;
    public GameObject Sprite;

    private ParticleSystem attackParticles;
    private bool hasSuicided;
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
        if (!hasSuicided && movementScript.inDesiredRange) {
            hasSuicided = true;
            Sprite.SetActive(false);
            attackParticles.Play();
        }

        if (hasSuicided && !attackParticles.isPlaying) {
            Destroy(transform.parent.gameObject);
        }
    }
}
