using DG.Tweening;
using TMPro;
using UnityEngine;

public class GameClearHelperView : MonoBehaviour
{
    [SerializeField] private GameObject helperObject;
    [SerializeField] private RectTransform helperRect;
    [SerializeField] private TextMeshProUGUI helperText;


    [Header("DOTween Settings")]
    [SerializeField] private float startPosX = -800f; // 화면 밖 (시작/원위치 X좌표)
    [SerializeField] private float targetPosX = 0f;   // 화면 안 (목표 X좌표)
    [SerializeField] private float duration = 0.5f;   // 애니메이션 재생 시간

    private Tween currentTween;

    private bool isHelperObjectActive;
    private void OnEnable()
    {
        GameEvents.OnClearHelper += SetHelperText;
        GameEvents.OnTilePointerDown += OnPointerClickEvent;


        isHelperObjectActive = false;

    }

    private void OnDisable()
    {
        GameEvents.OnClearHelper -= SetHelperText;
        GameEvents.OnTilePointerDown -= OnPointerClickEvent;
    }

    private void Awake()
    {
        if (helperObject != null)
        {
            Canvas helperCanvas = helperObject.GetComponent<Canvas>();
            if (helperCanvas == null)
            {
                helperCanvas = helperObject.AddComponent<Canvas>();
            }


            helperCanvas.overrideSorting = true;
            helperCanvas.sortingOrder = 300;

            if (helperRect != null)
            {
                helperRect.anchoredPosition = new Vector2(startPosX, helperRect.anchoredPosition.y);
                helperObject.SetActive(false);
            }
        }
    }

    private void SetHelperText(bool isFull, int wrongCount)
    {
        string pendingText = isFull ? $"기억과 다른 색이 전달되었습니다.\n현재 총 {wrongCount}개의 색이 잘못 전달되었습니다."
                                    : "색의 선이 지나가지 않은 부분이 존재합니다.";

        // 1. 진행 중인 애니메이션 즉시 정지
        currentTween?.Kill();
        SetHelperObjectActive(true);

        // 2. 현재 위치 파악 (중간에 취소되었을 경우를 대비)
        float currentX = helperRect.anchoredPosition.x;

        // //memo 케이스 1: 이미 화면 밖에 있거나 거의 밖에 있다면 (오차 허용) -> 즉시 등장
        if (currentX <= startPosX + 10f)
        {
            helperText.text = pendingText;
            currentTween = helperRect.DOAnchorPosX(targetPosX, duration).SetEase(Ease.OutQuint);
        }
        else
        {
            // //memo 케이스 2 & 3: 화면에 나와있는 도중이거나 들어가던 도중이라면
            // 이동할 목표(startPosX)까지 남은 거리에 비례하여 애니메이션 시간을 다시 계산 (일정한 속도 유지)
            float distanceRatio = Mathf.Abs(currentX - startPosX) / Mathf.Abs(targetPosX - startPosX);
            float outDuration = duration * distanceRatio;

            // 일단 화면 밖으로 퇴장
            // 주의: 중간에 끊길 때는 Ease.InQuad(부드러운 가속)를 써야 튕기는 어색함이 없습니다.
            currentTween = helperRect.DOAnchorPosX(startPosX, outDuration).SetEase(Ease.InQuad)
                .OnComplete(() =>
                {
                    // 완전히 밖으로 나간 뒤 텍스트를 몰래 교체하고, 다시 쫀득하게(OutBack) 등장
                    helperText.text = pendingText;
                    currentTween = helperRect.DOAnchorPosX(targetPosX, duration).SetEase(Ease.OutQuint);
                });
        }
    }

    private void OnPointerClickEvent(Vector2Int pos)
    {
        if (!isHelperObjectActive) return;

        currentTween?.Kill();

        // 퇴장할 때도 현재 위치를 기준으로 시간을 계산하여 부드럽게 퇴장
        float currentX = helperRect.anchoredPosition.x;
        float distanceRatio = Mathf.Abs(currentX - startPosX) / Mathf.Abs(targetPosX - startPosX);
        float outDuration = duration * distanceRatio;

        currentTween = helperRect.DOAnchorPosX(startPosX, outDuration).SetEase(Ease.InQuad)
            .OnComplete(() =>
            {
                SetHelperObjectActive(false);
            });
    }

    private void SetHelperObjectActive(bool isActive)
    {
        isHelperObjectActive = isActive;
        helperObject.SetActive(isActive);
    }

}
