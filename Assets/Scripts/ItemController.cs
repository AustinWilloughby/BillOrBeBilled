using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public enum ItemState
{
    None,
    Dragging,
    Placed
}

public class ItemController : MonoBehaviour
{
    List<TileController> tileColliders = new List<TileController>();

    RectTransform rect;

    ItemState currentState;

    [SerializeField]
    InventorySO inventorySO;

    [SerializeField]
    ItemTypes itemType;

    public ItemTypes ItemType
    {
        get { return itemType; }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rect = GetComponent<RectTransform>();

        foreach(TileController childTile in transform.GetComponentsInChildren<TileController>())
        {
            tileColliders.Add(childTile);
        }
    }

    // Update is called once per frame
    void Update()
    {
        switch (currentState)
        {
            case ItemState.None:
                break;
            case ItemState.Dragging:
                FollowMouse();

                //  Have each tile check if it is over a valid space
                foreach(TileController tile in tileColliders)
                {
                    tile.OverTileCheck();
                }
                break;
            case ItemState.Placed:
                break;
        }
    }

    void FollowMouse()
    {
        //  Check if player would move outside the bounds
        Vector2 newPos = Mouse.current.position.ReadValue();

        SetPosition(newPos);
    }

    void OnStartDrag()
    {
        rect = transform.parent.GetComponent<RectTransform>();
    }

    public void OnClick()
    {
        ChangeStateTo(ItemState.Dragging);

        inventorySO.uiInventoryManager.ActiveItem = this;
    }

    public void OnRelease()
    {
        inventorySO.uiInventoryManager.ActiveItem = null;

        //  Check if each tile is over a valid space
        foreach (TileController tile in tileColliders)
        {
            if (!tile.IsOverValid())
            {
                ChangeStateTo(ItemState.None);
                return;
            }
        }

        //  All tiles are over a valid space
        ChangeStateTo(ItemState.Placed);
    }

    void ChangeStateTo(ItemState newState)
    {
        switch (newState)
        {
            case ItemState.Dragging:
                rect.SetParent(inventorySO.uiInventoryManager.menuRect);

                rect.anchorMin = Vector2.zero;
                rect.anchorMax = Vector2.zero;
                break;
            case ItemState.None:
                rect.SetParent(inventorySO.uiInventoryManager.pickedUpRect);

                rect.rotation = Quaternion.identity;

                ResetAllTiles();
                break;
            case ItemState.Placed:
                rect.SetParent(inventorySO.uiInventoryManager.heldItemsRect);

                MoveToSnapPosition(tileColliders[0].GetSnapToPositionOffset());

                //ResetAllTiles();
                break;
        }

        currentState = newState;
    }

    void ResetAllTiles()
    {
        foreach(TileController tile in tileColliders)
        {
            tile.Reset();
        }
    }

    void SetPosition(Vector2 newPos)
    {
        rect.anchoredPosition = newPos;//inventorySO.uiInventoryManager.CanvasScaler.localScale;

    }

    public void AddRotation(float newRot)
    {
        rect.Rotate(0f, 0f, newRot);
    }

    void MoveToSnapPosition(Vector2 snapOffset)
    {
        SetPosition((Vector2)rect.position + snapOffset);
    }
}
