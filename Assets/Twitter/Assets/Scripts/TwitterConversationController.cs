using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Linq;

public class TwitterConversationController : MonoBehaviour
{
    private List<float> panelsHeight;
    private List<TwitterConversationSlider> panelsSlider;
    private List<RectTransform> tweets;
    private int tweetsDoneCount = 1;
    private RectTransform trans;

    public static readonly int gap = 24;

    private void Start()
    {
        StartCoroutine(Initialize());
    }

    private IEnumerator Initialize()
    {
        yield return new WaitForEndOfFrame();

        panelsHeight = new List<float>();
        panelsSlider = new List<TwitterConversationSlider>();
        tweets = new List<RectTransform>();

        for (int i = 0; i < transform.childCount; i++)
        {
            var child = transform.GetChild(i);
            var textLabel = child.GetComponentInChildren<TweetTimer>().GetComponent<TextMeshProUGUI>();
            var panelHeight = PanelSizeFitter.PanelMinSize + textLabel.preferredHeight;
            panelsHeight.Add(panelHeight);

            panelsSlider.Add(child.GetComponentInChildren<TwitterConversationSlider>());
            child.GetComponentInChildren<TweetTimer>().TimesUp += OnTimesUp;
            tweets.Add(child.GetComponent<RectTransform>());
        }

        // trans = GetComponent<RectTransform>();
        // trans.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, panelsHeight.Sum() + panelsHeight.Count * gap);
        // trans.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, panelsHeight[0]);
    }

    private void OnTimesUp()
    {
        StartCoroutine(AnimateConversation());
    }

    private IEnumerator AnimateConversation()
    {
        if (tweetsDoneCount == tweets.Count) yield break;

        var availableSpace = tweets[tweetsDoneCount - 1].anchoredPosition.y - panelsHeight[tweetsDoneCount - 1] / 2;
        Debug.Log($"Available space: {availableSpace}");
        var slideDistance = Mathf.Abs(Mathf.Clamp(availableSpace - (panelsHeight[tweetsDoneCount] + gap), Mathf.NegativeInfinity, 0));
        Debug.Log($"Slide Distance: {slideDistance}");
        var nextTweetPos = availableSpace + slideDistance - gap - panelsHeight[tweetsDoneCount] / 2;
        // var slideDistance = Mathf.Abs(Mathf.Clamp(panelsHeight[tweetsDoneCount - 1] / 2 + panelsHeight[tweetsDoneCount] / 2 + gap, 0, Mathf.Infinity));

        for (int i = 0; i < tweetsDoneCount; i++)
        {
            panelsSlider[i].Slide(slideDistance);

            var t = 0f;
            var timeBetweenTweets = i >= tweetsDoneCount - 1 ? 0.5f : 0.1f;

            while (t <= 1)
            {
                t += Time.deltaTime * 1 / timeBetweenTweets;

                yield return null;
            }

            if (i >= tweetsDoneCount - 1)
            {
                var nextTweet = tweets[i + 1];
                nextTweet.anchoredPosition = new Vector2(nextTweet.anchoredPosition.x, nextTweetPos);
                nextTweet.GetComponent<AnimationController>().Play();
            }
        }

        tweetsDoneCount += 1;
    }
}
