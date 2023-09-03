using UnityEngine;
using UnityEngine.Tilemaps;
using Rogue.System;

namespace Rogue.Player {
  // Required components to run this script
  [RequireComponent(typeof(ActionScheduler))]
  [RequireComponent(typeof(Animator))]
  [RequireComponent(typeof(Rigidbody2D))]
  [RequireComponent(typeof(PlayerAttributes))]

  // Main script
  public class PlayerMovement : MonoBehaviour, IAction {
    [Header("Ground")]
    [SerializeField] private LayerMask _groundLayerMask;
    [SerializeField] private Transform _groundCheck;

    [Header("Wall")]
    [SerializeField] private LayerMask _wallLayerMask;
    [SerializeField] private Transform _wallCheck;

    private ActionScheduler _scheduler;
    private Animator _animator;
    private Rigidbody2D _rigidbody;
    private PlayerAttributes _attributes;

    private float _movementDirection;
    private bool _facingRight = true;
    private bool _onGround = false;
    private bool _onWall = false;
    private bool _canDoubleJump = false;

    private void Start() {
      _scheduler = GetComponent<ActionScheduler>();
      _animator = GetComponent<Animator>();
      _rigidbody = GetComponent<Rigidbody2D>();
      _attributes = GetComponent<PlayerAttributes>();
    }

    private void Update() {
      if (_attributes.dead) {
        return;
      }

      // Check on ground and on wall
      CheckOnGround();
      CheckOnWall();

      // Traverse through platform
      TraversePlatform();

      // You can't move when you are doing other action than movement
      if (!_animator.GetCurrentAnimatorStateInfo(0).IsTag("Movement")) {
        return;
      }

      // Get movement axis
      _movementDirection = Input.GetAxisRaw("Horizontal");

      // Start movement action
      if (_rigidbody.velocity != Vector2.zero) {
        _scheduler.CommenceAction(this);
      }

      FlipBehaviour();
      Jump();
    }

    private void FixedUpdate() {
      // You can't move when you are doing other action than movement
      if (!_animator.GetCurrentAnimatorStateInfo(0).IsTag("Movement")) {
        return;
      }

      Move();
    }

    private void CheckOnGround() {
      bool wasGround = _onGround;
      _onGround = false;
      Physics2D.IgnoreLayerCollision(7, 21, false);

      Collider2D[] colliders = Physics2D.OverlapCircleAll(
        _groundCheck.position,
        .2f,
        _groundLayerMask
      );

      for (int i = 0; i < colliders.Length; i++) {
        if (colliders[i].gameObject != gameObject) {
          _onGround = true;
        }
      }
    }

    private void CheckOnWall() {
      bool wasOnWall = _onWall;
      _onWall = false;

      Collider2D[] colliders = Physics2D.OverlapBoxAll(
        _wallCheck.position,
        new Vector2(1f, 2f),
        0f,
        _wallLayerMask
      );

      for (int i = 0; i < colliders.Length; i++) {
        if (colliders[i].gameObject != gameObject) {
          _onWall = true;
        }
      }
    }

    private void Move() {
      // Trigger running animation depending on movement
      _animator.SetFloat("Movement", _movementDirection);

      if (!_onWall)
      {
        // Moves the character
        Vector2 targetVelocity = new Vector2(
          _movementDirection * _attributes.movementSpeed.GetValue(),
          _rigidbody.velocity.y
        );

        _rigidbody.velocity = targetVelocity;
      }
    }

    private void Jump()
    {
      // The character jumps
      if (Input.GetButtonDown("Jump")) {
        if (_onGround) {
          _rigidbody.velocity = Vector2.up * _attributes.jumpForce.GetValue();
          _canDoubleJump = true;
        } else if (_canDoubleJump) {
          _rigidbody.velocity = Vector2.up * _attributes.jumpForce.GetValue();
          _canDoubleJump = false;
        }
      }
    }

    private void TraversePlatform() {
      if (Input.GetButtonDown("Vertical") && Input.GetAxisRaw("Vertical") < 0) {
        Physics2D.IgnoreLayerCollision(7, 21, true);
      }
    }

    private void FlipBehaviour() {
      if (_movementDirection < 0 && _facingRight) {
        FlipCharacter();
      }
      if (_movementDirection > 0 && !_facingRight) {
        FlipCharacter();
      }
    }

    private void FlipCharacter() {
      // Switch the way the player is labelled as facing.
      _facingRight = !_facingRight;

      // Multiply the player's x local scale by -1.
      Vector3 theScale = transform.localScale;
      theScale.x *= -1;
      transform.localScale = theScale;
    }

    public void Abort() {
      _rigidbody.velocity = Vector2.zero;
    }
  }
}
