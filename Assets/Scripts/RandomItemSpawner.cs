using UnityEngine;

public class RandomItemSpawner : MonoBehaviour
{
    [SerializeField] InventorySO inventorySO;
    [SerializeField] Vector2 worldBounds = new Vector2(400, 400);
    [SerializeField] float numItems = 100;

    private void Start()
    {
        for(int i = 0; i < numItems; i++)
        {
            Vector3 randPos = new Vector3(
                Random.Range(-worldBounds.x, worldBounds.x),
                150,
                Random.Range(-worldBounds.y, worldBounds.y));
            Instantiate(inventorySO.GetRandomItem().itemPropPrefab, randPos, Random.rotationUniform);
        }
    }
}