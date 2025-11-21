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

    [SerializeField]
    InventorySO inventorySO;

    [SerializeField]
    int hitTileCount = 0;

    //Vector2 snapPosition = Vector2.zero;
    Vector2 snapOffset = Vector2.zero;

    [SerializeField]
    Color normalColor, validColor, invalidColor;

    RectTransform rect;

    BoxCollider2D boxCollider;

    List<RaycastHit2D> raycastHits = new List<RaycastHit2D>();

    [SerializeField]
    LayerMask tileLayerMask;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        rect = GetComponent<RectTransform>();

        boxCollider = GetComponent<BoxCollider2D>();

        boxCollider.enabled = tileImage.enabled;
    }

    public void Init(RectTransform parent)
    {
        rect.SetParent(parent);

        rect.localScale = Vector2.one;

        rect.anchoredPosition3D = new Vector3(0f, 0f, parent.position.z);

        Reset();
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
        if (!tileImage.isActiveAndEnabled)
            return;

        //  Reset Snap Position
        snapOffset = Vector2.zero;

        //  Raycast
        Vector3 raycastOrigin = transform.position;
        raycastOrigin.z -= 1f;
        Physics2D.Raycast(raycastOrigin, transform.forward, ContactFilter2D.noFilter, raycastHits);

        
        hitTileCount = 0;
        foreach (RaycastHit2D result in raycastHits)
        {
             ++hitTileCount;

             snapOffset = result.transform.position - transform.position;            
        }


        Debug.Log(raycastHits.Count);

        if (hitTileCount == 1)
        {
            currentState = TileState.OverValid;
        }
        else
        {
            currentState = TileState.OverInvalid;
        }

        raycastHits.Clear();
    }
    
    public void Reset()
    {
        currentState = TileState.None;

        boxCollider.size = inventorySO.uiInventoryManager.TileSize;
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

    private void OnDrawGizmos()
    {
        if (rect != null && tileImage.isActiveAndEnabled)
        {
            Gizmos.color = Color.red;

            Vector3 raycastOrigin = transform.position;
            //raycastOrigin.z -= 1f;
            Gizmos.DrawRay(raycastOrigin, transform.forward);
        }
    }

    public Vector2 GetTileSize()
    {
        return boxCollider.size;
    }

    public void SetTileSize(Vector2 size)
    {

    }
}
