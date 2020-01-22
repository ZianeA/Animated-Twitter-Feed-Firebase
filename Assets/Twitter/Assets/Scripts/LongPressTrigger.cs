using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class LongPressTrigger : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    [SerializeField]
    [Tooltip("How long must pointer be down on this object to trigger a long press")]
    private float holdTime = 1f;
    private float timestamp = 0f;

    public event Action onLongPress;

    public void OnPointerDown(PointerEventData eventData)
    {
        timestamp = Time.time;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (Time.time - timestamp >= holdTime) OnLongPressInvoke();
        timestamp = 0;
    }

    private void OnLongPressInvoke()
    {
        Debug.Log("Hola!");
        var handler = onLongPress;

        if (handler != null)
        {
            handler();
        }
    }
}
