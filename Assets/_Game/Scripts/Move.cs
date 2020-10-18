using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Move : MonoBehaviour
{
    private PlayerController controller;

    private Vector2 direction;

    [SerializeField] float accelaration = 30f;
    [SerializeField] float maxSpeed = 4f;

    // Start is called before the first frame update
    private void Awake()
    {
        controller = GetComponent<PlayerController>();
    }

    void Start()
    {
        controller.Controls.Game.Move.performed += ctx => direction = ctx.ReadValue<Vector2>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        UpdateDirection();
        Movement();
    }

    private void UpdateDirection()
    {
        // player is in movement
        if (direction.x != 0 || direction.y != 0)
        {
            if (direction.x > 0)
            {
                
            }
            if (direction.x < 0)
            {
                
            }
            if (direction.y > 0)
            {
                
            }
            if (direction.y < 0)
            {
                
            }
        }
        else
        {

        }
    }

    private void Movement()
    {
        var velocity = controller.Rigidbody2D.velocity;
        velocity += direction * accelaration * Time.fixedDeltaTime;

        direction = Vector2.zero;

        velocity.x = Mathf.Clamp(velocity.x, -maxSpeed, maxSpeed);
        velocity.y = Mathf.Clamp(velocity.y, -maxSpeed, maxSpeed);

        controller.Rigidbody2D.velocity = velocity;
    }
}
