using UnityEngine;
using System.Collections;

public class SpawnerManager : MonoBehaviour
{
    [Header("Daftar Barang")]
    public GameObject[] makananPrefabs;
    public GameObject[] bahayaPrefabs;
    public GameObject[] koinPrefabs; // Slot untuk koin
    
    [Header("Pengaturan Area Muncul")]
    public float batasKiri = -2.5f;
    public float batasKanan = 2.5f;
    public float posisiY = 7f; 
    
    [Header("Pengaturan Waktu")]
    public float waktuSpawnAwal = 1.5f;
    public float batasWaktuTercepat = 0.5f; 
    
    private float jedaSpawn;
    private bool isGameAktif = false;
    private Coroutine spawnCoroutine;

    public void MulaiSpawner()
    {
        jedaSpawn = waktuSpawnAwal;
        isGameAktif = true;
        spawnCoroutine = StartCoroutine(SpawnRutinitas());
    }

    public void HentikanSpawner()
    {
        isGameAktif = false;
        if (spawnCoroutine != null) StopCoroutine(spawnCoroutine);
    }

    IEnumerator SpawnRutinitas()
    {
        while (isGameAktif)
        {
            SpawnBarang();
            yield return new WaitForSeconds(jedaSpawn);
            
            if (jedaSpawn > batasWaktuTercepat) jedaSpawn -= 0.02f;
        }
    }

    void SpawnBarang()
    {
        float peluang = Random.value;
        GameObject prefabPilihan = null;

        // 60% Makanan, 25% Bahaya, 15% Koin
        if (peluang < 0.60f && makananPrefabs.Length > 0)
        {
            prefabPilihan = makananPrefabs[Random.Range(0, makananPrefabs.Length)];
        }
        else if (peluang < 0.85f && bahayaPrefabs.Length > 0)
        {
            prefabPilihan = bahayaPrefabs[Random.Range(0, bahayaPrefabs.Length)];
        }
        else if (koinPrefabs.Length > 0)
        {
            prefabPilihan = koinPrefabs[Random.Range(0, koinPrefabs.Length)];
        }

        if (prefabPilihan != null)
        {
            float randomX = Random.Range(batasKiri, batasKanan);
            Instantiate(prefabPilihan, new Vector3(randomX, posisiY, 0), Quaternion.identity);
        }
    }
}