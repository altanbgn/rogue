using UnityEngine;
using Rogue.Player;
using Rogue.System;

namespace Rogue.System {
  public class GameManager : MonoBehaviour {
    public int stage = 1;

    private Attribute maxHealth;
    private float currentHealth = 100f;
    [Space(10)]
    private Attribute physicalDamage;
    private Attribute magicDamage;
    [Space(10)]
    private Attribute movementSpeed;
    private Attribute jumpForce;

    public bool bSaved = false;

    public void ResetProgress() {
      Debug.Log("Reset Progress");
      stage = 1;
    }

    public void ProgressStage() {
      stage++;
    }

    public void SavePlayer() {
      PlayerAttributes attributes = GameObject
        .FindWithTag("Player")
        .GetComponent<PlayerAttributes>();

      if (!attributes) {
        return;
      }

      maxHealth = attributes.maxHealth;
      currentHealth = attributes.currentHealth;
      physicalDamage = attributes.physicalDamage;
      magicDamage = attributes.magicDamage;
      movementSpeed = attributes.movementSpeed;
      jumpForce = attributes.jumpForce;

      bSaved = true;
    }

    public void LoadPlayer() {
      PlayerAttributes attributes = GameObject
        .FindWithTag("Player")
        .GetComponent<PlayerAttributes>();

      if (!attributes) {
        return;
      }

      attributes.maxHealth = maxHealth;
      attributes.currentHealth = currentHealth;
      attributes.physicalDamage = physicalDamage;
      attributes.magicDamage = magicDamage;
      attributes.movementSpeed = movementSpeed;
      attributes.jumpForce = jumpForce;

      bSaved = false;
    }
  }
}
