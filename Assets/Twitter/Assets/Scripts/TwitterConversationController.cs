using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Linq;

public class TwitterConversationController : MonoBehaviour
{
    [SerializeField]
    private LongPressTrigger longPressTrigger;
    private List<float> panelsHeight;
    private List<TwitterConversationSlider> panelsSlider;
    private List<RectTransform> tweets;
    private List<TweetTimer> tweetTimers = new List<TweetTimer>();
    private int tweetsDoneCount = 1;
    private RectTransform trans;
    private bool canSkipTweet = true;

    public static readonly int gap = 24;

    private void Start()
    {
        longPressTrigger.onLongPress += () => StartCoroutine(SkipTweet());
        trans = GetComponent<RectTransform>();
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

            var tweetTimer = child.GetComponentInChildren<TweetTimer>();  
            tweetTimers.Add(tweetTimer);
            tweetTimer.TimesUp += OnTimesUp;

            tweets.Add(child.GetComponent<RectTransform>());
        }
    }

    private void OnTimesUp()
    {
        StartCoroutine(AnimateConversation());
    }

    private IEnumerator SkipTweet()
    {
        yield return new WaitUntil(() => canSkipTweet);

        if (tweetsDoneCount >= tweets.Count) yield break;

        canSkipTweet = false;
        for (int i = 0; i < tweetsDoneCount; i++)
        {
            tweetTimers[i].TimesUp -= OnTimesUp;
        }
        StartCoroutine(AnimateConversation());
    }

    private IEnumerator AnimateConversation()
    {
        if (tweetsDoneCount >= tweets.Count) yield break;

        canSkipTweet = false;
        var nextTweetIndex = tweetsDoneCount;
        var currentTweetIndex = tweetsDoneCount - 1;

        // Calculate available space to find sliding distance and next tweet position.
        var availableSpace = tweets[currentTweetIndex].anchoredPosition.y - panelsHeight[currentTweetIndex] / 2;
        var slideDistance = Mathf.Abs(Mathf.Clamp(availableSpace - (panelsHeight[nextTweetIndex] + gap), Mathf.NegativeInfinity, 0));
        var nextTweetPos = availableSpace + slideDistance - gap - panelsHeight[nextTweetIndex] / 2;

        // Start next tweet after the current tweet is done sliding.
        Action onCurrentTweetSlidingComplete = null;
        panelsSlider[currentTweetIndex].SlidingComplete += onCurrentTweetSlidingComplete = () =>
        {
            // Run only once.
            panelsSlider[currentTweetIndex].SlidingComplete -= onCurrentTweetSlidingComplete;

            tweets[nextTweetIndex].anchoredPosition = new Vector2(tweets[nextTweetIndex].anchoredPosition.x, nextTweetPos);
            tweets[nextTweetIndex].GetComponent<AnimationController>().Play();
            tweetsDoneCount += 1;
            canSkipTweet = true;
        };

        // Fit height to height of content.
        var contentHeight = panelsHeight.GetRange(0, tweetsDoneCount + 1).Sum() + tweetsDoneCount * gap;
        if (contentHeight > trans.rect.height)
        {
            trans.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, contentHeight);
        }

        // Slide all the done tweets the same distance.
        for (int i = 0; i < tweetsDoneCount; i++)
        {
            panelsSlider[i].Slide(slideDistance);

            // Action onLastTweetSlidingComplete = null;
            // if (i == tweetsDoneCount - 1)
            // {
            //     panelsSlider[i].SlidingComplete += onLastTweetSlidingComplete = () =>
            //     {
            //         panelsSlider[i].SlidingComplete -= onLastTweetSlidingComplete;

            //     };
            // }
        }
    }
}
