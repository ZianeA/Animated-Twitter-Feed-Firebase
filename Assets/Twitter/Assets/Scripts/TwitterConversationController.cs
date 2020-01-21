using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TwitterConversationController : MonoBehaviour
{
    private List<float> panelsHeight;
    private List<TwitterConversationSlider> panelsSlider;
    private List<RectTransform> tweets;
    private int tweetsDoneCount = 1;

    public static readonly int gap = 25;

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
    }

    private void OnTimesUp()
    {
        StartCoroutine(AnimateConversation());
    }

    private IEnumerator AnimateConversation()
    {
        if (tweetsDoneCount == tweets.Count) yield break;

        var slideDistance = Mathf.Abs(Mathf.Clamp(panelsHeight[tweetsDoneCount - 1] / 2 + panelsHeight[tweetsDoneCount] / 2 + gap, 0, Mathf.Infinity));

        for (int i = 0; i < tweetsDoneCount; i++)
        {
            var nextTweetPos = tweets[i].anchoredPosition.y;
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
