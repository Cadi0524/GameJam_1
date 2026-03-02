using UnityEngine;
using UnityEngine.UI;
using Sirenix.OdinInspector;
using System.Collections.Generic;
using DG.Tweening;
using System;

public class BoardClearEffect : SerializedMonoBehaviour
{
    [Title("연출 대상")]
    public Image colorPhotoImage;
    public List<GameObject> dotObjects = new List<GameObject>();

    [Title("연출 세팅")]
    public float fillDuration = 2f;

    public void PlayClearEffect(Action onComplete)
    {
        HideDots();

        if (colorPhotoImage != null)
        {
            SoundManager.Instance.PlaySFX(SoundType.ImageToColor);
            colorPhotoImage.fillAmount = 0f;
            colorPhotoImage.DOFillAmount(1f, fillDuration)
                .SetEase(Ease.InOutSine)
                .OnComplete(() => onComplete?.Invoke());
        }
        else
        {
            onComplete?.Invoke();
        }
    }

    private void HideDots()
    {
        // 당장 끄는 거라면 activeSelf 조작
        foreach (var dot in dotObjects)
        {
            if (dot != null) dot.SetActive(false);
        }

    }
}
