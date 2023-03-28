using UnityEngine;

namespace Rogue.System
{
  public class GameCamera : MonoBehaviour
  {
    public Vector3 offset = new Vector3(0f, 0f, -10f);
    public float smoothSpeed;

    private Transform _target = null;

    private void Start()
    {
      _target = GameObject.FindWithTag("Player").transform;
    }

    private void LateUpdate()
    {
      SmoothFollow();
    }

    private void SmoothFollow()
    {
      Vector3 desiredPosition = _target.position + offset;
      Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);
      transform.position = smoothedPosition;
    }
  }
}