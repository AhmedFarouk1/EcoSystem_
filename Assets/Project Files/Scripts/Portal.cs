using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Portal : MonoBehaviour
{
    [Header("Portal Settings")]
    [SerializeField] private Transform destinationPortal;
    [SerializeField] private float teleportCooldown = 1f;
    [SerializeField] private float fadeDuration = 0.5f;
    [SerializeField] private bool debugMode = true;

    [Header("Movement Settings")]
    [SerializeField] private float exitForwardOffset = 1f;
    [SerializeField] private float minExitDistance = 0.5f;

    [Header("Visual Effects")]
    [SerializeField] private Image fadeImage;

    private bool isOnCooldown = false;

    private void Start()
    {
        if (fadeImage != null)
        {
            fadeImage.color = Color.clear;
            fadeImage.enabled = false;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player") || isOnCooldown || destinationPortal == null)
            return;

        StartCoroutine(TeleportWithFade(other.gameObject));
    }

    private IEnumerator TeleportWithFade(GameObject player)
    {
        isOnCooldown = true;

        // Fade to black
        yield return StartCoroutine(FadeScreen(1f, fadeDuration / 2));

        PerformTeleport(player);

        // Fade back in
        yield return StartCoroutine(FadeScreen(0f, fadeDuration / 2));

        yield return new WaitForSeconds(teleportCooldown - fadeDuration);
        isOnCooldown = false;
    }

    private void PerformTeleport(GameObject player)
    {
        // Handle CharacterController
        CharacterController cc = player.GetComponent<CharacterController>();
        bool ccWasEnabled = cc != null && cc.enabled;
        if (ccWasEnabled) cc.enabled = false;

        // Calculate exit position with offset
        Vector3 exitPosition = destinationPortal.position +
                             (destinationPortal.forward * exitForwardOffset);

        // Ensure minimum distance from portal
        if (Vector3.Distance(exitPosition, destinationPortal.position) < minExitDistance)
        {
            exitPosition = destinationPortal.position +
                         (destinationPortal.forward * minExitDistance);
        }

        // Set Y position to 0
        exitPosition.y = 0f;

        // Make player face the portal's forward (blue arrow) direction
        Quaternion exitRotation = destinationPortal.rotation;

        // Apply teleportation
        player.transform.SetPositionAndRotation(exitPosition, exitRotation);

        // Preserve velocity magnitude but align with portal forward
        Rigidbody rb = player.GetComponent<Rigidbody>();
        if (rb != null)
        {
            float speed = rb.linearVelocity.magnitude;
            rb.linearVelocity = destinationPortal.forward * speed;
        }

        // Restore CharacterController
        if (ccWasEnabled) cc.enabled = true;

        if (debugMode)
        {
            Debug.Log($"Teleported player to: {exitPosition}");
            Debug.DrawRay(exitPosition, destinationPortal.forward * 3f, Color.blue, 2f);
        }
    }

    private IEnumerator FadeScreen(float targetAlpha, float duration)
    {
        if (fadeImage == null) yield break;

        fadeImage.enabled = true;
        Color startColor = fadeImage.color;
        Color endColor = new Color(0, 0, 0, targetAlpha);
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            fadeImage.color = Color.Lerp(startColor, endColor, elapsed / duration);
            yield return null;
        }

        fadeImage.color = endColor;
        if (targetAlpha == 0f) fadeImage.enabled = false;
    }

    private void OnDrawGizmosSelected()
    {
        if (destinationPortal != null)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawLine(transform.position, destinationPortal.position);
            Gizmos.DrawWireSphere(destinationPortal.position, 0.3f);

            // Draw portal forward direction
            Gizmos.color = Color.blue;
            Gizmos.DrawRay(destinationPortal.position, destinationPortal.forward * 2f);
        }
    }
}