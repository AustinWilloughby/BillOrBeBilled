using System.Collections.Generic;
using UnityEngine;
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
    TileState currentState;

    public TileState State
    {
        get { return currentState; }
    }

    [SerializeField]
    InventorySO inventorySO;

    //Vector2 snapPosition = Vector2.zero;
    Vector2 snapOffset = Vector2.zero;
    Vector3 snapOffset3d = Vector3.zero;

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
                tileImage.color = inventorySO.normalColor;
                break;
            case TileState.OverValid:
                tileImage.color = inventorySO.validColor;
                break;
            case TileState.OverInvalid:
                tileImage.color = inventorySO.invalidColor;
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
        Physics2D.Raycast(transform.position, transform.forward, ContactFilter2D.noFilter, raycastHits);

        int validTiles = 0;
        foreach (RaycastHit2D result in raycastHits)
        {
            if (result.transform.CompareTag(inventorySO.k_STORAGE_TAG))
            {
                ++validTiles;
              
                Vector2 storageTilePoint = RectTransformUtility.WorldToScreenPoint(inventorySO.uiInventoryManager.Camera, result.transform.position);

                Vector2 myPoint = RectTransformUtility.WorldToScreenPoint(inventorySO.uiInventoryManager.Camera, transform.position);

                snapOffset = storageTilePoint - myPoint;

                snapOffset3d = result.transform.position - transform.position;

                //Debug.Log(snapOffset);
            }                         
        }

        if (validTiles == 1 && raycastHits.Count == 2)
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

        if (inventorySO.uiInventoryManager.TileSize.x == 0)
        {
            boxCollider.size = rect.rect.size * 2f;
        }
        else
        {
            boxCollider.size = inventorySO.uiInventoryManager.TileSize;
        }
    }

    public Vector3 GetSnapToPositionOffset()
    {
        if(IsActive())
        {

            return snapOffset3d;// * 2f;
        }
        else
        {
            return Vector3.zero;
        }
    }

    public bool IsOverValid()
    {
        if(tileImage.enabled)
        {
            return currentState == TileState.OverValid;
        }

        return true;
    }

    private void OnDrawGizmosSelected()
    {
        //  Show Raycasting info
        /*
        if (rect != null && tileImage.isActiveAndEnabled)
        {
            Gizmos.color = Color.red;

            Vector3 raycastOrigin = transform.position;
            //raycastOrigin.z -= 1f;
            Gizmos.DrawRay(raycastOrigin, transform.forward);
        }*/

        //  Show Tile Snapping info
        if(IsActive())
        {
            Gizmos.color = Color.magenta;

            Gizmos.DrawLine(transform.position, transform.position + snapOffset3d);
        }
    }

    public Vector2 GetTileSize()
    {
        return boxCollider.size;
    }

    public void SetTileSize(Vector2 size)
    {

    }

    public bool IsActive()
    {
        return tileImage.enabled;
    }
}
