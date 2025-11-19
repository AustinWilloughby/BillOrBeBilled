using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class InventoryManager : MonoBehaviour
{
    [SerializeField]
    Canvas _canvas;

    CanvasScaler canvasScaler;
    public CanvasScaler CanvasScaler
    {
        get { return canvasScaler; }
    }

    GraphicRaycaster raycaster;
    public GraphicRaycaster Raycaster
    {
        get { return raycaster; }
    }
    //RectTransform canvasRect;

//      NOT SURE IF TWO PLAYER_INPUTS WORKS
/*
 * Each PlayerInput instance represents a separate player or user. You can use multiple PlayerInput instances at the same time (although not on the same GameObject) to represent local multiplayer setups. The Input System pairs each player to a unique set of Devices that the player uses exclusively, but you can also manually pair Devices in a way that enables two or more players to share a Device (for example, left/right keyboard splits or hot seat use).
 * */


[SerializeField]
CanvasSO canvasSO;


[SerializeField]
Camera _camera;



public RectTransform menuRect, pickedUpRect, storageRect, heldItemsRect;

ItemController activeItem;

public ItemController ActiveItem
{
    set { activeItem = value; }
}

[SerializeField]
InventorySO inventorySO;

List<ItemController> storedItems = new List<ItemController>();

bool isActive = true;

// Start is called once before the first execution of Update after the MonoBehaviour is created
void Awake()
{
    canvasScaler = _canvas.GetComponent<CanvasScaler>();
    raycaster = _canvas.GetComponent<GraphicRaycaster>();
    //canvasRect = _canvas.GetComponent<RectTransform>();

    //canvasSO.canvasRect = canvasRect;
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

    newItem.localScale = Vector2.one;

    newItem.anchoredPosition3D = Vector3.zero;

    SetActive(true);
}

private void OnDisable()
{
    //
    //  Spawn any not stored items
    //
    for(int i = 0; i < pickedUpRect.childCount; ++i)
    {
        ItemController missedItem = pickedUpRect.GetChild(i).GetComponent<ItemController>();

        inventorySO.propDropperManager.SpawnItem(missedItem.ItemType);

        Destroy(missedItem.gameObject);
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
        if (item.ItemType == itemType)
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

public void OnToggleInventory(InputAction.CallbackContext context)
{
    SetActive(!isActive);
}

public void SetActive(bool value)
{
    isActive = value;

    _camera.gameObject.SetActive(value);
}
}
