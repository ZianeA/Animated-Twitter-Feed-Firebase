using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenOrientationController : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        var canvasRect = transform.GetComponent<RectTransform>().rect;
        if (canvasRect.width > canvasRect.height)
        {
            Screen.orientation = ScreenOrientation.LandscapeLeft;
        }
        else
        {
            Screen.orientation = ScreenOrientation.Portrait;
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}
