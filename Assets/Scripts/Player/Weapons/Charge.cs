using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Charge : WeaponBase {
    public float chargeTime;

    private int currentCharge = 0;
    private float currentChargeTime = 0;
    private float currentReleaseTime = 0;

    public override void FireWeapon(Vector2 crosshairLocation, GameObject projectilePrefab) {
        GameObject projectile = Instantiate(projectilePrefab, transform.position, Quaternion.identity, Globals.projectiles);
        projectile.SetActive(true);
        Vector2 direction = crosshairLocation - (Vector2)transform.position;
        direction.Normalize();
        ProjectileBase projectileScript = projectile.GetComponent<ProjectileBase>();
        projectileScript.direction = direction * projectileSpeed * projectileSpeedMultiplier;
        projectileScript.damage = damage * damageMultiplier;
        projectileScript.lifetime = projectileLifetime * projectileLifetimeMultiplier;
    }

    public override void TriggerPressed(Vector2 crosshairLocation, GameObject projectilePrefab) {
        if (currentCharge < numProjectiles && currentChargeTime < 0) {
            Debug.Log("added charge");
            currentCharge++;
            currentChargeTime = chargeTime;
        }
        currentChargeTime -= Time.fixedDeltaTime;
    }

    public override void TriggerReleased(Vector2 crosshairLocation, GameObject projectilePrefab) {
        if (currentCharge > 0 && currentReleaseTime < 0) {
            Debug.Log("released charge");
            FireWeapon(crosshairLocation, projectilePrefab);
            currentCharge--;
            currentReleaseTime = fireRate;
        }
        currentReleaseTime -= Time.fixedDeltaTime;
    }
}
