using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class InventoryManager : MonoBehaviour
{
    [SerializeField]
    CanvasSO canvasSO;

    [SerializeField]
    Canvas canvas;

    [SerializeField]
    GraphicRaycaster raycaster;

    public RectTransform rect, pickedUpRect, storageRect, heldItemsRect;

    public ItemController activeItem;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        canvasSO.canvas = canvas;

        canvasSO.raycaster = raycaster;

        canvasSO.inventoryManager = this;
    }

    // Update is called once per frame
    void Update()
    {
        
    }



    public void OnRotate(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            Vector2 playerInput = context.ReadValue<Vector2>();

            float newRot = playerInput.x;

            if (newRot > 0)
            {
                newRot = 90f;
            }
            else if (newRot < 0)
            {
                newRot = -90f;
            }

            //Debug.Log(newRot);

            activeItem.AddRotation(newRot);
        }
    }
}
