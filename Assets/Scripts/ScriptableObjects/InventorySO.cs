using System.Collections.Generic;
using UnityEngine;

public enum ItemTypes
{
    Cone,
    Container,
    Fan,
    Gear,
    Gem,
    Gyro,
    Montherboard,
    Rock,
    Scrap,
    Sphere,
    Sword,
    Tube
}

// Use the CreateAssetMenu attribute to allow creating instances of this ScriptableObject from the Unity Editor.
[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/InventoryScriptableObject", order = 1)]
public class InventorySO : ScriptableObject
{
    public List<ItemSO> items;

    public InventoryManager uiInventoryManager;

    public PropDropper propDropperManager;

    public ItemSO GetItemDataByType(ItemTypes itemType)
    {
        foreach (ItemSO item in items)
        {
            if(item.itemType == itemType)
            {
                return item;
            }
        }

        return null;
    }

    public ItemSO GetRandomItem()
    {
        int rand = Random.Range(0, items.Count);

        return items[rand];
    }
}