using UnityEngine;

public class SpawnerKoper : MonoBehaviour
{
    public GameObject prefabItemPusaka;
    public Transform wadahPanelItemPusaka;

    void Start()
    {
        // Baca isi tas ransel
        foreach(ItemDataSO data in InventoryPemain.pusakaTerkumpul)
        {
            // Munculkan cetakan UI barang baru di meja
            GameObject barangBaru = Instantiate(prefabItemPusaka, wadahPanelItemPusaka);
            
            // Suntikkan data SO-nya
            UIDragItem komponenDrag = barangBaru.GetComponent<UIDragItem>();
            if (komponenDrag != null)
            {
                komponenDrag.dataPusaka = data;
            }
        }
    }
}