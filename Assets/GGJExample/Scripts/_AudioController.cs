using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class _AudioController : MonoBehaviour
{
    public static _AudioController Instance;

    public AudioClip[] tracks;

    #region Private Variables
    private int m_index;
    #endregion

    private void Awake()
    {
        Instance = this;
    }

    public void SetTrack(AudioSource audioSource)
    {
        try
        {
            audioSource.clip = tracks[m_index];
            m_index++;
            if (m_index >= tracks.Length) m_index = 0;
        }
        catch (System.Exception)
        {

            throw;
        }
    }
}
