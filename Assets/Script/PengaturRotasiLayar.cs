using UnityEngine;

public class PengaturRotasiLayar : MonoBehaviour
{
    [Header("Pengaturan Orientasi Layar")]
    [Tooltip("Pilih posisi layar yang dimau saat scene ini dibuka")]
    public ScreenOrientation targetOrientasi = ScreenOrientation.LandscapeLeft;

    [Tooltip("Centang jika ingin layar Landscape bisa dibolak-balik otomatis (kiri/kanan)")]
    public bool aktifkanAutoRotateLandscape = true;

    void Awake()
    {
        // Paksa orientasi sesuai target
        Screen.orientation = targetOrientasi;

        // Kalau butuh auto-rotate khusus landscape
        if (aktifkanAutoRotateLandscape && 
           (targetOrientasi == ScreenOrientation.LandscapeLeft || targetOrientasi == ScreenOrientation.LandscapeRight || targetOrientasi == ScreenOrientation.AutoRotation))
        {
            Screen.orientation = ScreenOrientation.AutoRotation;
            Screen.autorotateToLandscapeLeft = true;
            Screen.autorotateToLandscapeRight = true;
            Screen.autorotateToPortrait = false;
            Screen.autorotateToPortraitUpsideDown = false;
        }
    }
}