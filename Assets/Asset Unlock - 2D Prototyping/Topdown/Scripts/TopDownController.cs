﻿using System;
using UnityEngine;

public class TopDownController : MonoBehaviour
{
    // ========= MOVEMENT =================
    public float speed = 0.04f;

    // =========== MOVEMENT ==============
    Rigidbody2D rigidbody2d;
    private Vector2 movement;
    private Vector2 newPosition;

    // ==== ANIMATION =====
    Animator animator;
    Vector2 lookDirection = new Vector2(1, 0);

    void Start()
    {
        // =========== MOVEMENT ==============
        rigidbody2d = GetComponent<Rigidbody2D>();

        // ==== ANIMATION =====
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        // ============== MOVEMENT ======================
        if (newPosition != null) movement = new Vector2(newPosition.x - transform.position.x, newPosition.y - transform.position.y);

        if (!Mathf.Approximately(movement.x, 0.0f) || !Mathf.Approximately(movement.y, 0.0f))
        {
            lookDirection.Set(movement.x, movement.y);
            lookDirection.Normalize();
        }

        // ============== ANIMATION =======================

        animator.SetFloat("Look X", lookDirection.x);
        animator.SetFloat("Look Y", lookDirection.y);
        animator.SetFloat("Speed", movement.magnitude);
    }

    private void FixedUpdate()
    {
        Vector2 position = rigidbody2d.position;

        position = position + speed * Time.deltaTime * movement;

        rigidbody2d.MovePosition(position);

    }

    public void SetMovement(Vector3 newPosition)
    {
        this.newPosition = new Vector2(newPosition.x, newPosition.y);
    }
}
