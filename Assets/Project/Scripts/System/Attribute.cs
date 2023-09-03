using UnityEngine;
using System;
using System.Collections.Generic;

namespace Rogue.System {
  [Serializable]

  public class Attribute {
    private List<float> _modifiers = new List<float>();

    public float baseValue;

    public float GetValue() {
      float finalValue = baseValue;

      foreach (float x in _modifiers) {
        finalValue += x;
      }

      return (float)Mathf.Round(finalValue * 100f) / 100f;
    }

    public void AddModifier(float p_modifier) {
      if (p_modifier != 0) {
        _modifiers.Add(p_modifier);
      }
    }

    public void RemoveModifier(float p_modifier) {
      if (p_modifier != 0) {
        _modifiers.Remove(p_modifier);
      }
    }
  }
}
