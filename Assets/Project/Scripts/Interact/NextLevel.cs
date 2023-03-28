using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using Rogue.System;

namespace Rogue.Interact
{
  public class NextLevel : MonoBehaviour, Interactable
  {
    [SerializeField] private int _nextLevelIndex = 0;

    private IEnumerator ProceedNextLevel()
    {
      if (_nextLevelIndex == 0)
      {
        Debug.LogWarning("Next Level Index not given");
        yield break;
      }

      GameObject.FindWithTag("SceneTransition").GetComponent<Animator>().SetTrigger("FadeIn");
      GameObject.FindWithTag("GameManager").GetComponent<GameManager>().SavePlayer();

      yield return new WaitForSeconds(1f);

      SceneManager.LoadScene(_nextLevelIndex);
    }

    public void Interact()
    {
      StartCoroutine(ProceedNextLevel());
    }
  }
}