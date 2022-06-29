using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Globals : MonoBehaviour {
    public static Globals instance;

    public Transform enemiesTransform;
    //public Transform zombiesTransform;
    public Transform projectilesTransform;

    public BasicProjectile basicProjectileScript;
    public ExplosiveProjectile explosiveProjectileScript;
    public BouncingProjectile bouncingProjectileScript;

    public static Transform enemies => instance.enemiesTransform;
    //public static Transform zombies => instance.zombiesTransform;
    public static Transform projectiles => instance.projectilesTransform;

    public static BasicProjectile basicProjectile => instance.basicProjectileScript;
    public static ExplosiveProjectile explosiveProjectile => instance.explosiveProjectileScript;
    public static BouncingProjectile bouncingProjectile => instance.bouncingProjectileScript;

    private void Awake() {
        if (instance == null) {
            instance = this;
        } else {
            Debug.Log("Duplicate projectile globals, destroying self.");
            Destroy(gameObject);
        }
    }
}
