using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rogue.System;
using Rogue.Enemy;

namespace Rogue.Player {
  // Required components to run this script
  [RequireComponent(typeof(ActionScheduler))]
  [RequireComponent(typeof(Rigidbody2D))]
  [RequireComponent(typeof(Animator))]
  [RequireComponent(typeof(PlayerMovement))]
  [RequireComponent(typeof(PlayerAttributes))]

  // Main script
  public class PlayerCombat : MonoBehaviour, IAction {
    public float timeBetweenCombo = 0.35f;

    [SerializeField] private LayerMask _attackLayer;
    [SerializeField] private Transform _attackCheck;
    private ActionScheduler _scheduler;
    private Animator _animator;
    private Rigidbody2D _rigidbody;
    private PlayerMovement _movement;
    private PlayerAttributes _attributes;
    private List<GameObject> _targets = new List<GameObject>();

    private int _comboStep = 1;

    private void Start() {
      _scheduler = GetComponent<ActionScheduler>();
      _animator = GetComponent<Animator>();
      _rigidbody = GetComponent<Rigidbody2D>();
      _movement = GetComponent<PlayerMovement>();
      _attributes = GetComponent<PlayerAttributes>();
    }

    private void Update() {
      if (_attributes.dead) {
        return;
      }

      GetTargets();

      // When you are during these animations, You can't do the current action
      if (!_animator.GetCurrentAnimatorStateInfo(0).IsTag("Movement")) {
        return;
      }

      CommenceAttack();
    }

    private void GetTargets() {
      _targets = new List<GameObject>();

      Collider2D[] colliders = Physics2D.OverlapBoxAll(
        _attackCheck.position,
        new Vector2(4f, 1.5f),
        0f,
        _attackLayer
      );

      for (int i = 0; i < colliders.Length; i++) {
        if (colliders[i].gameObject != gameObject && !colliders[i].gameObject.CompareTag("Player")) {
          _targets.Add(colliders[i].gameObject);
        }
      }
    }

    private void CommenceAttack() {
      if (Input.GetButtonDown("Fire1")) {
        _scheduler.CommenceAction(this);

        switch (_comboStep) {
          case 1: {
            _animator.ResetTrigger("AttackAbort");
            _animator.SetTrigger("Attack1");
            _comboStep += 1;
            break;
          }
          case 2: {
            _animator.ResetTrigger("AttackAbort");
            _animator.SetTrigger("Attack2");
            _comboStep += 1;
            break;
          }
        }

        StartCoroutine(ComboReset(_comboStep));
      }
    }

    private IEnumerator ComboReset(int p_comboStep) {
      yield return new WaitForSeconds(timeBetweenCombo);

      if (_comboStep == p_comboStep) {
        _comboStep = 1;
      }
    }

    private void AE_Attack() {
      if (_targets.Count == 0) {
        return;
      }

      for (int i = 0; i < _targets.Count; i++) {
        EnemyAttributes e_attributes = _targets[i].gameObject.GetComponent<EnemyAttributes>();

        if (e_attributes != null) {
          e_attributes.TakeDamage(_attributes.physicalDamage.GetValue());
        }
      }
    }

    public void Abort() {
      _animator.ResetTrigger("Attack1");
      _animator.ResetTrigger("Attack2");
      _animator.SetTrigger("AttackAbort");
      _movement.Abort();
    }
  }
}
