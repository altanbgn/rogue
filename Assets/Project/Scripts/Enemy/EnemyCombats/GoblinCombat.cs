using UnityEngine;
using Rogue.System;
using Rogue.Player;

namespace Rogue.Enemy {
  // Required components to run this script
  [RequireComponent(typeof(ActionScheduler))]
  [RequireComponent(typeof(Animator))]
  [RequireComponent(typeof(EnemyAttributes))]
  [RequireComponent(typeof(EnemyMovement))]

  public class GoblinCombat : MonoBehaviour, IAction {
    [SerializeField] private LayerMask _attackLayer;
    [SerializeField] private Transform _attackCheck;

    private ActionScheduler _scheduler;
    private Animator _animator;
    private EnemyAttributes _attributes;
    private EnemyMovement _movement;

    private GameObject _target = null;
    private bool _inAttackRange = false;
    private float _cooldownCounter = 0f;

    private void Start() {
      _scheduler = GetComponent<ActionScheduler>();
      _animator = GetComponent<Animator>();
      _attributes = GetComponent<EnemyAttributes>();
      _movement = GetComponent<EnemyMovement>();
    }

    private void Update() {
      if (_attributes.dead) {
        return;
      }

      _cooldownCounter += Time.deltaTime;

      CheckAttackable();

      if (_animator.GetCurrentAnimatorStateInfo(0).IsTag("Attack")) {
        return;
      }

      if (_animator.GetCurrentAnimatorStateInfo(0).IsTag("Hit")) {
        return;
      }

      if (_cooldownCounter > _attributes.attackCooldown) {
        Attack();
      }
    }

    private void CheckAttackable() {
      _inAttackRange = false;
      _movement.inAttackRange = false;
      _target = null;

      Collider2D[] colliders = Physics2D.OverlapBoxAll(_attackCheck.position, new Vector2(_attributes.attackRange, 2f), 0f, _attackLayer);

      for (int i = 0; i < colliders.Length; i++) {
        if (
          colliders[i].gameObject != gameObject &&
          colliders[i].gameObject.CompareTag("Player")
        ) {
          PlayerAttributes p_target = colliders[i].gameObject.GetComponent<PlayerAttributes>();

          if (p_target == null) {
            return;
          }

          if (p_target.dead) {
            return;
          }

          _target = colliders[i].gameObject;

          if (Mathf.Abs(
            _target.transform.position.x - transform.position.x
          ) <= _attributes.attackRange) {
            _inAttackRange = true;
            _movement.inAttackRange = true;
            _movement.cooldownCounter = 0f;
          }
        }
      }
    }

    private void Attack() {
      if (_target == null) {
        return;
      }

      if (!_inAttackRange) {
        return;
      }

      _scheduler.CommenceAction(this);
      _animator.SetTrigger("Attack");
      _cooldownCounter = 0f;
    }

    private void AE_Attack() {
      if (_target == null)
        return;

      PlayerAttributes p_attributes = _target.GetComponent<PlayerAttributes>();

      if (p_attributes == null)
        return;

      if (p_attributes.dodgeImmunity)
        return;

      p_attributes.TakeDamage(_attributes.attackDamage);
    }

    public void Abort() {

    }
  }
}
