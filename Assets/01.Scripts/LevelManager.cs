using UnityEngine;
using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine.Tilemaps;
using System.Collections;
using TMPro;

public class LevelManager : SerializedMonoBehaviour
{
    [Title("각 레벨 리스트")]
    [ListDrawerSettings(ShowIndexLabels = true)]
    public List<BoardInputView> levelBoards = new();
    [Title("각 레벨 쪽지 리스트 & 편지지")]
    public List<LevelNote> levelNotes = new();
    public List<TMP_FontAsset> noteFonts = new();

    public List<Sprite> noteSprites = new();

    [Title("메일 박스 애니 컨트롤러")]
    public MailBoxAnimationController mailBoxAnimationController;


    public TMP_FontAsset defaultNoteFont;

    [Title("보드 부모 위치 ( 중심점 )")]
    [SerializeField] private Transform boardParent;

    [Title("현재 스테이지")]
    [ShowInInspector]
    private int _currentStage = 0;


    [Title("Ending")]
    [SerializeField] private EndingController endingController;

    private int currentStage
    {
        get { return _currentStage; }
        set
        {
            Debug.Log($"<color=orange>[LevelManager]</color> 스테이지 변경됨! 기존: {_currentStage} -> <b>변경: {value}</b>");
            _currentStage = value;
        }
    }
    [Title("클릭 시 앞뒤 바뀌는 SwapView")]
    public SwapView swapView;


    [Title("클리어 연출 끝난 후 올라가기 전까지 대기 시간")]
    public float clearEffectWaitTime = 3f;
    private BoardInputView currentBoardObject;

    public enum GameState
    {
        DeskEmpty,
        MailArrived,
        Playing,
        LevelCleared,
        Ending,
    }
    [Title("상태 확인 (읽기 전용)")]
    [ShowInInspector, ReadOnly]
    private GameState currentState = GameState.DeskEmpty;

    #region 테스트용


    private void OnEnable()
    {
        GameEvents.OnGameClear += HandleGameClear;
        GameEvents.OnMailBoxClicked += HandleMailBoxClicked;
    }
    private void OnDisable()
    {
        GameEvents.OnGameClear -= HandleGameClear;
        GameEvents.OnMailBoxClicked -= HandleMailBoxClicked;
    }


    [Title("테스트 콘솔")]
    [PropertySpace(SpaceBefore = 10)]
    [Button("▶️ 처음부터 시작 (Level 0)", ButtonSizes.Medium)]
    public void StartGame()
    {
        currentStage = 0;
        currentState = GameState.DeskEmpty;

        if (currentBoardObject != null)
        {
            Destroy(currentBoardObject.gameObject);
        }

        StartCoroutine(WaitAndDeliverMailRoutine());

        Debug.Log("🎮 [LevelManager] 게임 시작! 첫 번째 메일 도착 대기 중...");
    }
    [ButtonGroup("TestGroup")]
    [Button("🔄 현재 레벨 재시작")]
    public void ResetCurrentLevel()
    {
        LoadLevel(currentStage);
    }
    [ButtonGroup("TestGroup")]
    [Button("⏭️ 다음 레벨로")]
    public void LoadNextLevel()
    {
        if (currentStage + 1 < levelBoards.Count)
        {
            LoadLevel(currentStage + 1);
        }
        else
        {
            Debug.Log("🎉 모든 레벨을 클리어했습니다!");
        }
    }
    [PropertySpace(SpaceBefore = 10)]
    [GUIColor(0.8f, 1f, 0.8f)]
    [Button("🎯 특정 레벨 강제 실행", ButtonSizes.Large)]
    [InfoBox("버튼 옆의 입력창에 실행할 레벨의 인덱스 번호를 적고 버튼을 누르세요.")]
    public void TestSpecificLevel(int levelIndex)
    {
        LoadLevel(levelIndex);
    }

    #endregion
    private void LoadLevel(int index)
    {
        if (index < 0 || index >= levelBoards.Count)
        {
            Debug.LogError($"레벨 인덱스 오류! {index}번 레벨은 존재하지 않습니다.");
            return;
        }

        if (currentBoardObject != null)
        {
            Destroy(currentBoardObject.gameObject);
        }

        currentStage = index;

        currentBoardObject = Instantiate(levelBoards[index], boardParent);

        TileMap runtimeTileMap = currentBoardObject.tileMap.Clone();

        currentBoardObject.tileMap = runtimeTileMap;

        if (runtimeTileMap != null)
        {
            GameEvents.OnBoardInitialized?.Invoke(runtimeTileMap, currentBoardObject.GetComponent<RectTransform>());



        }
        else
        {
            Debug.LogError("생성된 프리팹에 TileMap 컴포넌트가 없습니다!");
        }
        if (swapView.paperAnimationController != null)
        {
            // string noteData = (index < levelNotes.Count && levelNotes[index] != null)
            //                   ? levelNotes[index].noteText : "쪽지 데이터가 없습니다.";

            // TMP_FontAsset font = noteFonts[index < noteFonts.Count ? index : -1] ?? defaultNoteFont;

            Sprite sprite = (index < noteSprites.Count && noteSprites[index] != null)
                            ? noteSprites[index] : null;
            swapView.paperAnimationController.InitTargets(currentBoardObject.GetComponent<RectTransform>(), sprite);


        }
    }

    private void HandleGameClear()
    {
        Debug.Log($"[LevelManager] {currentStage} 레벨 클리어 이벤트 수신!");
        if (currentState == GameState.LevelCleared)
        {
            Debug.Log("<color=grey>[LevelManager]</color> 이미 클리어 처리 중입니다. 중복 이벤트를 무시합니다.");
            return;
        }
        currentState = GameState.LevelCleared;
        StartCoroutine(ProcessLevelClearRoutine());

    }
    private IEnumerator ProcessLevelClearRoutine()
    {
        SoundManager.Instance.PlaySFX(SoundType.Success);
        yield return new WaitForSeconds(1f);

        bool isEffectDone = false;
        bool isClearAnimationDone = false;


        BoardClearEffect effectView = currentBoardObject.GetComponent<BoardClearEffect>();
        if (effectView != null)
        {
            effectView.PlayClearEffect(() =>
            {
                isEffectDone = true;
            });

            // 물들기 애니메이션이 끝날 때까지 여기서 멈춰서 기다림
            yield return new WaitUntil(() => isEffectDone);
        }

        yield return new WaitForSeconds(clearEffectWaitTime);
        if (currentBoardObject != null)

        {

            swapView.paperAnimationController.PlayClearOutro(() =>
            {
                isClearAnimationDone = true;
            });

            yield return new WaitUntil(() => isClearAnimationDone);
        }
        if (currentBoardObject != null)
        {
            Destroy(currentBoardObject.gameObject);
        }
        currentStage++;
        currentState = GameState.DeskEmpty;

        if (currentStage < levelBoards.Count)
        {
            StartCoroutine(WaitAndDeliverMailRoutine());
        }
        else
        {
            Debug.Log("🎉 [LevelManager] 모든 레벨을 클리어했습니다! 엔딩을 띄웁니다.");
            StartCoroutine(WaitAndDeliverMailRoutine());

        }
    }

    private IEnumerator WaitAndDeliverMailRoutine()
    {

        yield return new WaitForSeconds(1.5f);

        if (mailBoxAnimationController != null)
        {
            int lettersCount = (currentStage < 4) ? 1 : 4; // 예시: 4레벨까지는 편지 1장, 그 이후로는 편지 4장 도착
            mailBoxAnimationController.PlayMailArrived(() =>
            {
                currentState = GameState.MailArrived;
            }, lettersCount);
        }
        else
        {
            // 안전 장치: 컴포넌트가 없을 경우 바로 상태 변경
            currentState = GameState.MailArrived;
        }

    }

    private void HandleMailBoxClicked()
    {

        // 메일 온 상태가 아니면 무시
        if (currentState != GameState.MailArrived)
        {
            return;
        }

        if (mailBoxAnimationController != null)
        {
            mailBoxAnimationController.PlayMailEmpty();
            SoundManager.Instance.PlaySFX(SoundType.MailBoxOpen);
        }
        Debug.Log($"currentStage : {currentStage}");


        if (currentStage < 4)
        {
            LoadLevel(currentStage);
            currentState = GameState.Playing;
        }
        else
        {
            endingController.StartEndingSequence();
        }

    }
}
