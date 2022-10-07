using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class M_Loading : MonoBehaviour
{
    
    private void Start()
    {
        StopAllCoroutines();
        StartCoroutine(Load());
    }

    IEnumerator Load()
    {
        yield return new WaitForEndOfFrame();
        AsyncOperation async = SceneManager.LoadSceneAsync(1);
        async.allowSceneActivation = false;
        while (!async.isDone)
        {
            yield return null;
            async.allowSceneActivation = true;
        }
    }


    private void Awake()
    {
        II = this;
    }

    public static M_Loading II;

    public static M_Loading I
    {
        get
        {
            if (II == null)
            {
                II = GameObject.Find("M_Loading").GetComponent<M_Loading>();
            }

            return II;
        }
    }
}