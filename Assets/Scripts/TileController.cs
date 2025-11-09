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
    CanvasSO canvasSO;

    [SerializeField]
    int hitTileCount = 0;

    Vector2 snapPosition = Vector2.zero;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        SetStateTo(isActive);

        switch (currentState)
        {
            case TileState.None:
                tileImage.color = Color.white;
                break;
            case TileState.OverValid:
                tileImage.color = Color.green;
                break;
            case TileState.OverInvalid:
                tileImage.color = Color.red;
                break;
        }
    }

    private void OnEnable()
    {
        SetStateTo(isActive);
    }

    void SetStateTo(bool value)
    {
        tileImage.enabled = value;
    }

    public void OverTileCheck()
    {
        //  Reset Snap Position
        snapPosition = Vector2.zero;

        PointerEventData pointerEvent = new PointerEventData(EventSystem.current);

        pointerEvent.position = transform.position;

        canvasSO.raycaster.Raycast(pointerEvent, raycastResults);


        hitTileCount = 0;
        foreach (RaycastResult result in raycastResults)
        {
            if (result.gameObject.CompareTag("Tile"))
            {
                ++hitTileCount;
            }

            if(result.gameObject.name.Contains("Storage - Tile"))
            {
                snapPosition = result.gameObject.transform.position;
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

    public Vector2 GetSnapToPosition()
    {
        return snapPosition;
    }
}
