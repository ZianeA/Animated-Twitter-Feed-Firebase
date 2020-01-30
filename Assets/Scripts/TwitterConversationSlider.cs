using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TwitterConversationSlider : MonoBehaviour 
{
    [SerializeField] private AnimationCurve curve;
    [SerializeField] private float slideSpeed = 1.5f;

    private bool isDoneSliding = true;
    private float nextSlideDistance;

    public event Action SlidingComplete;

    public void Slide(float distance)
    {
        if(isDoneSliding)
        {
            StartCoroutine(SlideAnimation(distance));
        }
        else
        {
            nextSlideDistance = distance;
        }
    }

    private IEnumerator SlideAnimation(float distance)
    {
        isDoneSliding = false;
        var trans = GetComponent<RectTransform>();
        var currentPosition = trans.anchoredPosition.y;
        var targetPosition = trans.anchoredPosition.y + distance;
        var t = 0f;

        while (t <= 1)
        {
            t += Time.deltaTime * slideSpeed;
            var tempPosition = Mathf.Lerp(currentPosition, targetPosition, curve.Evaluate(t));
            trans.anchoredPosition = new Vector2(trans.anchoredPosition.x, tempPosition);

            yield return null;
        }

        trans.anchoredPosition = new Vector2(trans.anchoredPosition.x, targetPosition);
        isDoneSliding = true;
        OnSlidingComplete();

        if (nextSlideDistance != 0)
        {
            StartCoroutine(SlideAnimation(nextSlideDistance));
            nextSlideDistance = 0;
        }
    }

    private void OnSlidingComplete()
    {
        var handler = SlidingComplete;

        if(handler != null)
        {
            handler();
        }
    }
}
