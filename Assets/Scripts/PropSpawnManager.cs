using System;
using UnityEngine;

public class PropSpawnManager : MonoBehaviour
{
    [SerializeField]
    InventorySO inventorySO;

    [SerializeField]
    float spawnOffset = 3f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        inventorySO.spawnManager = this;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SpawnItem(ItemTypes itemType)
    {
        Vector3 playerPos = transform.position;

        playerPos.y += spawnOffset;

        GameObject newItem = Instantiate(inventorySO.GetItemDataByType(itemType).itemPropPrefab, playerPos, Quaternion.identity);
    }
}
