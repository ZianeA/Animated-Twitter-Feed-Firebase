using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Linq;
using UnityEngine.Networking;
using Firebase;
using Firebase.Database;
using Firebase.Unity.Editor;
using Firebase.Extensions;
using UnityEngine.UI;
using static PositionSetter;

public class TwitterConversationController : MonoBehaviour
{
    [SerializeField] private LongPressTrigger longPressTrigger;
    [SerializeField] private GameObject tweetPrefab;
    [SerializeField] private TweetPosition conversationStartPosition = TweetPosition.Center;
    private List<float> panelsHeight = new List<float>();
    private List<TwitterConversationSlider> panelsSlider = new List<TwitterConversationSlider>();
    private List<RectTransform> tweets = new List<RectTransform>();
    private List<TweetTimer> tweetTimers = new List<TweetTimer>();
    private List<AudioSource> tweetsAudio = new List<AudioSource>();
    private int tweetsDoneCount = 1;
    private RectTransform trans;
    private bool canSkipTweet = true;

    public static readonly int gap = 24;

    private void Start()
    {
        trans = GetComponent<RectTransform>();

        Firebase.FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(t =>
        {
            var dependencyStatus = t.Result;
            if (dependencyStatus == Firebase.DependencyStatus.Available)
            {
                FirebaseApp.DefaultInstance.SetEditorDatabaseUrl("https://animated-twitter-feed.firebaseio.com/");
                Debug.Log("firebase initialized successfully");
                FirebaseDatabase.DefaultInstance.GetReference("tweets").GetValueAsync().ContinueWithOnMainThread(task =>
                {
                    if (task.IsFaulted)
                    {
                        Debug.LogError("failed to retrive firebase database");
                    }
                    else if (task.IsCompleted)
                    {
                        Debug.Log("firebase database snapshot retrieved successfully");
                        DataSnapshot snapshot = task.Result;
                        int i = 0;

                        foreach (var tweetData in snapshot.Children)
                        {
                            var tweet = Instantiate(tweetPrefab) as GameObject;
                            tweets.Add(tweet.GetComponent<RectTransform>());
                            tweet.transform.SetParent(transform, false);

                            if (i == 0)
                            {
                                var positionSetter = tweet.AddComponent<PositionSetter>();
                                positionSetter.Position = conversationStartPosition;
                                tweet.GetComponent<AnimationController>().PlayOnAwake = true;
                            }

                            var tweetController = tweet.GetComponent<TweetController>();
                            tweetController.Name.text = tweetData.Child("name").Value as string;
                            tweetController.Username.text = tweetData.Child("username").Value as string;
                            tweetController.Body.text = AutoTags.AddTags(tweetData.Child("body").Value as string);
                            tweetController.Date.text = tweetData.Child("date").Value as string;
                            var profileImageUrl = tweetData.Child("profile_image").Value as string;
                            StartCoroutine(loadProfileImage(profileImageUrl, tweetController.ProfileImage));

                            i++;
                        }

                        longPressTrigger.onLongPress += () => StartCoroutine(SkipTweet());
                        StartCoroutine(Initialize());
                    }
                });
            }
            else
            {
                UnityEngine.Debug.LogError(System.String.Format(
                  "Could not resolve all Firebase dependencies: {0}", dependencyStatus));
            }
        });
    }

    private IEnumerator loadProfileImage(string profileImageUrl, Image profileImage)
    {
        UnityWebRequest request = UnityWebRequestTexture.GetTexture(profileImageUrl);

        yield return request.SendWebRequest();

        if (request.isNetworkError || request.isHttpError)
            Debug.LogError(request.error);
        else
        {
            var texture = ((DownloadHandlerTexture)request.downloadHandler).texture;
            profileImage.sprite = Sprite.Create(texture, new Rect(0.0f, 0.0f, texture.width, texture.height), new Vector2(0.5f, 0.5f), 100.0f);
        }
    }

    private IEnumerator Initialize()
    {
        yield return new WaitForEndOfFrame();

        for (int i = 0; i < tweets.Count; i++)
        {
            var tweet = tweets[i];
            var textLabel = tweet.GetComponentInChildren<TweetTimer>().GetComponent<TextMeshProUGUI>();
            var panelHeight = PanelSizeFitter.PanelMinSize + textLabel.preferredHeight;
            panelsHeight.Add(panelHeight);

            panelsSlider.Add(tweet.GetComponentInChildren<TwitterConversationSlider>());

            var tweetTimer = tweet.GetComponentInChildren<TweetTimer>();
            tweetTimers.Add(tweetTimer);
            tweetTimer.TimesUp += OnTimesUp;

            tweetsAudio.Add(tweetTimer.GetComponent<AudioSource>());
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
            tweetsAudio[i].mute = true;
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
        }
    }
}
