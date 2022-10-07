using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class M_Quality : MonoBehaviour
{
    public RectTransform ReferanceCanvas;
    public int width;
    public int height;

    void Start()
    {
        width = (int) ReferanceCanvas.rect.width;
        height = (int) ReferanceCanvas.rect.height;
        if (height <= UnityEngine.Screen.height)
        {
            UnityEngine. Screen.SetResolution(width, height,
                FullScreenMode.FullScreenWindow);
        }
        else
        {
            width = UnityEngine. Screen.width;
            height =  UnityEngine.Screen.height;
        }


        Destroy(ReferanceCanvas.gameObject);
#if UNITY_ANDROID || PLATFORM_ANDROID
        // Screen.SetResolution((int) (Screen.width * 0.5f), (int) (Screen.height * 0.5f), true);
#endif
    }


    private void Awake()
    {
        II = this;
    }

    public static M_Quality II;

    public static M_Quality I
    {
        get
        {
            if (II == null)
            {
                II = GameObject.Find("M_Quality").GetComponent<M_Quality>();
            }

            return II;
        }
    }
}