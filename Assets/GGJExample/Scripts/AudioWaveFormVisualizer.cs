using UnityEngine;
using System.Collections;

[RequireComponent(typeof(AudioSource))]
public class AudioWaveFormVisualizer : MonoBehaviour
{
    #region Private Variables
    private Color[] m_blank; // blank image array 
    private Texture2D m_texture;
    private float[] m_samples; // audio samples array
    private AudioSource m_audioSource;
    #endregion

    #region Public Variables
    public Shader shader;
    public Renderer rend;
    public int width = 500; // texture width 
    public int height = 100; // texture height 
    public Color backgroundColor = Color.black;
    public Color waveformColor = Color.green;
    public int size = 2048; // size of sound segment displayed in texture
    #endregion



    #region Private Methods

    #region Coroutines
    private IEnumerator Start()
    {
        if(rend == null)
        {
            rend = GameObject.FindWithTag("Trail").GetComponent<Renderer>();
        }
        m_audioSource = GetComponent<AudioSource>();
        // create the samples array 
        m_samples = new float[size];

        // create the texture and assign to the guiTexture: 
        m_texture = new Texture2D(width, height, TextureFormat.RGBA32, false);
        m_texture.wrapMode = TextureWrapMode.Repeat;
        m_texture.filterMode = FilterMode.Point;

        Material material = new Material(shader);
        material.mainTexture = m_texture;

        rend.material = material;

        // create a 'blank screen' image 
        m_blank = new Color[width * height];

        for (int i = 0; i < m_blank.Length; i++)
        {
            m_blank[i] = backgroundColor;
        }

        // refresh the display each 100mS 
        while (true)
        {
            GetCurWave();
            yield return new WaitForSeconds(0.1f);
        }
    }
    #endregion

    private void GetCurWave()
    {
        // clear the texture 
        m_texture.SetPixels(m_blank, 0);

        // get samples from channel 0 (left) 
        m_audioSource.GetOutputData(m_samples, 0);

        // draw the waveform 
        for (int i = 0; i < size; i++)
        {
            m_texture.SetPixel((int)(width * i / size), (int)(height * (m_samples[i] + 1f) / 2f), waveformColor);
        } // upload to the graphics card 

        m_texture.Apply();
    }
    #endregion
}