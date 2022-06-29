using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ProjectileBase : MonoBehaviour {
    public float damage;
    public Vector2 direction;
    public float lifetime;

    protected Rigidbody2D rb2d;

    protected virtual void Awake() {
        rb2d = GetComponent<Rigidbody2D>();
    }
}
