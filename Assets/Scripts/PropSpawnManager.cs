using System;
using UnityEngine;

public class PropSpawnManager : MonoBehaviour
{
    [SerializeField]
    InventorySO inventorySO;

    [SerializeField]
    Terrain terrain;

    [SerializeField]
    int spawnCount;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        inventorySO.spawnManager = this;

        for (int i = 0; i < spawnCount; i++)
        {
            SpawnItem(inventorySO.GetRandomItem().itemType);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SpawnItem(ItemTypes itemType)
    {
        GameObject newItem = Instantiate(inventorySO.GetItemDataByType(itemType).itemPropPrefab);
    }
}
