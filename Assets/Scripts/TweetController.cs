using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TweetController : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI name;
    [SerializeField] private TextMeshProUGUI username;
    [SerializeField] private TextMeshProUGUI body;
    [SerializeField] private TextMeshProUGUI date;
    [SerializeField] private Image profileImage;

    public TextMeshProUGUI Name { get => name; set => name = value; }
    public TextMeshProUGUI Username { get => username; set => username = value; }
    public TextMeshProUGUI Body { get => body; set => body = value; }
    public TextMeshProUGUI Date { get => date; set => date = value; }
    public Image ProfileImage { get => profileImage; set => profileImage = value; }
}
