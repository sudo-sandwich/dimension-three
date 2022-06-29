using System.Collections.Generic;
using UnityEngine;

public class ExplosiveProjectile : ProjectileBase {
    public GameObject sprite;
    public ParticleSystem explosionParticles;

    public bool hasExploded = false;

    private void FixedUpdate() {
        if (!hasExploded) {
            lifetime -= Time.fixedDeltaTime;
            if (lifetime <= 0) {
                Explode();
            } else {
                rb2d.MovePosition((Vector2)transform.position + direction * Time.fixedDeltaTime);
            }
        } else if (!explosionParticles.isPlaying) {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        if (!hasExploded) {
            if (collision.tag == "Wall") {
                Explode();
            } else if (collision.tag == "Enemy") {
                Explode();
                collision.gameObject.GetComponent<EnemyHealth>().TakeDamage(damage);
            }
        }
    }

    private void Explode() {
        hasExploded = true;
        explosionParticles.Play();
        sprite.SetActive(false);
    }
}
