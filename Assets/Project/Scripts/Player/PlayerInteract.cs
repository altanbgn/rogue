using UnityEngine;
using Rogue.Interact;
using TMPro;

namespace Rogue.Player {
  public class PlayerInteract : MonoBehaviour {
    [SerializeField] private LayerMask _interactLayerMask;
    [SerializeField] private TextMeshPro _interactMessage;
    private Interactable _interactable = null;

    private void Update() {
      FindInteractable();

      if (_interactable != null && Input.GetButtonDown("Interact")) {
        _interactable.Interact();
      }
    }

    private void FindInteractable() {
      if (_interactMessage == null) {
        return;
      }

      _interactable = null;
      _interactMessage.enabled = false;

      Collider2D collider = Physics2D.OverlapCircle(
        transform.position,
        3f,
        _interactLayerMask
      );

      if (collider == null) {
        return;
      }

      Interactable interactable = collider.gameObject.GetComponent<Interactable>();

      if (interactable == null) {
        return;
      }

      _interactMessage.enabled = true;
      _interactable = interactable;
    }
  }
}
