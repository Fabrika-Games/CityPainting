using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
{
    [SerializeField]
    private M_Pool mPool;

    [SerializeField]
    private Enemy _enemyPrefab;

    private void Start()
    {
        // Getting pool from pool manager.
        // Also has other overloads.
        var enemyPool = mPool.GetPool<Enemy>();

        // Find pool of type Enemy and get object from it.
        // Also has other overloads.
        var enemy = mPool.GetFromPool<Enemy>();

        // Find pool of type Enemy and returns clone to it.
        // Also has other overloads.
        mPool.TakeToPool<Enemy>(enemy);
    }
}
