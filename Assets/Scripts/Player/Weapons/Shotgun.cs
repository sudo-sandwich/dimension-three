using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shotgun : WeaponBase {
    public override void FireWeapon(Vector2 crosshairLocation, GameObject projectilePrefab) {
        for (int i = 0; i < numProjectiles; i++) {
            GameObject projectile = Instantiate(projectilePrefab, transform.position, Quaternion.identity, Globals.projectiles);
            projectile.SetActive(true);
            Vector2 direction = crosshairLocation - (Vector2)transform.position;
            direction.Normalize();
            direction = Quaternion.Euler(0, 0, Random.Range(-30, 31)) * direction;
            ProjectileBase projectileScript = projectile.GetComponent<ProjectileBase>();
            projectileScript.direction = direction * projectileSpeed * projectileSpeedMultiplier;
            projectileScript.damage = damage * damageMultiplier;
            projectileScript.lifetime = projectileLifetime * projectileLifetimeMultiplier;
        }
    }
}
