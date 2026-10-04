using UnityEngine;

public class AnimasiIconRotasi : MonoBehaviour
{
    [Header("Kecepatan Putar (Minus = Searah Jarum Jam)")]
    public float kecepatanPutar = -250f;

    void Update()
    {
        // Memutar objek pada sumbu Z secara terus-menerus
        transform.Rotate(0, 0, kecepatanPutar * Time.unscaledDeltaTime);
    }
}