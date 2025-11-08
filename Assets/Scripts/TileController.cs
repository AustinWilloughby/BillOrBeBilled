using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum TileState
{
    None,
    OverValid,
    OverInvalid
}

[RequireComponent(typeof(BoxCollider2D))]
public class TileController : MonoBehaviour
{
    [SerializeField]
    Image tileImage;

    [SerializeField]
    BoxCollider2D tileCollider;

    [SerializeField]
    bool isActive = true;

    [SerializeField]
    TileState currentState;

    List<Collider2D> overColliders = new List<Collider2D>();

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

        tileCollider.enabled = value;
    }

    public void OverTileCheck()
    {
        Physics2D.OverlapPoint(transform.position, ContactFilter2D.noFilter, overColliders);
        //overColliders = Physics2D.OverlapPointAll(transform.position);

        Debug.Log(overColliders.Count);

        if (overColliders.Count > 1)
        {
            currentState = TileState.OverValid;
        }
        else
        {
            currentState = TileState.OverInvalid;
        }
    }    
}
