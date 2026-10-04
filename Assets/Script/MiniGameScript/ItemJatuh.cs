using UnityEngine;

public enum TipeItemJatuh 
{ 
    Makanan, 
    Bahaya, 
    Koin 
}

[RequireComponent(typeof(BoxCollider2D))]
public class ItemJatuh : MonoBehaviour
{
    [Header("Pengaturan Item")]
    public TipeItemJatuh tipeItem;
    public float kecepatanJatuh = 5f;
    public float batasBawahLayar = -10f; 

    void Start()
    {
        GetComponent<BoxCollider2D>().isTrigger = true;
    }

    void Update()
    {
        transform.Translate(Vector3.down * kecepatanJatuh * Time.deltaTime);

        if (transform.position.y < batasBawahLayar)
        {
            Destroy(gameObject);
        }
    }
}