using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class WeaponBase : MonoBehaviour {
    public float damage;
    public float damageMultiplier = 1;
    public float fireRate;
    public float fireRateMultiplier = 1;
    public float projectileSpeed;
    public float projectileSpeedMultiplier = 1;
    public float projectileLifetime;
    public float projectileLifetimeMultiplier = 1;
    public int numProjectiles = 1;
    public float numProjectilesMultiplier = 1;

    protected float cooldown = 0;

    protected virtual void FixedUpdate() {
        cooldown -= Time.fixedDeltaTime;
    }

    // fires the weapon regardless of any cooldown
    public abstract void FireWeapon(Vector2 crosshairLocation, GameObject projectilePrefab);

    // fires the weapon, respecting the weapon's cooldown
    public virtual void TriggerPressed(Vector2 crosshairLocation, GameObject projectilePrefab) {
        if (cooldown < 0) {
            FireWeapon(crosshairLocation, projectilePrefab);
            cooldown = 1 / (fireRate * fireRateMultiplier);
        }
    }

    // occurs when the trigger is released, most weapons won't do anything
    public virtual void TriggerReleased(Vector2 crosshairLocation, GameObject projectilePrefab) { }
}
