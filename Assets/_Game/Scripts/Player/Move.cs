using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Move : MonoBehaviour
{
    private PlayerController controller;

    private Vector2 direction;

    [SerializeField] float accelaration = 30f;

    // Start is called before the first frame update
    private void Awake()
    {
        controller = GetComponent<PlayerController>();
    }

    void Start()
    {
        controller.Controls.Game.Move.performed += ctx => direction = ctx.ReadValue<Vector2>();
    }

    private void Update()
    {
        UpdateAnimation();
    }

    // Update is called once per frame
    void FixedUpdate()
    {      
        Movement();
    }

    private void UpdateAnimation()
    {
        controller.Animator.SetFloat("x", direction.x);
        controller.Animator.SetFloat("y", direction.y);
        controller.Animator.SetFloat("magnitude", direction.sqrMagnitude);
    }

    private void Movement()
    {
        var rb = controller.Rigidbody2D;
        rb.MovePosition(rb.position + direction.normalized * accelaration * Time.fixedDeltaTime);
    }
}
