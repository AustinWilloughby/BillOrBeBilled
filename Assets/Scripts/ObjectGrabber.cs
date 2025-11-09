using UnityEngine;
using UnityEngine.InputSystem;

public class ObjectGrabber : MonoBehaviour
{
    [SerializeField] LayerMask pickupMask;
    [SerializeField] float distInFront;
    [SerializeField] float distDown;
    [SerializeField] float pickupRadius;

    Collider[] pickupsInFront;

    private void FixedUpdate()
    {
        Vector3 position = transform.position + (transform.forward * distInFront);
        position += transform.up * (-1.0f * distDown);
        pickupsInFront = Physics.OverlapSphere(position, pickupRadius, pickupMask);
    }

    public void OnClick(InputAction.CallbackContext ctx)
    {
        if(ctx.phase == InputActionPhase.Performed)
        {
            foreach(Collider c in pickupsInFront)
            {
                //TODO: Add to inventory
                Destroy(c.gameObject);
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Vector3 position = transform.position + (transform.forward * distInFront);
        position += transform.up * (-1.0f * distDown);
        Gizmos.DrawWireSphere(position, pickupRadius);
    }
}
