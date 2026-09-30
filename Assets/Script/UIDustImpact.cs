using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UIDustImpact : MonoBehaviour
{
    private Image gambarDebu;
    private RectTransform rect;
    private Vector3 ukuranAsli; // Variabel penyimpan ukuran dari Editor

    void Awake()
    {
        gambarDebu = GetComponent<Image>();
        rect = GetComponent<RectTransform>();
        
        // Rekam ukuran yang lu set di Unity Editor
        ukuranAsli = rect.localScale;

        if (gambarDebu != null)
        {
            Color warna = gambarDebu.color;
            warna.a = 0f;
            gambarDebu.color = warna;
        }
    }

    public void LedakkanDebu()
    {
        StopAllCoroutines();
        StartCoroutine(AnimasiDebu());
    }

    private IEnumerator AnimasiDebu()
    {
        float waktu = 0f;
        float durasi = 0.5f;

        // Skala awal dibikin pipih berdasarkan ukuran asli
        Vector3 skalaAwal = new Vector3(ukuranAsli.x * 0.3f, ukuranAsli.y * 0.1f, ukuranAsli.z);
        // Skala akhir kembali persis ke ukuran asli di Editor
        Vector3 skalaAkhir = ukuranAsli;

        while (waktu < durasi)
        {
            waktu += Time.unscaledDeltaTime;
            float persen = waktu / durasi;
            
            float kurvaEaseOut = 1f - Mathf.Pow(1f - persen, 3f); 

            rect.localScale = Vector3.Lerp(skalaAwal, skalaAkhir, kurvaEaseOut);

            if (gambarDebu != null)
            {
                Color warna = gambarDebu.color;
                if (persen < 0.15f) {
                    warna.a = Mathf.Lerp(0f, 0.7f, persen / 0.15f); 
                } else {
                    warna.a = Mathf.Lerp(0.7f, 0f, (persen - 0.15f) / 0.85f); 
                }
                gambarDebu.color = warna;
            }

            yield return null;
        }
    }
}