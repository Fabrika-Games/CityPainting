using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Data/UnlockSystem/UnlockSystemData", fileName = "NewUnlockSystemData")]
public class UnlockSystemData : ScriptableObject
{
    public UnlockAssets[] UnlockAssets;
}

[System.Serializable]
public struct UnlockAssets
{
    public int UnlockLevelCount;
    public Sprite UnlockStrokeSprite;
    public Sprite UnlockColourfulSprite;
}