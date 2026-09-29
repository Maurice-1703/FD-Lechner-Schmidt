using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float speed = 5f;
    [SerializeField] private Vector2 input;
    [SerializeField] private Rigidbody rb;
    
    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        //rb.MovePosition(rb.position + (Vector3)input * speed * Time.deltaTime);
        rb.AddRelativeForce(input.x * speed, 0f, input.y * speed);
    }

    private void OnMove(InputValue value)
    {
        input = value.Get<Vector2>();
    }
}