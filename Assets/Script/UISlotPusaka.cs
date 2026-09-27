using UnityEngine;
using UnityEngine.EventSystems;

public class UISlotPusaka : MonoBehaviour, IPointerClickHandler
{
    [HideInInspector] 
    public string namaPusaka = "Nama Item"; 

    // Fungsi ini otomatis terpanggil saat pemain mengeklik/tap UI ini
    public void OnPointerClick(PointerEventData eventData)
    {
        if (TooltipManager.Instance != null)
        {
            // Panggil fungsi tampilkan tooltip dengan memberikan nama dan posisinya
            TooltipManager.Instance.TampilkanTooltip(namaPusaka, transform.position);
        }
    }
}