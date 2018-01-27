using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class SoundWave : MonoBehaviour
{
    #region Private Variables
    private AudioSource m_audioSource;
    #endregion


    #region Private Methods
    int resolution = 60;

    float[] waveForm;
    float[] samples;

    // Use this for initialization
    void Start()
    {
        m_audioSource = GetComponent<AudioSource>();
        resolution = m_audioSource.clip.frequency / resolution;

        samples = new float[m_audioSource.clip.samples * m_audioSource.clip.channels];
        m_audioSource.clip.GetData(samples, 0);

        waveForm = new float[(samples.Length / resolution)];

        for (int i = 0; i < waveForm.Length; i++)
        {
            waveForm[i] = 0;

            for (int ii = 0; ii < resolution; ii++)
            {
                waveForm[i] += Mathf.Abs(samples[(i * resolution) + ii]);
            }

            waveForm[i] /= resolution;
        }
    }

    // Update is called once per frame
    void Update()
    {
        MeshFilter filter = GetComponent<MeshFilter>();
        Mesh mesh = filter.mesh;
        Vector3[] vertices = mesh.vertices;

        for (int i = 0; i < waveForm.Length - 1; i++)
        {
            Vector3 sv = new Vector3(i * .01f, waveForm[i] * 10, 0);
            Vector3 ev = new Vector3(i * .01f, -waveForm[i] * 10, 0);

            Debug.DrawLine(sv, ev, Color.yellow);

            if (i <= 512)
                vertices[i] = Vector3.Scale(sv, ev);
        }

        int current = m_audioSource.timeSamples / resolution;
        current *= 2;

        Vector3 c = new Vector3(current * .01f, 0, 0);

        Debug.DrawLine(c, c + Vector3.up * 10, Color.white);
    }

    #endregion
}
