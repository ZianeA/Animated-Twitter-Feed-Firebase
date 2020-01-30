using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PositionSetter : MonoBehaviour
{
    [SerializeField] private TweetPosition position = TweetPosition.Center;
    public TweetPosition Position { get => position; set => position = value; }
    public enum TweetPosition { Top, Center, Bottom }

    private IEnumerator StartDelayed()
    {
        yield return new WaitForEndOfFrame();
        
        var tweetText = GetComponentInChildren<TypeOn>().GetComponent<TextMeshProUGUI>();
        var panelHeight = PanelSizeFitter.PanelMinSize + tweetText.preferredHeight;
        var trans = GetComponent<RectTransform>();
        var canvasHeight = trans.parent.GetComponent<RectTransform>().rect.height;

        switch (position)
        {
            case TweetPosition.Top:
                trans.anchoredPosition = new Vector2(trans.anchoredPosition.x, canvasHeight - panelHeight / 2);
                break;
            case TweetPosition.Bottom:
                trans.anchoredPosition = new Vector2(trans.anchoredPosition.x, panelHeight / 2);
                break;
            case TweetPosition.Center:
                trans.anchoredPosition = new Vector2(trans.anchoredPosition.x, canvasHeight / 2);
                break;
            default:
                break;
        }
    }

    private void Start()
    {
        StartCoroutine(StartDelayed());
    }
}
