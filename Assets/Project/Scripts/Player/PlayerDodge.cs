using UnityEngine;
using Rogue.System;

namespace Rogue.Player
{
  // Required components to run this script
  [RequireComponent(typeof(ActionScheduler))]
  [RequireComponent(typeof(Animator))]
  [RequireComponent(typeof(Rigidbody2D))]
  [RequireComponent(typeof(PlayerAttributes))]

  // Main script
  public class PlayerDodge : MonoBehaviour, IAction
  {
    private ActionScheduler _scheduler;
    private Animator _animator;
    private Rigidbody2D _rigidbody;
    private PlayerAttributes _attributes;

    private float _dodgeCooldownCounter = Mathf.Infinity;

    private void Start()
    {
      _scheduler = GetComponent<ActionScheduler>();
      _animator = GetComponent<Animator>();
      _rigidbody = GetComponent<Rigidbody2D>();
      _attributes = GetComponent<PlayerAttributes>();
    }

    private void Update()
    {
      if (_attributes.dead)
        return;

      _dodgeCooldownCounter += Time.deltaTime;

      // When you are during these animations, You can't do the current action
      if (!_animator.GetCurrentAnimatorStateInfo(0).IsTag("Movement"))
        return;

      Dodge();
    }

    private void Dodge()
    {
      if (Input.GetButtonDown("Dodge") && Mathf.Abs(_rigidbody.velocity.y) < 0.001f && _dodgeCooldownCounter > _attributes.dodgeCooldown)
      {
        // Trigger roll animation
        _animator.SetTrigger("Dodge");

        // Starts roll action
        _scheduler.CommenceAction(this);

        // Rolling function with movement
        Vector2 targetVelocity = new Vector2(transform.localScale.x * _attributes.dodgeForce, _rigidbody.velocity.y);
        _rigidbody.velocity = targetVelocity;

        // Resets cooldown
        _dodgeCooldownCounter = 0;
      }
    }

    private void AE_Start_Immunity()
    {
      _attributes.dodgeImmunity = true;
    }

    private void AE_End_Immunity()
    {
      _attributes.dodgeImmunity = false;
    }

    public void Abort()
    {
      return;
    }
  }
}