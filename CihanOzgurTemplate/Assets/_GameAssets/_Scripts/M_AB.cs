using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using System.Linq;
using System;

public class M_AB : MonoBehaviour
{
    public AB CurrentAB;
    public TMP_Dropdown ABChangeDropdown;


    public void ChangeAB()
    {
        PlayerPrefs.DeleteAll();
        PlayerPrefs.SetInt("Debug_AB", Mathf.Clamp(ABChangeDropdown.value, 0, (int) AB.ChildCount - 1));
        Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
        // SceneManager.LoadScene(0);
    }

    private void Awake()
    {
        II = this;

        if (M_Config.I.IsDebug)
        {
        }
        else
        {
            ABChangeDropdown.transform.parent.gameObject.SetActive(false);
        }
    }

    List<string> ABList = new List<string>();

    public void Start()
    {
        for (int i = 0; i < (int) AB.ChildCount; i++)
        {
            ABList.Add(((AB) i).ToString());
        }

#if UNITY_ANDROID
         CurrentAB = AB.Control;
         PlayerPrefs.SetInt("UserAB", (int) AB.Optimized);
         if (IsDebug)
         {
             ABChangeDropdown.SetValueWithoutNotify((int) CurrentAB);
             ABChangeDropdown.interactable = false;
         }
#else
        if (!M_Config.I.IsDebug)
        {
            if (!PlayerPrefs.HasKey("UserAB"))
            {
                // string currentABName = VoodooSauce.GetPlayerCohort();
                // if (VoodooSauce.GetAbTests().Contains(currentABName))
                // {
                //     var index = Array.IndexOf(ABList.ToArray(), ABList.Where(x => x == currentABName).FirstOrDefault());
                //     if (index < 0)
                //     {
                //         index = 0;
                //     }
                //
                //     CurrentAB = (AB) index;
                //     PlayerPrefs.SetInt("UserAB", index);
                // }
            }
            else
            {
                CurrentAB = (AB) PlayerPrefs.GetInt("UserAB");
            }
        }

#endif
    }

    public bool IsWinAB(AB aB)
    {
        // if (CurrentAB == AB.V3_1_IOS)
        // {
        //     return false;
        // }
        //
        // switch (aB)
        // {
        //     case AB.Shop:
        //         return true;
        // }

        return false;
    }


    public static M_AB II;

    public static M_AB I
    {
        get
        {
            if (II == null)
            {
                GameObject _g = GameObject.Find("M_AB");
                if (_g != null)
                {
                    II = _g.GetComponent<M_AB>();
                }
            }

            return II;
        }
    }
}

public enum AB
{
    Control,

    ChildCount
}