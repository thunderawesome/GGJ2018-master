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

    //set these in the inspector!
    public AudioSource master;
    public AudioSource[] slaves;

    public GameObject[] visualsFX;
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
    }

    private void Update()
    {
        for (int i = 0; i < slaves.Length; i++)
        {
            slaves[i].timeSamples = master.timeSamples;
        }        
    }

    public void SetTrack(AudioSource audioSource)
    {
        try
        {
            audioSource.clip = tracks[m_index];

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
            Debug.Log("VisualFX " + m_index + " ACTIVE");
            visualsFX[m_index].SetActive(true);
            m_index++;
            if (m_index >= audioNodes.Count) m_index = 0;

        }
        catch (System.Exception)
        {

            throw;
        }
    }
}