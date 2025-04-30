using UnityEngine;

public class OneTimeCollisionTrigger : MonoBehaviour
{
    [Tooltip("The GameObject containing the component to enable")]
    public GameObject targetGameObject;

    [Tooltip("The component to enable on collision (must be a Behaviour like MonoBehaviour, Renderer, Collider, etc.)")]
    public Behaviour componentToEnable;

    [Tooltip("The collider that will be disabled after triggering (leave empty to disable this object's collider)")]
    public Collider colliderToDisable;

    private bool hasTriggered = false;

    private void Start()
    {
        // If no specific collider is assigned, use this object's collider
        if (colliderToDisable == null)
        {
            colliderToDisable = GetComponent<Collider>();
        }

        // Verify the component exists on the target GameObject
        if (targetGameObject != null && componentToEnable != null)
        {
            if (componentToEnable.gameObject != targetGameObject)
            {
                Debug.LogWarning("Component is not on the target GameObject!", this);
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!hasTriggered && targetGameObject != null && componentToEnable != null)
        {
            // Enable the specified component on the target GameObject
            componentToEnable.enabled = true;

            // Disable the collider
            if (colliderToDisable != null)
            {
                colliderToDisable.enabled = false;
            }

            hasTriggered = true;
        }
    }

    // Alternative for trigger colliders
    private void OnTriggerEnter(Collider other)
    {
        if (!hasTriggered && targetGameObject != null && componentToEnable != null)
        {
            componentToEnable.enabled = true;

            if (colliderToDisable != null)
            {
                colliderToDisable.enabled = false;
            }

            hasTriggered = true;
        }
    }
}