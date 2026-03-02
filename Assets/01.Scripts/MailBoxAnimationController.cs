using System;
using System.Collections;
using UnityEngine;

public class MailBoxAnimationController : MonoBehaviour
{
    [SerializeField] private Animator LettersAnimator;
    [SerializeField] private Animator EffectAnimator;
    private static readonly int ArriveTrigger_1 = Animator.StringToHash("Arrive_1");
    private static readonly int ArriveTrigger_4 = Animator.StringToHash("Arrive_4");
    private static readonly int EmptyTrigger = Animator.StringToHash("Empty");

    private static readonly int EffectTrigger = Animator.StringToHash("Effect");
    private static readonly int EffectStopTrigger = Animator.StringToHash("EffectStop");

    // 편지가 도착했을 때 (흔들림 등)
    public void PlayMailArrived(Action onComplete, int lettersCount)
    {

        if (lettersCount == 1)
        {
            LettersAnimator.SetTrigger(ArriveTrigger_1);
        }
        else if (lettersCount == 4)
        {
            LettersAnimator.SetTrigger(ArriveTrigger_4);
        }
        EffectAnimator.SetTrigger(EffectTrigger);

        StartCoroutine(WaitAndComplete(onComplete));
    }

    private IEnumerator WaitAndComplete(Action onComplete)
    {
        SoundManager.Instance.PlaySFX(SoundType.MailArrived);
        yield return new WaitForSeconds(1f);
        onComplete?.Invoke();
    }

    // 편지를 클릭해서 비워졌을 때 (사라짐 등)
    public void PlayMailEmpty()
    {
        LettersAnimator.SetTrigger(EmptyTrigger);
        EffectAnimator.SetTrigger(EffectStopTrigger);
    }
}
