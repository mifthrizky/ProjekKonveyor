using UnityEngine;

public class XRVelocityTracker : MonoBehaviour
{
    public Vector3 velocity { get; private set; }
    public Vector3 angularVelocity { get; private set; }

    private Vector3 lastPosition;
    private Quaternion lastRotation;

    private void Start()
    {
        lastPosition = transform.position;
        lastRotation = transform.rotation;
    }

    private void FixedUpdate()
    {
        velocity = (transform.position - lastPosition) / Time.fixedDeltaTime;

        Quaternion deltaRotation = transform.rotation * Quaternion.Inverse(lastRotation);
        deltaRotation.ToAngleAxis(out float angleInDegrees, out Vector3 rotationAxis);
        if (angleInDegrees > 180f) angleInDegrees -= 360f;
        angularVelocity = rotationAxis * angleInDegrees * Mathf.Deg2Rad / Time.fixedDeltaTime;

        lastPosition = transform.position;
        lastRotation = transform.rotation;
    }
}
