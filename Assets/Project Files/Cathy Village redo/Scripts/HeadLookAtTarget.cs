using UnityEngine;

[RequireComponent(typeof(Animator))]
public class HeadLookAtTarget : MonoBehaviour
{
    [Header("Target Settings")]
    [Tooltip("The target object to look at. If null, will look at main camera.")]
    public Transform target;

    [Header("Tracking Settings")]
    [Tooltip("How quickly the head rotates to face the target")]
    [Range(0.1f, 10f)] public float lookSpeed = 3f;

    [Tooltip("Maximum angle the head can rotate vertically (up/down)")]
    [Range(0f, 90f)] public float maxVerticalAngle = 45f;

    [Tooltip("Maximum angle the head can rotate horizontally (left/right)")]
    [Range(0f, 90f)] public float maxHorizontalAngle = 60f;

    [Tooltip("Weight of the head look (0 = no effect, 1 = full effect)")]
    [Range(0f, 1f)] public float lookWeight = 0.8f;

    [Tooltip("Weight of the body look (0 = no effect, 1 = full effect)")]
    [Range(0f, 1f)] public float bodyWeight = 0.2f;

    [Tooltip("Weight of the eyes look (0 = no effect, 1 = full effect)")]
    [Range(0f, 1f)] public float eyesWeight = 1f;

    [Tooltip("Clamp weight (0 = no effect, 1 = full effect)")]
    [Range(0f, 1f)] public float clampWeight = 0.5f;

    [Header("Debug")]
    [Tooltip("Show debug gizmos for look direction")]
    public bool showDebug = true;

    private Animator animator;
    private float currentLookWeight = 0f;
    private Vector3 smoothedLookDirection;

    void Start()
    {
        animator = GetComponent<Animator>();

        // If no target specified, default to main camera
        if (target == null)
        {
            if (Camera.main != null)
            {
                target = Camera.main.transform;
            }
            else
            {
                Debug.LogWarning("No target assigned and no main camera found. Please assign a target.");
            }
        }
    }

    void OnAnimatorIK(int layerIndex)
    {
        if (animator == null || target == null) return;

        // Calculate direction to target
        Vector3 lookDirection = target.position - animator.GetBoneTransform(HumanBodyBones.Head).position;

        // Smooth the look direction for more natural movement
        smoothedLookDirection = Vector3.Slerp(smoothedLookDirection, lookDirection, Time.deltaTime * lookSpeed);

        // Clamp angles to prevent unnatural rotations
        Vector3 clampedDirection = ClampLookDirection(smoothedLookDirection);

        // Gradually increase look weight when target is in view
        float targetWeight = IsTargetInView(lookDirection) ? lookWeight : 0f;
        currentLookWeight = Mathf.Lerp(currentLookWeight, targetWeight, Time.deltaTime * lookSpeed);

        // Apply the look at position
        animator.SetLookAtWeight(currentLookWeight, bodyWeight, eyesWeight, 0f, clampWeight);
        animator.SetLookAtPosition(animator.GetBoneTransform(HumanBodyBones.Head).position + clampedDirection);
    }

    private Vector3 ClampLookDirection(Vector3 direction)
    {
        // Convert to local space for angle calculations
        Vector3 localDirection = transform.InverseTransformDirection(direction);

        // Calculate angles
        float horizontalAngle = Mathf.Atan2(localDirection.x, localDirection.z) * Mathf.Rad2Deg;
        float verticalAngle = Mathf.Atan2(localDirection.y, new Vector2(localDirection.x, localDirection.z).magnitude) * Mathf.Rad2Deg;

        // Clamp angles
        horizontalAngle = Mathf.Clamp(horizontalAngle, -maxHorizontalAngle, maxHorizontalAngle);
        verticalAngle = Mathf.Clamp(verticalAngle, -maxVerticalAngle, maxVerticalAngle);

        // Convert back to direction
        float horizontalRad = horizontalAngle * Mathf.Deg2Rad;
        float verticalRad = verticalAngle * Mathf.Deg2Rad;

        float x = Mathf.Sin(horizontalRad) * Mathf.Cos(verticalRad);
        float y = Mathf.Sin(verticalRad);
        float z = Mathf.Cos(horizontalRad) * Mathf.Cos(verticalRad);

        return transform.TransformDirection(new Vector3(x, y, z)).normalized * direction.magnitude;
    }

    private bool IsTargetInView(Vector3 direction)
    {
        // Check if target is within view cone
        float angle = Vector3.Angle(transform.forward, direction);
        return angle < maxHorizontalAngle * 1.5f; // Slightly more generous than the clamp angle
    }

    void OnDrawGizmos()
    {
        if (!showDebug || animator == null || target == null) return;

        Transform head = animator.GetBoneTransform(HumanBodyBones.Head);
        if (head == null) return;

        // Draw line to target
        Gizmos.color = Color.red;
        Gizmos.DrawLine(head.position, target.position);

        // Draw smoothed look direction
        Gizmos.color = Color.green;
        Gizmos.DrawLine(head.position, head.position + smoothedLookDirection.normalized * 2f);

        // Draw clamped look direction
        Gizmos.color = Color.blue;
        Vector3 clampedDirection = ClampLookDirection(smoothedLookDirection);
        Gizmos.DrawLine(head.position, head.position + clampedDirection.normalized * 2f);
    }
}