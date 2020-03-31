using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public int maxJumpCount;
    public float horizontalSpeed;
    public float jumpSpeed;
    public bool isGround,jumpPressed;
    public Transform groundCheck;
    public float groundCheckRadius;
    public string groundLayerName;
    
    private Rigidbody2D _rb2d;
    private Animator _animator;
    private int _jumpCount;
    private LayerMask _groundLayer;
    private static readonly int Running = Animator.StringToHash("running");
    private static readonly int Jumping = Animator.StringToHash("jumping");
    private static readonly int Falling = Animator.StringToHash("falling");

    void Start()
    {
        _rb2d = GetComponent<Rigidbody2D>();
        _groundLayer = LayerMask.GetMask(groundLayerName);
        _jumpCount = maxJumpCount;
        _animator = GetComponent<Animator>();
    }
    
    void Update()
    {
        // check jump button input,then control the performance
        // of rigid body 2d in FixedUpdate()
        if (Input.GetButtonDown("Jump") && _jumpCount > 0)
        {
            jumpPressed = true;
        }
    }

    private void FixedUpdate()
    {
        // check ground
        isGround = Physics2D.OverlapCircle(
            groundCheck.position, 
            groundCheckRadius, 
            _groundLayer
        );
        Move();
        Jump();
        SwitchAnimation();
    }

    private void Move()
    {
        if (!isGround) return;
        var horizontal = Input.GetAxisRaw("Horizontal");
        _rb2d.velocity = new Vector2(horizontalSpeed * horizontal, _rb2d.velocity.y);
        // facing towards input axis
        if (Math.Abs(horizontal) > 0)
        {
            transform.localScale = new Vector3(horizontal, 1, 1);
        }
    }

    private void Jump()
    {
        // reset jump count while landing
        if (isGround)
        {
            _jumpCount = maxJumpCount;
        }
        
        if (jumpPressed)
        {
            _rb2d.velocity = new Vector2(_rb2d.velocity.x, jumpSpeed);
            _jumpCount--;
            jumpPressed = false;
        }
    }
    
    /**
     * there is four state with player:
     * 1) idle 2) running 3) jumping 4) falling
     */
    private void SwitchAnimation()
    {
        if (isGround)
        {
            // player is idle or running depends on parameter named "running"
            _animator.SetFloat(Running, Mathf.Abs(_rb2d.velocity.x));
            _animator.SetBool(Falling, false);
        }
        else
        {
            if (_rb2d.velocity.y > 0)
            {
                // player is jumping
                _animator.SetBool(Jumping, true);
            }
            else
            {
                // player is falling
                _animator.SetBool(Jumping, false);
                _animator.SetBool(Falling, true);    
            }
        }
    }
}
