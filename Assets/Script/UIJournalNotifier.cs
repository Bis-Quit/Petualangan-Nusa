using UnityEngine;
using System.Collections;

public class UIJournalNotifier : MonoBehaviour
{
    [Header("Referensi UI")]
    [Tooltip("Masukkan target gambar ikon bukunya")]
    public RectTransform ikonBuku;
    [Tooltip("Masukkan objek gambar bintik merah/notif")]
    public GameObject bintikMerah;

    [Header("Pengaturan Animasi")]
    public float durasiGoyang = 0.8f;
    public float jedaAntarGoyang = 2.5f;
    public float intensitasRotasi = 12f;

    private Coroutine animasiCoroutine;

    void Start()
    {
        CekNotifikasi();
    }

    public void CekNotifikasi()
    {
        // Cek apakah ada pusaka baru dari PlayerPrefs (default 0)
        int adaBaru = PlayerPrefs.GetInt("AdaPusakaBaru", 0);

        if (adaBaru == 1)
        {
            if (bintikMerah != null) bintikMerah.SetActive(true);
            
            if (animasiCoroutine != null) StopCoroutine(animasiCoroutine);
            animasiCoroutine = StartCoroutine(AnimasiManggilManggil());
        }
        else
        {
            // Matikan efek kalau nggak ada barang baru
            if (bintikMerah != null) bintikMerah.SetActive(false);
            if (animasiCoroutine != null) StopCoroutine(animasiCoroutine);
            if (ikonBuku != null) ikonBuku.localRotation = Quaternion.identity;
        }
    }

    private IEnumerator AnimasiManggilManggil()
    {
        if (ikonBuku == null) yield break;

        // Loop terus-menerus selama scene map terbuka dan notif belum di-klik
        while (true)
        {
            float waktu = 0f;
            
            // Gerakan goyang kiri-kanan ala lonceng notifikasi
            while (waktu < durasiGoyang)
            {
                waktu += Time.deltaTime;
                
                // Rumus ayunan yang makin lama makin pelan (mereda di akhir)
                float sisaKekuatan = 1f - (waktu / durasiGoyang);
                float zRot = Mathf.Sin(waktu * 30f) * intensitasRotasi * sisaKekuatan;
                
                ikonBuku.localRotation = Quaternion.Euler(0, 0, zRot);
                yield return null;
            }

            ikonBuku.localRotation = Quaternion.identity;

            // Istirahat sebentar sebelum caper (goyang) lagi
            yield return new WaitForSeconds(jedaAntarGoyang);
        }
    }

    // FUNGSI INI DIPANGGIL SAAT TOMBOL BUKU DIKLIK
    public void HapusNotifikasi()
    {
        PlayerPrefs.SetInt("AdaPusakaBaru", 0);
        PlayerPrefs.Save();
        CekNotifikasi();
    }
}