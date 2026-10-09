using System.Collections;
using UnityEngine;

public class BulletController : MonoBehaviour
{
    [SerializeField] private float lifetime = 5f;

    public static event System.Action<GameObject> OnBulletHit;

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
        OnBulletHit?.Invoke(collision.gameObject);

        Destroy(gameObject);
    }
}