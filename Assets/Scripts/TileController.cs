using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public enum TileState
{
    None,
    OverValid,
    OverInvalid
}

public class TileController : MonoBehaviour
{
    [SerializeField]
    Image tileImage;

    [SerializeField]
    bool isActive = true;

    [SerializeField]
    TileState currentState;

    public TileState State
    {
        get { return currentState; }
    }

    List<RaycastResult> raycastResults = new List<RaycastResult>();

    [SerializeField]
    InventorySO inventorySO;

    [SerializeField]
    int hitTileCount = 0;

    //Vector2 snapPosition = Vector2.zero;
    Vector2 snapOffset = Vector2.zero;

    [SerializeField]
    Color normalColor, validColor, invalidColor;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        switch (currentState)
        {
            case TileState.None:
                tileImage.color = normalColor;
                break;
            case TileState.OverValid:
                tileImage.color = validColor;
                break;
            case TileState.OverInvalid:
                tileImage.color = invalidColor;
                break;
        }
    }

    public void OverTileCheck()
    {
        //  Reset Snap Position
        snapOffset = Vector2.zero;

        PointerEventData pointerEvent = new PointerEventData(EventSystem.current);

        pointerEvent.position = transform.position;

        inventorySO.uiInventoryManager.Raycaster.Raycast(pointerEvent, raycastResults);


        hitTileCount = 0;
        foreach (RaycastResult result in raycastResults)
        {
            if (result.gameObject.CompareTag("Tile"))
            {
                ++hitTileCount;
            }

            if(result.gameObject.name.Contains("Storage - Tile"))
            {
                snapOffset = result.gameObject.transform.position - transform.position;
            }
        }


        //Debug.Log(hitTileCount);

        if (hitTileCount == 2)
        {
            currentState = TileState.OverValid;
        }
        else
        {
            currentState = TileState.OverInvalid;
        }

        raycastResults.Clear();
    }
    
    public void Reset()
    {
        currentState = TileState.None;
    }

    public Vector2 GetSnapToPositionOffset()
    {
        return snapOffset;
    }

    public bool IsOverValid()
    {
        if(tileImage.enabled)
        {
            return currentState == TileState.OverValid;
        }

        return true;
    }
}
