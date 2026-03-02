using UnityEngine;
using UnityEngine.UI;
using Sirenix.OdinInspector;
using System.Collections;

public class TutorialManager : MonoBehaviour
{
    [Title("레벨매니저")]
    [SerializeField] private LevelManager levelManager;

    [Title("UI 화면 참조")]
    [SerializeField] private GameObject startScreen;        // 시작 화면 (타이틀)
    [SerializeField] private GameObject fullScreenNote;     // 전체화면 쪽지 (N초 대기용)
    [SerializeField] private GameObject tutorialPage;       // 튜토리얼 이미지 
    [SerializeField] private Button tutorialToggleButton;   // 좌측 하단 튜토리얼 On/Off 버튼


    [SerializeField] private Sprite sprite;

    [Title("설정")]
    [SerializeField] private float noteDisplayTime = 3f;    // 쪽지가 떠 있을 시간 (N초)

    private void Start()
    {
        startScreen.SetActive(true);
        fullScreenNote.SetActive(false);
        tutorialPage.SetActive(false);
        tutorialToggleButton.gameObject.SetActive(false);

        // tutorialToggleButton.onClick.AddListener(ToggleTutorial);
    }

    public void OnClickStartButton()
    {
        StartCoroutine(TutorialSequenceRoutine());
        SoundManager.Instance?.PlaySFX(SoundType.ClickUI);
    }


    public void OnClickQuitButton()
    {
        if (SoundManager.Instance != null)
            SoundManager.Instance.PlaySFX(SoundType.ClickUI);

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
    private IEnumerator TutorialSequenceRoutine()
    {
        startScreen.SetActive(false);
        fullScreenNote.SetActive(true);

        yield return new WaitForSeconds(noteDisplayTime);

        fullScreenNote.SetActive(false);


        tutorialPage.SetActive(true);
        tutorialToggleButton.gameObject.SetActive(true);

        Debug.Log("🎮 [TutorialManager] 오프닝 종료! 첫 메일 도착 로직을 실행합니다.");

        levelManager.StartGame();


    }

    // 좌측 하단 버튼이나 튜토리얼 내부의 '닫기' 버튼에서 호출할 함수
    public void ToggleTutorial()
    {

        SoundManager.Instance.PlaySFX(SoundType.ClickUI);
        bool isActive = tutorialPage.activeSelf;
        tutorialPage.SetActive(!isActive);

    }
}
