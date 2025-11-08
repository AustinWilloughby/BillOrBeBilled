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

        rect.anchoredPosition = newPos;
    }

    void OnStartDrag()
    {
        rect = transform.parent.GetComponent<RectTransform>();
    }
}
