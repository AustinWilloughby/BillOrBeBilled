using UnityEngine;
using DG.Tweening;

public class CameraFollow : MonoBehaviour
{
    [Header("Target")]
    [SerializeField] private Rigidbody targetRigidbody;

    [Header("Follow Settings")]
    [SerializeField] private Vector3 offset = new Vector3(0f, 2.5f, -7f);

    [Header("Look Settings")]
    [SerializeField] private Vector3 lookAtOffset = new Vector3(0f, 0f, 5f);
    [SerializeField] private float lookAheadDistance = 3f;
    [SerializeField] private float lookAheadSpeed = 2f;

    [Header("Smoothing")]
    [SerializeField] private float tweenDuration = 0.3f;
    [SerializeField] private Ease tweenEase = Ease.OutQuad;

    private Tween positionTween;
    private Tween rotationTween;
    private Vector3 currentLookAhead;

    private void LateUpdate()
    {
        if (targetRigidbody == null) return;

        // Calculate target position based on target's rotation and offset
        Vector3 targetPosition = targetRigidbody.position + targetRigidbody.rotation * offset;

        // Calculate base look-at point with configurable offset (in target's local space)
        Vector3 baseLookAtPoint = targetRigidbody.position + targetRigidbody.rotation * lookAtOffset;

        // Add velocity-based look-ahead (only if moving)
        if (targetRigidbody.linearVelocity.sqrMagnitude > 0.01f) // Check if actually moving
        {
            Vector3 targetLookAhead = targetRigidbody.linearVelocity.normalized * lookAheadDistance;
            currentLookAhead = Vector3.Lerp(currentLookAhead, targetLookAhead, lookAheadSpeed * Time.deltaTime);
        }
        // When stopped, keep the current look-ahead (don't lerp back to zero)

        // Combine base look-at with velocity look-ahead
        Vector3 lookAtPoint = baseLookAtPoint + currentLookAhead;
        Vector3 directionToTarget = lookAtPoint - targetPosition;

        // Ensure we have a valid direction
        if (directionToTarget.sqrMagnitude < 0.001f)
        {
            directionToTarget = targetRigidbody.rotation * Vector3.forward;
        }

        Quaternion targetRotation = Quaternion.LookRotation(directionToTarget);

        // Remove Z-axis rotation (roll)
        Vector3 euler = targetRotation.eulerAngles;
        targetRotation = Quaternion.Euler(euler.x, euler.y, 0f);

        // Kill and restart tweens each frame for smooth following
        positionTween?.Kill();
        rotationTween?.Kill();

        positionTween = transform.DOMove(targetPosition, tweenDuration).SetEase(tweenEase);
        rotationTween = transform.DORotateQuaternion(targetRotation, tweenDuration).SetEase(tweenEase);
    }

    private void OnDestroy()
    {
        // Clean up tweens
        positionTween?.Kill();
        rotationTween?.Kill();
    }

    private void OnDrawGizmosSelected()
    {
        if (targetRigidbody == null) return;

        // Visualize the camera offset
        Vector3 targetPosition = targetRigidbody.position + targetRigidbody.rotation * offset;

        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(targetPosition, 0.3f);
        Gizmos.DrawLine(targetRigidbody.position, targetPosition);

        // Draw base look-at point
        Vector3 baseLookAtPoint = targetRigidbody.position + targetRigidbody.rotation * lookAtOffset;
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(baseLookAtPoint, 0.4f);
        Gizmos.DrawLine(targetRigidbody.position, baseLookAtPoint);

        // Draw final look-at point with velocity
        Vector3 lookAtPoint = baseLookAtPoint + currentLookAhead;
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(lookAtPoint, 0.5f);

        // Draw camera view direction
        Gizmos.color = Color.yellow;
        Gizmos.DrawRay(targetPosition, (lookAtPoint - targetPosition).normalized * 2f);
    }
}