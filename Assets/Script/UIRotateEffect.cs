using UnityEngine;

public class UIRotateEffect : MonoBehaviour
{
    public float kecepatanRotasi = -45f; 

    void Update()
    {
        transform.Rotate(0, 0, kecepatanRotasi * Time.deltaTime);
    }
}