using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class M_Audio : MonoBehaviour
{
    public AudioSource AudioSource;


    [Header("-----------")] public AudioClip[] AddPartPancakeAC;
    public float AddPartPancakeVolume = 0.5f;
    public AudioClip[] AddPartFruitAC;
    public float AddPartFruitVolume = 0.5f;
    private float addPartPitchResetTime = 1f;
    private float timeCountAddPart = 0;
    private int indexAddPart = 0;

    public void AddPartPlay(bool isFruit)
    {
        if (!IsSoundFxActive)
        {
            return;
        }

        if (timeCountAddPart + addPartPitchResetTime < Time.time)
        {
            indexAddPart = 0;
        }

        if (timeCountAddPart < Time.time)
        {
            timeCountAddPart = Time.time + 0.025f;

            AudioClip _audioClip;
            if (isFruit)
            {
                _audioClip = AddPartFruitAC[Random.Range(0, AddPartFruitAC.Length)];
                AudioSource.PlayOneShot(_audioClip, AddPartFruitVolume);
            }
            else
            {
                _audioClip = AddPartPancakeAC[Random.Range(0, AddPartPancakeAC.Length)];
                AudioSource.PlayOneShot(_audioClip, AddPartPancakeVolume);
            }

            AudioSource.pitch = 1.0f + (1.0f * indexAddPart / AddPartPancakeAC.Length);
            if (AddPartPancakeAC.Length - 1 > indexAddPart)
            {
                indexAddPart++;
            }
        }
    }


    [Header("-----------")] public AudioClip[] ConfettiAC;
    public float ConfettiVolume = 0.5f;
    public float ConfettiPitch = 1;

    public void ConfettiPlay()
    {
        if (!IsSoundFxActive)
        {
            return;
        }

        StartCoroutine(ConfettiPlayIE());
    }

    IEnumerator ConfettiPlayIE()
    {
        for (int i = 0; i < 2; i++)
        {
            AudioClip _audioClip = ConfettiAC[Random.Range(0, ConfettiAC.Length)];
            AudioSource.PlayOneShot(_audioClip, ConfettiVolume);
            AudioSource.pitch = ConfettiPitch;
            M_Haptic.I.ButtonClick();
            yield return new WaitForSeconds(0.4f);
        }
    }


    public bool IsSoundFxActive
    {
        set
        {
            if (value)
            {
                PlayerPrefs.SetInt("IsSoundFxActive", 1);
            }
            else
            {
                PlayerPrefs.SetInt("IsSoundFxActive", 0);
            }
        }
        get
        {
            if (PlayerPrefs.GetInt("IsSoundFxActive", 0) == 1)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }

    public bool IsMusicActive
    {
        set
        {
            if (value)
            {
                PlayerPrefs.SetInt("IsMusicActive", 1);
            }
            else
            {
                PlayerPrefs.SetInt("IsMusicActive", 0);
            }
        }
        get
        {
            if (PlayerPrefs.GetInt("IsMusicActive", 0) == 1)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }


    private void Awake()
    {
        II = this;
        IsSoundFxActive = IsSoundFxActive;
        IsMusicActive = IsMusicActive;
    }

    public static M_Audio II;

    public static M_Audio I
    {
        get
        {
            if (II == null)
            {
                II = GameObject.Find("M_Audio").GetComponent<M_Audio>();
            }

            return II;
        }
    }
}