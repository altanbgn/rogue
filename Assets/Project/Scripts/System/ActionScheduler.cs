using UnityEngine;

namespace Rogue.System
{
  public class ActionScheduler : MonoBehaviour
  {
    private IAction _currentAction;

    public void CommenceAction(IAction action)
    {
      if (_currentAction == action) return;

      if (_currentAction != null)
        _currentAction.Abort();

      _currentAction = action;
    }

    public void AbortAction()
    {
      CommenceAction(null);
    }
  }
}