using UnityEngine;
using Sirenix.OdinInspector;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine.UI;

public class EndingController : SerializedMonoBehaviour
{
    [Title("엔딩 쪽지")]
    [SerializeField] private List<RectTransform> endingNotes = new();

    [Title("정위치")]
    [SerializeField] private RectTransform originPos;

    [Title("레이케스터")]
    [SerializeField] private GameObject ray;

    [Title("연출 설정")]
    public float upDistance = 1000f;
    public float upDuration = 1f;

    public float downDuration = 0.5f;

    [Title("엔딩버튼")]
    public Button endingButton;

    [SerializeField] private EndingView endingView;

    [ShowInInspector]
    private int currentNoteIndex = 0;
    [ShowInInspector]
    private bool isAnimation = false;


    private void OnEnable()
    {
        endingView.OnClickNextNote += OnClickNextNote;
    }

    private void OnDisable()
    {
        endingView.OnClickNextNote -= OnClickNextNote;
    }

    [Button("엔딩테스트")]
    public void EndingTest()
    {
        StartEndingSequence();
    }
    public void StartEndingSequence()
    {

        currentNoteIndex = 0;
        isAnimation = false;

        foreach (var note in endingNotes)
        {
            // 위치는 각각 다름 
            note.gameObject.SetActive(true);
        }
        ray.SetActive(true);
    }

    public void OnClickNextNote()
    {
        if (isAnimation || currentNoteIndex >= endingNotes.Count) return;

        isAnimation = true;
        RectTransform currentNote = endingNotes[currentNoteIndex];

        currentNote.DOAnchorPosY(upDistance, upDuration)
            .SetRelative()
            .SetEase(Ease.InBack) // 뒤로 살짝 당겼다가 날아가는 효과
            .OnComplete(() =>
            {
                currentNote.gameObject.SetActive(false);
                currentNoteIndex++;

                if (currentNoteIndex < endingNotes.Count)
                {
                    RectTransform nextNote = endingNotes[currentNoteIndex];

                    nextNote.DOAnchorPos(originPos.anchoredPosition, downDuration)
                        .SetEase(Ease.OutCubic)
                        .OnComplete(() =>
                        {
                            isAnimation = false;
                        });

                    nextNote.DORotate(originPos.localEulerAngles, downDuration).SetEase(Ease.OutCubic);
                }
                else
                {
                    Debug.Log("🎉 [엔딩 끝] 모든 쪽지가 날아갔습니다! 종료 버튼을 띄웁니다.");
                    isAnimation = false;

                    // 더 이상 빈 화면을 클릭해도 아무 반응 없도록 레이캐스터를 끕니다.
                    if (ray != null) ray.SetActive(false);

                    // 종료 버튼을 짠! 하고 나타나게 합니다.
                    if (endingButton != null)
                    {
                        endingButton.gameObject.SetActive(true);

                        // 원한다면 버튼도 DOTween으로 스르륵 나타나게 할 수 있습니다. (CanvasGroup 필요)
                        // endingButton.GetComponent<CanvasGroup>().DOFade(1f, 1f); 
                    }
                }
            });
    }

    public void OnClickEndingButton()
    {
        // 종료 효과음이 있다면 재생
        // SoundManager.Instance?.PlaySFX(SoundType.SFX_Click);

        Debug.Log("게임을 종료합니다.");

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
