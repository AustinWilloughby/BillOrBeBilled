using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ObjectGrabber : MonoBehaviour
{
    [SerializeField] LayerMask pickupMask;
    [SerializeField] float distInFront;
    [SerializeField] float distDown;
    [SerializeField] float pickupRadius;

    Collider[] pickupsInFront;
    List<PropItemData> inFrontItems = new List<PropItemData>();

    [SerializeField]
    InventorySO inventorySO;

    private void FixedUpdate()
    {
        Vector3 position = transform.position + (transform.forward * distInFront);
        position += transform.up * (-1.0f * distDown);

        pickupsInFront = Physics.OverlapSphere(position, pickupRadius, pickupMask);

        inFrontItems.Clear();
        foreach(Collider collider in pickupsInFront)
        {
            PropItemData pickupItem = collider.transform.root.GetComponent<PropItemData>();

            if(pickupItem != null && !inFrontItems.Contains(pickupItem))
            {
                inFrontItems.Add(pickupItem);
            }            
        }
    }

    public void OnClick(InputAction.CallbackContext ctx)
    {
        if(ctx.phase == InputActionPhase.Performed)
        {
            foreach(PropItemData pickupItem in inFrontItems)
            {
                inventorySO.uiInventoryManager.PickupItemOfType(pickupItem.itemType);

                Destroy(pickupItem.gameObject);
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
