using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class LongPressTrigger : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    [SerializeField]
    [Tooltip("How long must pointer be down on this object to trigger a long press")]
    private float holdTime = 1f;

    [SerializeField]
    private RectTransform canvas;
    private float timestamp = Mathf.Infinity;
    private float pressPosition = Mathf.Infinity;

    public event Action onLongPress;

    public void OnPointerDown(PointerEventData eventData)
    {
        pressPosition = eventData.position.y;
        timestamp = Time.time;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        var dragDistance = Mathf.Abs(eventData.position.y - pressPosition) / canvas.rect.height;
        if (Time.time - timestamp >= holdTime && dragDistance < 0.035f) OnLongPressInvoke();

        timestamp = Mathf.Infinity;
        pressPosition = Mathf.Infinity;
    }

    private void OnLongPressInvoke()
    {
        var handler = onLongPress;

        if (handler != null)
        {
            handler();
        }
    }
}
