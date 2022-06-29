using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pistol : WeaponBase {
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
}
