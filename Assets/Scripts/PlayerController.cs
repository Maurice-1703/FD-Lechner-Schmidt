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

    
    private Rigidbody rb;
    private float thrustInput, rollInput;
    private Vector2 lookInput;
    private bool inCockpit = false;


    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.useGravity = false;
    }

    private void OnEnable()
    {
        thrustAction.action.Enable();
        rollAction.action.Enable();
        lookAction.action.Enable();
        switchCameraAction.action.Enable();

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
}