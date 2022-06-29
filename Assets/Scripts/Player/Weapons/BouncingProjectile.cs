using System.Collections.Generic;
using UnityEngine;

public class BouncingProjectile : ProjectileBase {
    private void FixedUpdate() {
        lifetime -= Time.fixedDeltaTime;
        if (lifetime <= 0) {
            Destroy(gameObject);
        }
        rb2d.MovePosition((Vector2)transform.position + direction * Time.fixedDeltaTime);
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        if (collision.tag == "Wall") {
            RaycastHit2D hit = Physics2D.Raycast((Vector2)transform.position - direction * Time.fixedDeltaTime, direction, Mathf.Infinity, LayerMask.GetMask("Walls"));
            direction = Vector2.Reflect(direction, hit.normal);
        } else if (collision.tag == "Enemy") {
            Destroy(gameObject);
            collision.gameObject.GetComponent<EnemyHealth>().TakeDamage(damage);
        }
    }
}
