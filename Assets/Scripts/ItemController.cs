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
    void Awake()
    {
        rect = GetComponent<RectTransform>();

        foreach(TileController childTile in transform.GetComponentsInChildren<TileController>())
        {
            tileColliders.Add(childTile);
        }
    }

    public void Init(RectTransform parent)
    {
        rect.SetParent(parent);

        rect.localScale = Vector2.one;

        rect.anchoredPosition3D = new Vector3(0f, 0f, parent.position.z);

        ResetAllTiles();
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

        RectTransformUtility.ScreenPointToLocalPointInRectangle(inventorySO.uiInventoryManager.menuRect, newPos, inventorySO.uiInventoryManager.Camera, out newPos);

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

                rect.anchorMin = new Vector2(.5f, .5f);
                rect.anchorMax = new Vector2(.5f, .5f);
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

    void SetPosition(Vector3 newPos)
    {
        rect.anchoredPosition3D = newPos;// / inventorySO.uiInventoryManager.CanvasRect.localScale;
    }

    public void AddRotation(float newRot)
    {
        rect.Rotate(0f, 0f, newRot);
    }

    void MoveToSnapPosition(Vector2 snapOffset)
    {
        Vector3 snapPos = rect.anchoredPosition3D;
        snapPos.x += snapOffset.x;
        snapPos.y += snapOffset.y;
        snapPos.z = 0;

        SetPosition(snapPos);
    }
}
