using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PositionSetter : MonoBehaviour
{
    [SerializeField] private Position position = Position.Center;
    [SerializeField] private TextMeshProUGUI tweetText;

    public enum Position { Top, Center, Bottom }

    private IEnumerator StartDelayed()
    {
        yield return new WaitForEndOfFrame();
        
        var panelHeight = PanelSizeFitter.PanelMinSize + tweetText.preferredHeight;
        var trans = GetComponent<RectTransform>();
        var canvasHeight = trans.parent.GetComponent<RectTransform>().rect.height;

        switch (position)
        {
            case Position.Top:
                trans.anchoredPosition = new Vector2(trans.anchoredPosition.x, canvasHeight - panelHeight / 2);
                break;
            case Position.Bottom:
                trans.anchoredPosition = new Vector2(trans.anchoredPosition.x, panelHeight / 2);
                break;
            case Position.Center:
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
