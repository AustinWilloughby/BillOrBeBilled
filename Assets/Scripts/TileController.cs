using UnityEngine;
using UnityEngine.UI;

public class TileController : MonoBehaviour
{
    [SerializeField]
    Image tileImage;

    [SerializeField]
    BoxCollider2D tileCollider;

    [SerializeField]
    bool isActive = true;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        SetStateTo(isActive);
    }

    private void OnEnable()
    {
        SetStateTo(isActive);
    }

    void SetStateTo(bool value)
    {
        tileImage.enabled = value;

        tileCollider.enabled = value;
    }
}
