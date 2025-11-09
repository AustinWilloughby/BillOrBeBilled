using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class InventoryManager : MonoBehaviour
{
    [SerializeField]
    CanvasSO canvasSO;

    [SerializeField]
    Canvas canvas;

    [SerializeField]
    CanvasScaler canvasScaler;

    [SerializeField]
    GraphicRaycaster raycaster;

    public RectTransform rect, pickedUpRect, storageRect, heldItemsRect, canvasRect;

    public ItemController activeItem;

    [SerializeField]
    InventorySO inventorySO;

    List<ItemController> storedItems;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        canvasSO.canvasRect = canvasRect;
        canvasSO.raycaster = raycaster;

        canvasSO.canvasScaler = canvasScaler;

        canvasSO.inventoryManager = this;

        inventorySO.uiInventoryManager = this;
    }

    // Update is called once per frame
    void Update()
    {
        
    }



    public void OnRotate(InputAction.CallbackContext context)
    {
        if(activeItem == null)
        {
            return;
        }

        if (context.performed)
        {
            Vector2 playerInput = context.ReadValue<Vector2>();

            float newRot = playerInput.x;

            if (newRot > 0)
            {
                newRot = 90f;
            }
            else if (newRot < 0)
            {
                newRot = -90f;
            }

            //Debug.Log(newRot);

            activeItem.AddRotation(newRot);
        }
    }

    public void PickupItemOfType(ItemTypes itemType)
    {
        RectTransform newItem = Instantiate(inventorySO.GetItemDataByType(itemType).itemUIPrefab);

        newItem.SetParent(pickedUpRect);

        gameObject.SetActive(true);
    }

    private void OnDisable()
    {
        //
        //  Spawn any not stored items
        //
        for(int i = 0; i < pickedUpRect.childCount; ++i)
        {
            ItemController missedItem = pickedUpRect.GetChild(i).GetComponent<ItemController>();

            inventorySO.spawnManager.SpawnItem(missedItem.itemType);
        }


        //
        //  Save Stored Items
        //
        storedItems.Clear();

        for (int i = 0; i < heldItemsRect.childCount; ++i)
        {
            storedItems.Add(heldItemsRect.GetChild(i).GetComponent<ItemController>());
        }
    }

    public bool TradeItem(ItemTypes itemType)
    {
        ItemController foundItem = null;

        foreach (ItemController item in storedItems)
        {
            if (item.itemType == itemType)
            {
                foundItem = item;

                break;
            }
        }

        if (foundItem != null)
        {
            storedItems.Remove(foundItem);

            Destroy(foundItem.gameObject);

            return true;
        }

        return false;
    }
}
