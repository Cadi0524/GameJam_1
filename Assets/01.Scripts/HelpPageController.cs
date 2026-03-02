using UnityEngine;
using UnityEngine.UI; // GraphicRaycaster 사용을 위해 필수!

public class HelpPageController : MonoBehaviour
{
    public int number = 500;
    private void Awake()
    {
        // 1. 캔버스 컴포넌트 가져오기 (없으면 자동 생성)
        Canvas canvas = GetComponent<Canvas>();
        if (canvas == null)
        {
            canvas = gameObject.AddComponent<Canvas>();
        }

        if (GetComponent<GraphicRaycaster>() == null)
        {
            gameObject.AddComponent<GraphicRaycaster>();
        }

        // 3. //memo LineRenderer 및 다른 UI를 무시하고 무조건 맨 앞으로 강제 배치
        canvas.overrideSorting = true;
        canvas.sortingOrder = number;
    }


}