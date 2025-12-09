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
    List<TileController> tileControllers = new List<TileController>();

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
            tileControllers.Add(childTile);
        }
    }

    public void Init(RectTransform parent)
    {
        rect.SetParent(parent);

        rect.localScale = Vector3.one;

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
                foreach(TileController tile in tileControllers)
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

        //newPos.z = inventorySO.uiInventoryManager.pickedUpRect.position.z;

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
        foreach (TileController tile in tileControllers)
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
        Vector3 newPos = Vector3.zero;

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

                ResetZDepth(rect.anchoredPosition3D);
                break;
            case ItemState.Placed:
                rect.SetParent(inventorySO.uiInventoryManager.heldItemsRect);

                MoveToSnapPosition(GetActiveTileSnapOffset());
                break;
        }



        ResetZDepth(rect.anchoredPosition3D);

        currentState = newState;
    }

    void ResetAllTiles()
    {
        foreach(TileController tile in tileControllers)
        {
            tile.Reset();
        }
    }

    void SetPosition(Vector3 newPos)
    {
        newPos.z = rect.parent.transform.localPosition.z;

        rect.anchoredPosition3D = newPos;
    }

    public void AddRotation(float newRot)
    {
        rect.Rotate(0f, 0f, newRot);
    }

    void MoveToSnapPosition(Vector3 snapOffset)
    {
        transform.position += snapOffset;

        //SetPosition(snapPos);
    }

    Vector3 GetActiveTileSnapOffset()
    {
        foreach(TileController tile in tileControllers)
        {
            if(tile.IsActive())
            {
                return tile.GetSnapToPositionOffset();
            }
        }

        return Vector3.zero;
    }

    void ResetZDepth(Vector3 ancorPos)
    {
        ancorPos.z = 0f;

        rect.anchoredPosition3D = ancorPos;
    }
}
