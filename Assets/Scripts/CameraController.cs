using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

public class CameraController : MonoBehaviour {
    public bool enableMouseWarp;

    public GameObject playerController;
    public float maxCursorDistance; // maximum vertical cursor distance allowed from player, measured in world space
    public float maxCameraDistance; // maximum camera distance allowed from player, measured in world space
    public float lerpTime; // time required to finish lerping


    private PlayerController playerScript;
    public Vector3 currentMouseWorldSpacePosition;
    private Vector3 currentMouseWorldSpacePositionRelativeToPlayer;
    private Vector3 velocity = Vector3.zero; // required variable for Vector3.SmoothDamp()

    private void Awake() {
        playerScript = playerController.GetComponent<PlayerController>();
    }

    private void Start() {
        currentMouseWorldSpacePosition = playerScript.transform.position;
        currentMouseWorldSpacePosition.z = 0;
        currentMouseWorldSpacePositionRelativeToPlayer = Vector3.zero;
        Mouse.current.WarpCursorPosition(Camera.main.WorldToScreenPoint(currentMouseWorldSpacePosition));
    }

    private void LateUpdate() {
        Vector2 mouseDelta = Mouse.current.delta.ReadValue();
        float xPixelDist = 2 * Camera.main.orthographicSize * Camera.main.aspect / Screen.width;
        float yPixelDist = 2 * Camera.main.orthographicSize / Screen.height;
        currentMouseWorldSpacePositionRelativeToPlayer += new Vector3(xPixelDist * mouseDelta.x, yPixelDist * mouseDelta.y, 0);
        currentMouseWorldSpacePositionRelativeToPlayer.x = Mathf.Clamp(currentMouseWorldSpacePositionRelativeToPlayer.x, -maxCursorDistance * Camera.main.aspect, maxCursorDistance * Camera.main.aspect);
        currentMouseWorldSpacePositionRelativeToPlayer.y = Mathf.Clamp(currentMouseWorldSpacePositionRelativeToPlayer.y, -maxCursorDistance, maxCursorDistance);
        currentMouseWorldSpacePosition = playerScript.transform.position + currentMouseWorldSpacePositionRelativeToPlayer;
        currentMouseWorldSpacePosition.z = 0;
        Vector3 destinationPosition = Vector3.ClampMagnitude(currentMouseWorldSpacePositionRelativeToPlayer, maxCameraDistance) + playerScript.transform.position;
        transform.position = Vector3.SmoothDamp(transform.position, destinationPosition + Vector3.back, ref velocity, lerpTime);
        playerScript.currentCrosshairLocation = currentMouseWorldSpacePosition;
        playerScript.UpdateRotation(currentMouseWorldSpacePosition);
        if (enableMouseWarp) {
            Mouse.current.WarpCursorPosition(Camera.main.WorldToScreenPoint(currentMouseWorldSpacePosition));
        } else {
            currentMouseWorldSpacePositionRelativeToPlayer = Vector3.zero;
        }
    }
}
