using UnityEngine;

public class RotateCube : MonoBehaviour
{
    [Tooltip("Rotation speed in degrees per second")]
    public float rotationSpeed = 90f;

    [Tooltip("Rotation axis (normalized vector)")]
    public Vector3 rotationAxis = Vector3.up;

    void Update()
    {
        // Rotate the cube around its own axis
        transform.Rotate(rotationAxis.normalized * rotationSpeed * Time.deltaTime);
    }
}