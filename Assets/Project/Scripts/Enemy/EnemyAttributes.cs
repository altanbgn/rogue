using System.Collections;
using UnityEngine;
using Rogue.System;

namespace Rogue.Enemy
{
  public class EnemyAttributes : MonoBehaviour
  {
    public float maxHealth = 30f;
    public float currentHealth = 30f;
    [Space(10)]
    public float attackDamage = 10;
    public float attackRange = 4f;
    public float attackCooldown = 5f;
    [Space(10)]
    public float vision = 20f;
    public float movementSpeed = 10f;

    [HideInInspector] public bool dead = false;

    public void TakeDamage(float amount)
    {
      if (dead)
        return;

      currentHealth = Mathf.Max(currentHealth - amount, 0);

      if (currentHealth == 0)
        StartCoroutine(Die());
      else
      {
        Animator _animator = GetComponent<Animator>();
        _animator.SetTrigger("Hit");
      }
    }

    private IEnumerator Die()
    {
      if (dead)
        yield break;

      dead = true;

      Animator _animator = GetComponent<Animator>();
      ActionScheduler _scheduler = GetComponent<ActionScheduler>();

      if (_animator != null)
        _animator.SetTrigger("Death");

      if (_scheduler != null)
        _scheduler.AbortAction();

      Destroy(this.gameObject, 20f);
    }
  }
}