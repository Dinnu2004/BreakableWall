using UnityEngine;

public class Breakable : MonoBehaviour
{
    [SerializeField] GameObject burstPrefab;
    bool isBroken;

    public void Break()
    {
        if (isBroken) return;
        isBroken = true;
        DashVfxAudio.Burst(transform.position, new Color(1f, .3f, .05f));
        if (burstPrefab != null) Instantiate(burstPrefab, transform.position + Vector3.up, Quaternion.identity);
        Destroy(gameObject);
    }
}