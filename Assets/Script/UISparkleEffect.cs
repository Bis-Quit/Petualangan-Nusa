using UnityEngine;

public class UISparkleEffect : MonoBehaviour
{
    [Header("Pengaturan Kedap-Kedip")]
    public float kecepatan = 2f; 
    public float ukuranTerkecil = 0.7f; 
    public float ukuranTerbesar = 1.3f; 

    private Vector3 ukuranAsli;
    private float randomOffset;

    void Start()
    {
        ukuranAsli = transform.localScale;
        randomOffset = Random.Range(0f, 100f); 
    }

    void Update()
    {
        float wave = Mathf.Sin((Time.time + randomOffset) * kecepatan);
        
        float currentScale = Mathf.Lerp(ukuranTerkecil, ukuranTerbesar, (wave + 1f) / 2f);
        
        transform.localScale = ukuranAsli * currentScale;
    }
}