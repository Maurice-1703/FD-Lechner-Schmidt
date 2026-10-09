using UnityEngine;

public class MessageLogger : MonoBehaviour
{
    private void OnEnable()
    {
        BulletController.OnBulletHit += LogHit;
    }

    private void OnDisable()
    {
        BulletController.OnBulletHit -= LogHit;
    }

    private void LogHit(GameObject hitObject)
    {
        Debug.Log("Projectile hit: " + hitObject.name);
    }
}