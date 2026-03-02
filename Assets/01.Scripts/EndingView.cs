using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class EndingView : MonoBehaviour, IPointerClickHandler
{
    public Action OnClickNextNote;

    public void OnPointerClick(PointerEventData eventData)
    {

        OnClickNextNote?.Invoke();
    }


}
