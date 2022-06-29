using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMovementController : MonoBehaviour {
    public GameObject Sprite;
    public float speed;
    public float speedMultiplier = 1;
    public float desiredRange; // how close this enemy wants to get to the player
    public float aggroRange; // how far the player has to be to drop aggro
    public float aggroTimeout; // how long it takes for this enemy to stop looking for the player

    public bool inDesiredRange = false;

    public PlayerController playerScript;

    private Rigidbody2D rb2d;
    private CircleCollider2D circleCollider2d;
    
    private bool active = false;
    private bool playerInRange = false;
    private bool canSeePlayer = false;
    private float awakeTimer = 0;
    private Vector2 lastKnownPlayerPosition;

    private bool leftRcValid = false;
    private bool rightRcValid = false;
    private Vector2 velo = Vector2.zero;

    private void Awake() {
        rb2d = GetComponent<Rigidbody2D>();
        circleCollider2d = GetComponent<CircleCollider2D>();
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        if (collision.tag == "Player") {
            playerInRange = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision) {
        if (collision.tag == "Player") {
            playerInRange = false;
        }
    }  

    private void FixedUpdate() {
        inDesiredRange = false;

        if (!playerInRange && !active) {
            return;
        }

        // check if we should become active
        RaycastHit2D playerRaycast = Physics2D.Raycast(transform.position, playerScript.transform.position - transform.position, Mathf.Min(Vector2.Distance(transform.position, playerScript.transform.position), aggroRange), LayerMask.GetMask("Walls", "Player"));
        canSeePlayer = playerRaycast.collider != null && playerRaycast.collider.tag == "Player";

        if (playerInRange && canSeePlayer) {
            active = true;
        }

        if (!active) {
            return;
        }

        if (!canSeePlayer && Vector2.Distance(lastKnownPlayerPosition, transform.position) < 2) {
            // we are at the player's last known location and we don't see the player so the aggro counter starts counting down
            awakeTimer -= Time.fixedDeltaTime;
            velo = Vector2.zero;
            //rb2d.velocity = velo;
            if (awakeTimer < 0) {
                // aggro timer expired so we are no longer active
                active = false;
            }
            return;
        } else if (canSeePlayer) {
            // we can still see the player so we update the last known position and reset the aggro timer
            lastKnownPlayerPosition = playerScript.transform.position;
            awakeTimer = aggroTimeout;
            if (Vector2.Distance(transform.position, playerScript.transform.position) < desiredRange) {
                // player is in range for an attack
                rb2d.velocity = Vector2.zero;
                Sprite.transform.rotation = Quaternion.Euler(0, 0, Vectors.Vector2ToDegrees(playerScript.transform.position - transform.position) - 90);
                inDesiredRange = true;
                return;
            }
        }

        // player isn't in range for an attack, so we move closer
        Vector2 lastKnownPlayerDirection = lastKnownPlayerPosition - (Vector2)transform.position;
        lastKnownPlayerDirection.Normalize();
        Vector2 perpendicular = Vector2.Perpendicular(lastKnownPlayerDirection) * circleCollider2d.radius;
        Vector2 leftRcStart = (Vector2)transform.position + perpendicular;
        float leftDistance = Vector2.Distance(leftRcStart, lastKnownPlayerPosition);
        Vector2 rightRcStart = (Vector2)transform.position - perpendicular;
        float rightDistance = Vector2.Distance(rightRcStart, lastKnownPlayerPosition);
        RaycastHit2D leftRc = Physics2D.Raycast(leftRcStart, lastKnownPlayerPosition - leftRcStart, leftDistance, LayerMask.GetMask("Walls"));
        RaycastHit2D rightRc = Physics2D.Raycast(rightRcStart, lastKnownPlayerPosition - rightRcStart, rightDistance, LayerMask.GetMask("Walls"));

        leftRcValid = leftRc.collider == null;
        rightRcValid = rightRc.collider == null;
        Vector2 intersectPoint;

        if (leftRcValid && rightRcValid) {
            // no obstacles so we just beeline it for the player
            velo = lastKnownPlayerDirection * speed * speedMultiplier;
            rb2d.velocity = velo;
            Sprite.transform.rotation = Quaternion.Euler(0, 0, Vectors.Vector2ToDegrees(velo) - 90);
            return;
        } else if (leftRcValid || (!rightRcValid && leftRc.distance < rightRc.distance)) {
            // turn left, but use the right point in the reflection
            intersectPoint = rightRc.point;
        } else {
            // turn right, but use the left point in the reflection
            intersectPoint = leftRc.point;
        }

        // carefully navigate around whatever is in our way
        velo = Vector2.Reflect((Vector2)transform.position - intersectPoint, lastKnownPlayerDirection);
        velo.Normalize();
        velo *= speed * speedMultiplier;
        rb2d.velocity = velo;
        Sprite.transform.rotation = Quaternion.Euler(0, 0, Vectors.Vector2ToDegrees(velo) - 90);
    }

    private void Update() {
        // debug vectors
        if (playerInRange || active) {
            Color c = canSeePlayer ? Color.green : Color.red;
            Debug.DrawRay(transform.position, (Vector3)lastKnownPlayerPosition - transform.position, Color.yellow);
            Debug.DrawRay(transform.position, playerScript.transform.position - transform.position, c);

            Vector2 lastKnownPlayerDirection = lastKnownPlayerPosition - (Vector2)transform.position;
            lastKnownPlayerDirection.Normalize();
            Vector2 perpendicular = Vector2.Perpendicular(lastKnownPlayerDirection) * circleCollider2d.radius;
            Vector2 leftRcStart = (Vector2)transform.position + perpendicular;
            Vector2 rightRcStart = (Vector2)transform.position - perpendicular;
            Debug.DrawRay(leftRcStart, lastKnownPlayerPosition - leftRcStart, leftRcValid ? Color.cyan : Color.blue);
            Debug.DrawRay(rightRcStart, lastKnownPlayerPosition - rightRcStart, rightRcValid ? Color.cyan : Color.blue);

            Debug.DrawRay(transform.position, velo, Color.magenta);
        }
    }
}
