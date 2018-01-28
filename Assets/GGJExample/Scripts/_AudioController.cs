using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class _AudioController : MonoBehaviour
{
    #region Public Variables
    public static _AudioController Instance;

    public AudioClip[] tracks;
    public GameObject audioNode;

    public List<GameObject> audioNodes;
    #endregion

    #region Private Variables
    private int m_index;
    #endregion

    #region Public Properties
    public GameObject CurrentAudioNode
    {
        get
        {
            Debug.Log("INDEX: " + m_index + " && AudioNode: " + audioNodes[m_index]);
            return audioNodes[m_index];
        }
    }
    #endregion

    private void Awake()
    {
        Instance = this;
        audioNodes = new List<GameObject>();
    }

    public void SetTrack(AudioSource audioSource)
    {
        try
        {
            audioSource.clip = tracks[m_index];
            audioSource.Play();

        }
        catch (System.Exception)
        {

            throw;
        }
    }

    public void InitializeAudioNode()
    {
        try
        {
            CurrentAudioNode.transform.parent = null;
            m_index++;
            if (m_index >= tracks.Length) m_index = 0;

        }
        catch (System.Exception)
        {

            throw;
        }
    }
}
