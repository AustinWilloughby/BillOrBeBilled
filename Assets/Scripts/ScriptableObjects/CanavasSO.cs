using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// Use the CreateAssetMenu attribute to allow creating instances of this ScriptableObject from the Unity Editor.
[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/CanvasScriptableObject", order = 1)]
public class CanvasSO : ScriptableObject
{
    //public RectTransform canvasRect;
    public CanvasScaler canvasScaler;

    public GraphicRaycaster raycaster;

    public InventoryManager inventoryManager;
}