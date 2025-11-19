using UnityEngine;

public class PropDropper : MonoBehaviour
{
    [SerializeField]
    InventorySO inventorySO;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        inventorySO.propDropperManager = this;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SpawnItem(ItemTypes itemType)
    {
        GameObject newItem = Instantiate(inventorySO.GetItemDataByType(itemType).itemPropPrefab, transform.position, Quaternion.identity);
        Vector3 randomForce = Random.onUnitSphere;
        randomForce.y = Mathf.Abs(randomForce.y);
        newItem.GetComponent<Rigidbody>().AddForce(randomForce * 5000.0f);
    }
}
