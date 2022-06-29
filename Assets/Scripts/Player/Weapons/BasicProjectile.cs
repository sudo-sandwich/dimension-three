using System.Collections.Generic;
using UnityEngine;

public class BasicProjectile : ProjectileBase {
    private void FixedUpdate() {
        lifetime -= Time.fixedDeltaTime;
        if (lifetime <= 0) {
            Destroy(gameObject);
        }
        rb2d.MovePosition((Vector2)transform.position + direction * Time.fixedDeltaTime);
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        if (collision.tag == "Wall") {
            Destroy(gameObject);
        } else if (collision.tag == "Enemy") {
            Destroy(gameObject);
            collision.gameObject.GetComponent<EnemyHealth>().TakeDamage(damage);
        }
    }
}
