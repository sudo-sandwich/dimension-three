using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Vectors {
    public static Vector2 DegreesToVector2(float degrees) {
        return new Vector2(Mathf.Cos(degrees * Mathf.Deg2Rad), Mathf.Sin(degrees * Mathf.Deg2Rad));
    }

    public static float Vector2ToDegrees(Vector2 vector) {
        return Vector2ToRadians(vector) * Mathf.Rad2Deg;
    }

    public static float Vector2ToRadians(Vector2 vector) {
        return Mathf.Atan2(vector.y, vector.x);
    }
}
