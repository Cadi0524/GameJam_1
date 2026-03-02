using UnityEngine;

public class PenAnimationController : MonoBehaviour
{
    [SerializeField] private Animator penAnimator;

    private void OnEnable()
    {
        GameEvents.OnTilePointerDown += HandlePointerDown;
        GameEvents.OnTilePointerUp += HandlePointerUp;

        penAnimator.speed = 0f;
    }

    private void OnDisable()
    {
        GameEvents.OnTilePointerDown -= HandlePointerDown;
        GameEvents.OnTilePointerUp -= HandlePointerUp;
    }

    private void HandlePointerDown(Vector2Int pos)
    {
        penAnimator.speed = 1f;
    }

    private void HandlePointerUp()
    {
        penAnimator.speed = 0f;
    }


}
