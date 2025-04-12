using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Vector2 minPosition;
    public Vector2 maxPosition;
    public float smoothSpeed = 0.125f;

    private Transform target;
    private float camHalfHeight;
    private float camHalfWidth;

    void Start()
    {
        Camera cam = Camera.main;
        camHalfHeight = cam.orthographicSize;
        camHalfWidth = cam.aspect * camHalfHeight;

        // 🔍 Try to find the player automatically by tag
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            target = player.transform;
        }
        else
        {
            Debug.LogWarning("CameraFollow: No GameObject with tag 'Player' found.");
        }
    }

    void LateUpdate()
    {
        if (target != null)
        {
            Vector3 desiredPosition = new Vector3(target.position.x, target.position.y, transform.position.z);
            Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);

            float clampedX = Mathf.Clamp(smoothedPosition.x, minPosition.x + camHalfWidth, maxPosition.x - camHalfWidth);
            float clampedY = Mathf.Clamp(smoothedPosition.y, minPosition.y + camHalfHeight, maxPosition.y - camHalfHeight);

            float pixelPerUnit = 258f;
            clampedX = Mathf.Round(clampedX * pixelPerUnit) / pixelPerUnit;
            clampedY = Mathf.Round(clampedY * pixelPerUnit) / pixelPerUnit;

            transform.position = new Vector3(clampedX, clampedY, transform.position.z);
        }
    }
}
