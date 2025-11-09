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
    [SerializeField]
    List<TileController> tileColliders;

    Vector2 moveDelta;

    [SerializeField]
    RectTransform rect;

    [SerializeField]
    ItemState currentState;

    [SerializeField]
    CanvasSO canvasSO;

    [SerializeField]
    Vector2 itemTileSize;

    [SerializeField]
    InventorySO inventorySO;

    public ItemTypes itemType;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        foreach(BoxCollider2D childCollider in transform.GetComponentsInChildren<BoxCollider2D>())
        {
            tileColliders.Add(childCollider.GetComponent<TileController>());
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

        //newPos /= canvasSO.canvasRect.localScale;

        SetPosition(newPos);
    }

    void OnStartDrag()
    {
        rect = transform.parent.GetComponent<RectTransform>();
    }

    public void OnClick()
    {
        ChangeStateTo(ItemState.Dragging);

        canvasSO.inventoryManager.activeItem = this;
    }

    public void OnRelease()
    {
        canvasSO.inventoryManager.activeItem = null;

        //  Check if each tile is over a valid space
        foreach (TileController tile in tileColliders)
        {
            if (tile.State != TileState.OverValid)
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
                rect.SetParent(canvasSO.inventoryManager.rect);

                rect.anchorMin = Vector2.zero;
                rect.anchorMax = Vector2.zero;
                break;
            case ItemState.None:
                rect.SetParent(canvasSO.inventoryManager.pickedUpRect);

                rect.rotation = Quaternion.identity;

                ResetAllTiles();
                break;
            case ItemState.Placed:
                rect.SetParent(canvasSO.inventoryManager.heldItemsRect);

                SetPosition(GetSnappedCenterPos(tileColliders[0].GetSnapToPosition()));

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
        rect.anchoredPosition = newPos / canvasSO.canvasRect.localScale;
    }

    public void AddRotation(float newRot)
    {
        rect.Rotate(0f, 0f, newRot);
    }

    Vector2 GetSnappedCenterPos(Vector2 tileCenter)
    {
        Vector2 snapCenter = tileCenter;

        //Debug.Log(transform.rotation.eulerAngles.z);
        switch(transform.rotation.eulerAngles.z)
        {
            case 90f:
                if (itemTileSize.y % 2 == 0)
                {
                    snapCenter.x -= 32f * canvasSO.canvasRect.localScale.x;
                }

                if (itemTileSize.x % 2 == 0)
                {
                    snapCenter.y -= 32f * canvasSO.canvasRect.localScale.y;
                }
                break;
            case 180f:
                if (itemTileSize.x % 2 == 0)
                {
                    snapCenter.x += 32f * canvasSO.canvasRect.localScale.x;
                }

                if (itemTileSize.y % 2 == 0)
                {
                    snapCenter.y -= 32f * canvasSO.canvasRect.localScale.y;
                }
                break;
            case 270f:
            case -90f:
                if (itemTileSize.y % 2 == 0)
                {
                    snapCenter.x += 32f * canvasSO.canvasRect.localScale.x;
                }

                if (itemTileSize.x % 2 == 0)
                {
                    snapCenter.y += 32f * canvasSO.canvasRect.localScale.y;
                }
                break;
            case 0:
                if (itemTileSize.x % 2 == 0)
                {
                    snapCenter.x -= 32f * canvasSO.canvasRect.localScale.x;
                }

                if (itemTileSize.y % 2 == 0)
                {
                    snapCenter.y += 32f * canvasSO.canvasRect.localScale.y;
                }
                break;
        }

        return snapCenter;
    }
}
