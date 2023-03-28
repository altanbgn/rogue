using UnityEngine;
using Rogue.Player;
using Rogue.System;

namespace Rogue.Enemy
{
  // Required components to run this script
  [RequireComponent(typeof(Rigidbody2D))]
  [RequireComponent(typeof(ActionScheduler))]
  [RequireComponent(typeof(Animator))]
  [RequireComponent(typeof(EnemyAttributes))]

  public class EnemyMovement : MonoBehaviour, IAction
  {
    [HideInInspector] public bool inAttackRange = false;
    [HideInInspector] public float cooldownCounter = 0f;

    [SerializeField] private LayerMask _searchLayer;
    [SerializeField] private Transform _searchCheck;
    [SerializeField] private LayerMask _edgeLayer;
    [SerializeField] private Transform _edgeCheck;
    private ActionScheduler _scheduler;
    private Rigidbody2D _rigidbody;
    private Animator _animator;
    private EnemyAttributes _attributes;

    private float _movementDirection = 1f;
    private bool _facingRight = true;
    private GameObject _target = null;

    private void Start()
    {
      _scheduler = GetComponent<ActionScheduler>();
      _rigidbody = GetComponent<Rigidbody2D>();
      _animator = GetComponent<Animator>();
      _attributes = GetComponent<EnemyAttributes>();
    }

    private void Update()
    {
      if (_attributes.dead)
        return;

      cooldownCounter += Time.deltaTime;

      FlipBehaviour();

      if (_animator.GetCurrentAnimatorStateInfo(0).IsTag("Attack"))
        return;

      SearchTarget();

      // Trigger running animation depending on movement
      _animator.SetFloat("Movement", _rigidbody.velocity.x);

      if (cooldownCounter > _attributes.attackCooldown)
        Chase();
    }

    private void SearchTarget()
    {
      _target = null;

      Collider2D[] colliders = Physics2D.OverlapBoxAll(_searchCheck.position, new Vector2(_attributes.vision, 1.5f), 0f, _searchLayer);

      for (int i = 0; i < colliders.Length; i++)
      {
        if (colliders[i].gameObject != gameObject && colliders[i].gameObject.CompareTag("Player"))
        {
          PlayerAttributes p_target = colliders[i].gameObject.GetComponent<PlayerAttributes>();

          if (p_target == null)
            return;

          if (p_target.dead)
            return;

          _target = colliders[i].gameObject;
        }
      }
    }

    private void Chase()
    {
      if (_target == null)
        return;

      if (inAttackRange)
        return;

      _scheduler.CommenceAction(this);

      float targetDirection = 0f;
      float positionDifference = _target.transform.position.x - transform.position.x;

      if (positionDifference > 0)
        targetDirection = 1f;

      if (positionDifference < 0)
        targetDirection = -1f;

      // Moves the character
      Vector2 targetVelocity = new Vector2(targetDirection * _attributes.movementSpeed, _rigidbody.velocity.y);
      _rigidbody.velocity = targetVelocity;

      if (CheckEdge())
        _rigidbody.velocity = Vector2.zero;

      _movementDirection = targetDirection;
    }

    private bool CheckEdge()
    {
      Collider2D[] colliders = Physics2D.OverlapCircleAll(_edgeCheck.position, .5f, _edgeLayer);

      if (colliders.Length == 0)
        return true;

      for (int i = 0; i < colliders.Length; i++)
      {
        if (colliders[i].gameObject != gameObject)
          return false;
      }

      return true;
    }

    private void FlipBehaviour()
    {
      if (_movementDirection < 0 && _facingRight)
        FlipCharacter();
      if (_movementDirection > 0 && !_facingRight)
        FlipCharacter();
    }

    private void FlipCharacter()
    {
      // Switch the way the player is labelled as facing.
      _facingRight = !_facingRight;

      // Multiply the player's x local scale by -1.
      Vector3 theScale = transform.localScale;
      theScale.x *= -1;
      transform.localScale = theScale;
    }

    public void Abort()
    {
      _rigidbody.velocity = Vector2.zero;
    }
  }
}