using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour {
    private PlayerControls playerControls;

    private Rigidbody2D rb2d;

    public GameObject Sprite;

    public WeaponBase currentWeapon;
    public ProjectileBase currentProjectile;

    public Vector2 currentCrosshairLocation;

    public float health = 100;
    public float speed;

    private void Awake() {
        playerControls = new PlayerControls();

        //rb2d = Player.GetComponent<Rigidbody2D>();
        rb2d = GetComponent<Rigidbody2D>();
    }

    private void OnEnable() {
        playerControls.Enable();
    }

    private void OnDisable() {
        playerControls.Disable();
    }

    private void FixedUpdate() {
        Vector2 move = playerControls.Player.Move.ReadValue<Vector2>();

        //rb2d.MovePosition((Vector2)transform.position + move * Time.deltaTime * speed);
        rb2d.velocity = move * speed;

        if (playerControls.Player.Fire.ReadValue<float>() == 1) {
            currentWeapon.TriggerPressed(currentCrosshairLocation, currentProjectile.gameObject);
        } else {
            currentWeapon.TriggerReleased(currentCrosshairLocation, currentProjectile.gameObject);
        }
    }

    public void TakeDamage(float damage) {
        health -= damage;
        Debug.Log($"took {damage} damage");
    }

    public void UpdateRotation(Vector3 pointToLookAt) {
        //Sprite.transform.rotation = Quaternion.Euler(0, 0, Vectors.Vector2ToDegrees(pointToLookAt - Player.transform.position) - 90);
        Sprite.transform.rotation = Quaternion.Euler(0, 0, Vectors.Vector2ToDegrees(pointToLookAt - transform.position) - 90);
    }
}
