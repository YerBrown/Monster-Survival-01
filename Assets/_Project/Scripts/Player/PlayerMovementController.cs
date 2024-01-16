using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class PlayerMovementController : MonoBehaviour
{
    public float movementSpeed = 1f;
    private Rigidbody2D _rb;
    void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
    }
    void FixedUpdate()
    {
        Vector2 currentPos = _rb.position;
        float horizontalInput = Input.GetAxisRaw("Horizontal");
        float verticalInput = Input.GetAxisRaw("Vertical");
        Vector2 inputVector = new Vector2(horizontalInput, verticalInput).normalized;
        Vector2 movement = inputVector * movementSpeed;
        Vector2 newPos = currentPos + movement * Time.fixedDeltaTime;
        _rb.MovePosition(newPos);
    }

    public void TestEvent()
    {
        Debug.Log("Event raised received");
    }
}
