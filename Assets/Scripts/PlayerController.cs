using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{   
    [Header("Input (aus PlayerInputActions)")]
    [SerializeField] private InputActionReference thrustAction; // W/S für Schub
    [SerializeField] private InputActionReference rollAction;   // A/D  für Rollen
    [SerializeField] private InputActionReference lookAction;   // Maus-Delta für Blickrichtung
    [SerializeField] private InputActionReference switchCameraAction; // C für Kamera wechseln
    [SerializeField] private GameObject thirdPersonCamera;
    [SerializeField] private GameObject cockpitCamera;

    [Header("Parameter")]
    [SerializeField] private float thrustAcceleration;
    [SerializeField] private float rollAcceleration;
    [SerializeField] private float lookSensitivity;

    [Header("Distance Display")]
    [SerializeField] private Transform distanceTarget;  // GameObject aus der Szene, zu dem die Entfernung gemessen wird
    [SerializeField] private TMP_Text distanceText;     // Text (TMP) auf dem Cockpit-Display
    [SerializeField] private string distanceLabel = "Distance: "; // Text vor der Zahl
    [SerializeField] private RectTransform compassNeedle;         // Pfeil auf dem Display, das Sprite muss nach oben zeigen

    [Header("Magazine")]
    [SerializeField] private int magazineSize = 10;       // maximale Anzahl an Patronen
    [SerializeField] private GameObject bulletIconPrefab; // UI-Image einer einzelnen Patrone
    [SerializeField] private Transform magazineContainer; // Objekt oben links mit Horizontal Layout Group


    private Rigidbody rb;
    private float thrustInput, rollInput;
    private Vector2 lookInput;
    private bool inCockpit = false;

    private int currentAmmo;                                                // aktuell verfügbare Patronen
    private readonly List<GameObject> bulletIcons = new List<GameObject>(); // ein Icon pro Patrone


    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.useGravity = false;

        // Mit vollem Magazin starten
        currentAmmo = magazineSize;
        CreateBulletIcons();
    }

    private void OnEnable()
    {
        thrustAction.action.Enable();
        rollAction.action.Enable();
        lookAction.action.Enable();
        switchCameraAction.action.Enable();

        // Bei jedem zerstörten Projektil eine Patrone zurückgeben
        BulletController.OnBulletDestroyed += RestoreBullet;

        // Cursor in der Mitte fixieren und verstecken
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void OnDisable()
    {
        thrustAction.action.Disable();
        rollAction.action.Disable();
        lookAction.action.Disable();
        switchCameraAction.action.Disable();

        // Event wieder abmelden, damit kein deaktiviertes Objekt aufgerufen wird
        BulletController.OnBulletDestroyed -= RestoreBullet;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    private void Update()
    {
        // Input auslesen
        thrustInput = thrustAction.action.ReadValue<float>();
        rollInput = rollAction.action.ReadValue<float>();
        lookInput += lookAction.action.ReadValue<Vector2>();    // Bewegung über Frames akkumulieren, da FixedUpdate seltener aufgerufen wird als Update

        // Nur im Frame reagieren, in dem C gedrückt wurde (verhindert, dass die Kamera mehrmals gewechselt wird, wenn C gehalten wird)
        if (switchCameraAction.action.WasPressedThisFrame())
            SetView(!inCockpit);
    }

    private void LateUpdate()
    {
        // Läuft nach allen Bewegungen des Frames; so zeigt das Display immer die aktuelle Entfernung
        UpdateDistanceDisplay();
        UpdateCompass();
    }

    private void FixedUpdate()
    {
        // Schub entlang der lokalen Vorwärtsachse
        Vector3 thrustForce = Vector3.forward * thrustInput * thrustAcceleration;
        rb.AddRelativeForce(thrustForce, ForceMode.Acceleration);

        // Rollen um die lokale Vorwärtsachse
        Vector3 rollTorque = Vector3.forward * (-rollInput) * rollAcceleration;
        rb.AddRelativeTorque(rollTorque, ForceMode.Acceleration);

        // Blickrichtung über den Rigidbody drehen
        float yaw = lookInput.x * lookSensitivity;
        float pitch = -lookInput.y * lookSensitivity;
        rb.MoveRotation(rb.rotation * Quaternion.Euler(pitch, yaw, 0f));

        lookInput = Vector2.zero;   // verbrauchte Bewegung zurücksetzen
    }

    private void SetView(bool toggle)
    {
        inCockpit = toggle;
        cockpitCamera.SetActive(toggle);
        thirdPersonCamera.SetActive(!toggle);
    }

    private void UpdateDistanceDisplay()
    {
        if (distanceTarget == null || distanceText == null) return;

        // 1 Unity Einheit = 1 Meter
        float distance = Vector3.Distance(transform.position, distanceTarget.position);

        // rundet auf ganze Meter, e.g. "Distance: 128 m"
        distanceText.text = $"{distanceLabel}{distance:0} m";
    }

    private void UpdateCompass()
    {
        if (distanceTarget == null || compassNeedle == null) return;

        // Position des Ziels aus Sicht des Schiffs: x = rechts, y = oben, z = vorne
        Vector3 local = transform.InverseTransformPoint(distanceTarget.position);

        // Draufsicht: Winkel zwischen "geradeaus" und dem Ziel, im Uhrzeigersinn in Grad
        float angle = Mathf.Atan2(local.x, local.z) * Mathf.Rad2Deg;

        // UI-Elemente drehen bei positivem Z gegen den Uhrzeigersinn, deshalb das Minus
        compassNeedle.localRotation = Quaternion.Euler(0f, 0f, -angle);
    }


    // ---------- Magazin ----------
    // Wird von den Kanonen vor jedem Schuss aufgerufen, gibt false zurück, wenn das Magazin leer ist
    public bool TryUseBullet()
    {
        if (currentAmmo <= 0) return false;

        currentAmmo--;
        UpdateMagazineUI();
        return true;
    }

    // Wird aufgerufen, sobald ein Projektil zerstört wurde
    private void RestoreBullet()
    {
        currentAmmo = Mathf.Min(currentAmmo + 1, magazineSize); // nie mehr als die Magazingröße
        UpdateMagazineUI();
    }

    // Erzeugt beim Start ein Icon pro Patrone
    private void CreateBulletIcons()
    {
        if (bulletIconPrefab == null || magazineContainer == null) return;

        for (int i = 0; i < magazineSize; i++)
            bulletIcons.Add(Instantiate(bulletIconPrefab, magazineContainer));

        UpdateMagazineUI();
    }

    // Zeigt genau so viele Icons an, wie Patronen verfügbar sind
    private void UpdateMagazineUI()
    {
        for (int i = 0; i < bulletIcons.Count; i++)
            bulletIcons[i].SetActive(i < currentAmmo);
    }
}