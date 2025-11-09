using UnityEngine;

public class TaskGiver : MonoBehaviour
{
    [SerializeField] InventorySO inventorySO;
    [SerializeField] TextMesh text;
    [SerializeField] GameObject itemDisplayQuad;
    [SerializeField] Vector2 quadMaxDimensions = new Vector2(3, 2);
    //CurrentItem they are looking for
    ItemSO currentItem;
    [SerializeField] string[] starterPhrases;

    MeshRenderer mRenderer;
    Quaternion baseRotation;

    private void Start()
    {
        baseRotation = itemDisplayQuad.transform.localRotation;
        mRenderer = itemDisplayQuad.GetComponent<MeshRenderer>();
        GenerateTask();
    }

    private void DisplayImage(Texture displayImage)
    {
        float aspect = displayImage.width / (float)displayImage.height;
        if (aspect < 1.0f)
        {
            Vector3 newRot = baseRotation.eulerAngles;
            newRot.z = -90f;
            itemDisplayQuad.transform.rotation = Quaternion.Euler(newRot);
            itemDisplayQuad.transform.localScale
                = new Vector3(quadMaxDimensions.x * aspect, quadMaxDimensions.x, 1);
        }
        else
        {
            itemDisplayQuad.transform.localRotation = baseRotation;
            itemDisplayQuad.transform.localScale
                = new Vector3(quadMaxDimensions.x, quadMaxDimensions.x / aspect, 1);
        }

        if (mRenderer != null && displayImage != null)
        {
            mRenderer.material.mainTexture = displayImage;
        }
    }

    private void GenerateTask()
    { 
        currentItem = inventorySO.GetRandomItem();
        text.text = starterPhrases[Random.Range(0, starterPhrases.Length - 1)]
            + " " + currentItem.name + "!";
        //Assign a random item to get
    }

    private void OnTriggerEnter(Collider other)
    {
        //Check for item in inventory
        //Award money
        //Add text
        //give new task
        GenerateTask();

        //if no item, give task
    }
}
