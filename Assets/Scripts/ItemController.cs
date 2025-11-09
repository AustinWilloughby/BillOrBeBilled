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

        SetPosition(newPos);
    }

    void OnStartDrag()
    {
        rect = transform.parent.GetComponent<RectTransform>();
    }

    public void OnClick()
    {
        ChangeStateTo(ItemState.Dragging);
    }

    public void OnRelease()
    {
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

                ResetAllTiles();
                break;
            case ItemState.Placed:
                rect.SetParent(canvasSO.inventoryManager.heldItemsRect);

                SetPosition(tileColliders[0].GetSnapToPosition());

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
        rect.anchoredPosition = newPos;
    }
}
