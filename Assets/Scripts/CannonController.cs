using UnityEngine;
using UnityEngine.InputSystem;

public class CannonController : MonoBehaviour
{
    [Header("Input (aus PlayerInputActions)")]
    [SerializeField] private InputActionReference shootAction; // Linke Maustaste für Schießen

    [SerializeField] private GameObject projectilePrefab; // Prefab des Projektils, das abgeschossen werden soll

    [SerializeField] private Transform firePoint; // Position und Ausrichtung, von der aus das Projektil gespawnt wird

    [SerializeField] private float projectileSpeed = 30f; // Geschwindigkeit des Projektils

    private void OnEnable()
    {
        shootAction.action.Enable();
    }

    private void OnDisable()
    {
        shootAction.action.Disable();
    }

    private void Update()
    {
        // Prüft, ob die Schießen-Aktion ausgelöst wurde
        if (shootAction.action.triggered)
        {
            Shoot();
        }
    }

    private void Shoot()
    {
        // Erstellt ein neues Projektil an der Position des FirePoints
        GameObject projectile = Instantiate(
            projectilePrefab,
            firePoint.position,
            firePoint.rotation
        );

        // Holt die Rigidbody-Komponente des Projektils
        Rigidbody rb = projectile.GetComponent<Rigidbody>();

        // Falls ein Rigidbody vorhanden ist, wird das Projektil nach vorne bewegt
        if (rb != null)
        {
            rb.linearVelocity = firePoint.forward * projectileSpeed;
        }
    }
}