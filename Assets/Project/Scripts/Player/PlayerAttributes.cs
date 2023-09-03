using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using Rogue.System;

namespace Rogue.Player {
  // Required components to run this script
  [RequireComponent(typeof(ActionScheduler))]
  [RequireComponent(typeof(Animator))]

  // Main script
  public class PlayerAttributes : MonoBehaviour {
    public Attribute maxHealth;
    public float currentHealth = 100f;
    [Space(10)]
    public Attribute physicalDamage;
    public Attribute magicDamage;
    [Space(10)]
    public Attribute movementSpeed;
    public Attribute jumpForce;

    public GameObject deathMessage;

    [HideInInspector] public bool dead = false;
    [HideInInspector] public bool dodgeImmunity = false;
    [HideInInspector] public float dodgeForce = 25f;
    [HideInInspector] public float dodgeCooldown = .8f;

    private ActionScheduler _scheduler;
    private Animator _animator;

    private void Start() {
      _scheduler = GetComponent<ActionScheduler>();
      _animator = GetComponent<Animator>();

      GameObject goGameManager = GameObject.FindWithTag("GameManager");

      if (!goGameManager) {
        return;
      }

      GameManager gameManager = goGameManager.GetComponent<GameManager>();

      if (!gameManager) {
        return;
      }

      if (gameManager.bSaved == false) {
        return;
      }

      gameManager.LoadPlayer();
    }

    public void TakeDamage(float amount) {
      if (dead)
        return;

      currentHealth = Mathf.Max(currentHealth - amount, 0);

      if (currentHealth == 0)
      {
        StartCoroutine(Die());
        return;
      }

      if (_scheduler)
        _scheduler.AbortAction();

      _animator.SetTrigger("Hit");
    }

    private IEnumerator Die() {
      if (dead)
        yield break;

      dead = true;

      if (_animator)
        _animator.SetTrigger("Death");

      if (_scheduler)
        _scheduler.AbortAction();

      GameObject _deathMessage = Instantiate(deathMessage, new Vector3(0, 0, 0), Quaternion.identity) as GameObject;
      _deathMessage.transform.SetParent(GameObject.FindGameObjectWithTag("Canvas").transform, false);

      yield return new WaitForSeconds(5f);

      GameObject.FindWithTag("SceneTransition").GetComponent<Animator>().SetTrigger("FadeIn");
      GameObject.FindWithTag("GameManager").GetComponent<GameManager>().ResetProgress();

      yield return new WaitForSeconds(1f);

      SceneManager.LoadScene(1);
    }
  }
}
