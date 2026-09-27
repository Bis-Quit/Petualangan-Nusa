using UnityEngine;

[CreateAssetMenu(fileName = "DataPusakaBaru", menuName = "Inventory/Item Pusaka")]
public class ItemDataSO : ScriptableObject
{
    [Header("Identitas Barang")]
    public string namaItem;
    public Sprite gambarItem;

    [Header("Ukuran Grid")]
    public int lebar = 1;  // X (Ke samping)
    public int tinggi = 1; // Y (Ke bawah)
}