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

    List<RaycastResult> raycastResults = new List<RaycastResult>();

    [SerializeField]
    CanvasSO canvasSO;

    [SerializeField]
    int hitTileCount = 0;

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
        PointerEventData pointerEvent = new PointerEventData(EventSystem.current);

        pointerEvent.position = transform.position;

        canvasSO.raycaster.Raycast(pointerEvent, raycastResults);

        Debug.Log(raycastResults.Count);

        hitTileCount = 0;
        foreach(RaycastResult result in raycastResults)
        {
            if(result.gameObject.CompareTag("Tile"))
            {
                ++hitTileCount;
            }
        }

        if (hitTileCount > 1)
        {
            currentState = TileState.OverValid;
        }
        else
        {
            currentState = TileState.OverInvalid;
        }

        raycastResults.Clear();
    }    
}
