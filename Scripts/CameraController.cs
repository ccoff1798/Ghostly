using UnityEngine;

public class ThirdPersonCameraController : MonoBehaviour
{
    public float rotationSpeed = 5.0f;
    public Transform playerModel;
    public Transform cameraHolder;
    public float cameraCollisionOffset = 0.3f; // Offset to prevent camera from going inside objects
    public float cameraDistance = 3.0f; // Default distance from the player
    public LayerMask collisionLayer; // Layer to detect collisions with

    void Start()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        if (cameraHolder == null && Camera.main != null)
        {
            cameraHolder = Camera.main.transform.parent;
        }
    }

    void Update()
    {
        float mouseX = Input.GetAxis("Mouse X") * rotationSpeed;
        if (playerModel != null)
        {
            playerModel.Rotate(Vector3.up, mouseX);
        }
        else
        {
            Debug.LogError("Player model is not assigned to the script!");
        }

        if (cameraHolder != null)
        {
            AdjustCameraPosition();
        }
        else
        {
            Debug.LogError("Camera holder is not assigned to the script!");
        }
    }

    void AdjustCameraPosition()
{
    RaycastHit hit;
    Vector3 start = playerModel.position + Vector3.up * 1.0f;
    Vector3 dir = -cameraHolder.forward;
    float desiredCameraDistance = cameraDistance;
    float sphereRadius = 0.5f;

    if (Physics.SphereCast(start, sphereRadius, dir, out hit, cameraDistance, collisionLayer))
    {
        desiredCameraDistance = hit.distance - cameraCollisionOffset;
        desiredCameraDistance = Mathf.Clamp(desiredCameraDistance, 0.5f, cameraDistance); // Prevent camera from going too close or too far
    }

    // Use a faster smoothing rate if the camera is very close to an obstacle to avoid clipping
    float smoothRate = (desiredCameraDistance < 1.0f) ? 20f : 10f;
    cameraHolder.localPosition = Vector3.Lerp(cameraHolder.localPosition, new Vector3(0.0f, 0.0f, -desiredCameraDistance), Time.deltaTime * smoothRate);
}

}
