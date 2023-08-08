using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    private CustomInput _customInput; // This is the auto generated class from the Input Actions

    private Rigidbody2D _rigidbody2D;
    private Vector2 moveDirection;

    private void Awake()
    {
        _customInput = new CustomInput(); // Need to initialize this script
        _rigidbody2D = GetComponent<Rigidbody2D>();
    }

    private void OnEnable()
    {
        _customInput.Enable();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        ReadMovementInput();
        MovePlayer();
    }

    private void OnDisable()
    {
        _customInput.Disable();
    }
    
    private void ReadMovementInput()
    { 
        moveDirection = _customInput.Player.Movement.ReadValue<Vector2>();
    }

    private void MovePlayer()
    {
        _rigidbody2D.velocity = moveDirection * 5;
    }
}
