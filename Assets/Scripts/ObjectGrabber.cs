using UnityEngine;
using UnityEngine.InputSystem;

public class ObjectGrabber : MonoBehaviour
{
    [SerializeField] LayerMask pickupMask;
    [SerializeField] float distInFront;
    [SerializeField] float distDown;
    [SerializeField] float pickupRadius;

    Collider[] pickupsInFront;

    [SerializeField]
    InventorySO inventorySO;

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
                inventorySO.uiInventoryManager.PickupItemOfType(c.transform.root.GetComponent<PropItemData>().itemType);



                Destroy(c.transform.root.gameObject);
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
