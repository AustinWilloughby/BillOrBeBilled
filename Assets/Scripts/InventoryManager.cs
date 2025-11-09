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
        for(int i = 0; i < pickedUpRect.childCount; ++i)
        {
            ItemController missedItem = pickedUpRect.GetChild(i).GetComponent<ItemController>();

            inventorySO.spawnManager.SpawnItem(missedItem.itemType);
        }
    }
}
