using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System; // TextMeshPro 사용 시 추가

public class PaperAnimationController : MonoBehaviour
{
    [Title("스왑 대상 UI")]
    public RectTransform noteRect;
    public TextMeshProUGUI noteTextUI;
    private RectTransform boardRect;

    [Title("편지지")]

    [SerializeField] private Image noteImage;

    [Title("스왑 애니메이션 세팅")]
    public float duration = 0.3f;
    [InfoBox("종이 높이의 몇 %만큼 위로 뽑아낼지 결정 (0.65 = 65%)")]
    public float slideUpRatio = 0.65f;


    // 같은 캔버스 안이 되게 되면, 무조건 라인 렌더러가 보이게 되므로
    private Canvas boardCanvas;
    private Canvas noteCanvas;

    [Title("클리어 아웃트로 세팅")]
    public float outroDuration = 1f;

    [Title("앞면 세팅 (기준점)")]
    public Vector2 frontPosition = Vector2.zero;
    public Vector3 frontRotation = Vector3.zero;

    [Title("뒷면 세팅 (앞면 기준 오프셋)")]
    public Vector2 backPositionOffset = new Vector2(20f, -10f);
    public Vector3 backRotation = new Vector3(0, 0, -5f);

    private bool isBoardFront = true;
    private bool isAnimating = false;


    [SerializeField] private BoardDrawView boardDrawView;
    public void InitTargets(RectTransform newBoard, Sprite sprite)
    {
        boardRect = newBoard;


        if (noteImage != null && sprite != null)
        {
            noteImage.sprite = sprite;
            noteImage.SetNativeSize();
        }

        SwapView boardView = boardRect.gameObject.GetComponent<SwapView>();
        if (boardView != null) boardView.paperAnimationController = this;

        SwapView noteView = noteRect.gameObject.GetComponent<SwapView>();
        if (noteView != null) noteView.paperAnimationController = this;

        boardCanvas = boardRect.GetComponent<Canvas>();
        if (boardCanvas == null) boardCanvas = boardRect.gameObject.AddComponent<Canvas>();
        if (boardRect.GetComponent<GraphicRaycaster>() == null) boardRect.gameObject.AddComponent<GraphicRaycaster>();

        boardCanvas.overrideSorting = true;

        noteCanvas = noteRect.GetComponent<Canvas>();
        if (noteCanvas == null) noteCanvas = noteRect.gameObject.AddComponent<Canvas>();
        if (noteRect.GetComponent<GraphicRaycaster>() == null) noteRect.gameObject.AddComponent<GraphicRaycaster>();

        noteCanvas.overrideSorting = true;


        // 노트가 앞
        noteCanvas.sortingOrder = 200;
        boardCanvas.sortingOrder = 0;

        noteRect.anchoredPosition = frontPosition;
        noteRect.localRotation = Quaternion.Euler(frontRotation);
        noteRect.SetAsLastSibling();

        boardRect.anchoredPosition = frontPosition + backPositionOffset;
        boardRect.localRotation = Quaternion.Euler(backRotation);
        boardRect.SetAsFirstSibling();

        isBoardFront = false;



    }
    public void SwapPapers(GameObject clickedObj = null)
    {
        if (isAnimating || boardRect == null || noteRect == null) return;

        if (clickedObj != null)
        {
            if (isBoardFront && clickedObj == boardRect.gameObject) return;
            if (!isBoardFront && clickedObj == noteRect.gameObject) return;
        }

        SoundManager.Instance.PlaySFX(SoundType.PaperSwap);
        isAnimating = true;

        RectTransform goToFront = isBoardFront ? noteRect : boardRect;
        RectTransform goToBack = isBoardFront ? boardRect : noteRect;

        Sequence swapSequence = DOTween.Sequence();

        float currentHeight = boardRect.rect.height;
        float slideOutY = frontPosition.y + (currentHeight * slideUpRatio);
        float halfDuration = duration * 0.5f;

        swapSequence.Append(goToFront.DOAnchorPosY(slideOutY, halfDuration).SetEase(Ease.OutSine));

        swapSequence.AppendCallback(() =>
        {
            goToFront.SetAsLastSibling();

            if (goToFront == noteRect)
            {
                noteCanvas.sortingOrder = 200;
                boardCanvas.sortingOrder = 0;
                if (boardDrawView != null) boardDrawView.SetLineRendererOrder(false);
            }
            else
            {
                boardCanvas.sortingOrder = 200;
                noteCanvas.sortingOrder = 0;
                if (boardDrawView != null) boardDrawView.SetLineRendererOrder(true);
            }
        });

        swapSequence.Append(goToFront.DOAnchorPos(frontPosition, halfDuration).SetEase(Ease.OutBack));
        swapSequence.Join(goToFront.DORotate(frontRotation, halfDuration).SetEase(Ease.OutQuad));

        Vector2 targetBackPos = frontPosition + backPositionOffset;
        swapSequence.Join(goToBack.DOAnchorPos(targetBackPos, halfDuration).SetEase(Ease.InOutQuad));
        swapSequence.Join(goToBack.DORotate(backRotation, halfDuration).SetEase(Ease.InOutQuad));

        swapSequence.OnComplete(() =>
        {
            isBoardFront = !isBoardFront;
            isAnimating = false;
        });
    }


    public void PlayClearOutro(Action onComplete)
    {

        Debug.Log("[PaperAnimationController] 클리어 아웃트로(위로 날리기)");
        Sequence outroSeq = DOTween.Sequence();

        SoundManager.Instance.PlaySFX(SoundType.PaperUP);

        outroSeq.Join(boardRect.DOAnchorPosY(1000f, outroDuration).SetRelative().SetEase(Ease.InBack));
        outroSeq.Join(noteRect.DOAnchorPosY(1000f, outroDuration).SetRelative().SetEase(Ease.InBack));

        outroSeq.OnComplete(() =>
        {
            Debug.Log("클리어 아웃트로 끝");

            onComplete?.Invoke(); //

        });
    }
}