using UnityEngine;
using UnityEngine.EventSystems;

public class MailboxView : MonoBehaviour, IPointerClickHandler
{
    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log("MailBox 클릭됨");


       GameEvents.OnMailBoxClicked?.Invoke();
    }
}
