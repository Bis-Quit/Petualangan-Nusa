using UnityEngine;
using TMPro;
using System.Collections;

[RequireComponent(typeof(CanvasGroup))]
public class TooltipManager : MonoBehaviour
{
    public static TooltipManager Instance;
    
    [Header("Referensi UI")]
    public TextMeshProUGUI teksNamaPusaka;
    
    [Header("Pengaturan")]
    public float jarakMelayang = 80f; // Jarak Y agar tooltip tidak menutupi jari/kursor

    private CanvasGroup canvasGroup;

    void Awake()
    {
        Instance = this;
        canvasGroup = GetComponent<CanvasGroup>();
        
        // Pastikan tooltip tersembunyi saat game baru mulai
        canvasGroup.alpha = 0f;
        gameObject.SetActive(false);
    }

    public void TampilkanTooltip(string namaItem, Vector3 posisiSlot)
    {
        gameObject.SetActive(true);
        teksNamaPusaka.text = namaItem;
        
        // Pindahkan tooltip persis ke atas slot siluet yang diklik
        transform.position = posisiSlot + new Vector3(0, jarakMelayang, 0);

        // Hentikan animasi fade sebelumnya jika pemain klik barang lain dengan cepat
        StopAllCoroutines();
        StartCoroutine(AnimasiFade());
    }

    private IEnumerator AnimasiFade()
    {
        // Muncul secara instan
        canvasGroup.alpha = 1f;
        
        // Jeda waktu membaca (2 detik)
        yield return new WaitForSeconds(2f);
        
        // Animasi memudar perlahan
        float durasi = 0.5f;
        float waktu = 0f;
        while (waktu < durasi)
        {
            waktu += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(1f, 0f, waktu / durasi);
            yield return null;
        }
        
        // Matikan objek setelah benar-benar transparan
        gameObject.SetActive(false);
    }
}