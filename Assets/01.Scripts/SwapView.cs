using UnityEngine;
using UnityEngine.EventSystems;

public class SwapView : MonoBehaviour, IPointerClickHandler
{
    public PaperAnimationController paperAnimationController;

    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log("SwapView 클릭 감지됨");
        // 클릭되면 스왑 컨트롤러의 함수 실행
        if (paperAnimationController != null)
        {
            paperAnimationController.SwapPapers(this.gameObject);
        }
    }
}
