using System.Collections;
using UnityEngine;

public class BulletController : MonoBehaviour
{
    [SerializeField] private float lifetime = 5f;

    public static event System.Action<GameObject> OnBulletHit;
    public static event System.Action OnBulletDestroyed;            // meldet, dass ein Projektil verschwunden ist (Munition zurückgeben)
    private bool isDestroyed = false;                               // verhindert, dass ein Projektil doppelt zählt, wenn es im selben Frame zwei Objekte berührt

    private void Start()
    {
        StartCoroutine(DestroyAfterTime());
    }

    private IEnumerator DestroyAfterTime()
    {
        yield return new WaitForSeconds(lifetime);

        Destroy(gameObject);
    }

    private void OnCollisionEnter(Collision collision)
    {
        // Mehrere Kollisionen im selben Frame ignorieren
        if (isDestroyed) return;

        OnBulletHit?.Invoke(collision.gameObject);

        DestroyBullet();
    }

    private void DestroyBullet()
    {
        if (isDestroyed) return;
        isDestroyed = true;

        // Dem Magazin melden, dass eine Patrone zurückgegeben werden kann
        OnBulletDestroyed?.Invoke();

        Destroy(gameObject);
    }
}