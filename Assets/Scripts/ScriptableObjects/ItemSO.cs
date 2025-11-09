using UnityEngine;

// Use the CreateAssetMenu attribute to allow creating instances of this ScriptableObject from the Unity Editor.
[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/ItemScriptableObject", order = 1)]
public class ItemSO : ScriptableObject
{
    public GameObject itemPropPrefab;

    public RectTransform itemUIPrefab;

    public ItemTypes itemType;

    public float itemSellValue;

    public float itemEnergyCost;

    public Texture itemTexture;
}