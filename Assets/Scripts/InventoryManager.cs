using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventoryManager : MonoBehaviour
{
    [SerializeField]
    CanvasSO canvasSO;

    [SerializeField]
    Canvas canvas;

    [SerializeField]
    GraphicRaycaster raycaster;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        canvasSO.canvas = canvas;

        canvasSO.raycaster = raycaster;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
