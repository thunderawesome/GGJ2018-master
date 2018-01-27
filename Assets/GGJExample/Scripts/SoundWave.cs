using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class SoundWave : MonoBehaviour
{
    #region Private Variables
    private AudioSource m_audioSource;
    private int m_resolution = 60;

    private float[] m_waveForm;
    private float[] m_samples;

    private LineRenderer m_lineRenderer1;

    private float start;
    private float warpT;
    private float angle;
    private float sinAngle;
    private float sinAngleZ;
    private double walkShift;
    private Vector3 posVtx2;
    private Vector3 posVtxSizeMinusOne;
    private float ampT;
    #endregion

    #region Public Variables
    public Material traceMaterial;
    public float traceWidth = 0.3f;
    public GameObject targetOptional;
    public float altRotation;
    public enum Origins { Start, Middle };
    public SoundWave.Origins origin = Origins.Start;
    public int size = 300;
    public float length = 10.0f;
    public float freq = 2.5f;
    public float amp = 1;
    public bool ampByFreq;
    public bool centered = true;
    public bool centCrest = true;
    public bool warp = true;
    public bool warpInvert;
    public float warpRandom;
    public float walkManual;
    public float walkAuto;
    public bool spiral;

    #endregion

    #region Private Methods
    // Use this for initialization
    private void Start()
    {
        m_lineRenderer1 = transform.GetChild(0).GetComponent<LineRenderer>();
        m_lineRenderer1.useWorldSpace = false;
        m_lineRenderer1.material = traceMaterial;

        m_audioSource = GetComponent<AudioSource>();
        m_resolution = m_audioSource.clip.frequency / m_resolution;

        m_samples = new float[m_audioSource.clip.samples * m_audioSource.clip.channels];
        m_audioSource.clip.GetData(m_samples, 0);

        m_waveForm = new float[(m_samples.Length / m_resolution)];
        for (int i = 0; i < m_waveForm.Length; i++)
        {
            m_waveForm[i] = 0;

            for (int ii = 0; ii < m_resolution; ii++)
            {
                m_waveForm[i] += Mathf.Abs(m_samples[(i * m_resolution) + ii]);
            }

            m_waveForm[i] /= m_resolution;
        }
    }

    // Update is called once per frame
    private void Update()
    {
        if (size > m_waveForm.Length) return;
        UpdateLineRenderer();

        for (int i = 0; i < size; i++)
        {
            Vector3 sv = new Vector3(i * .01f, m_waveForm[i] * 10, 0);
            Vector3 ev = new Vector3(i * .01f, -m_waveForm[i] * 10, 0);

            Debug.DrawLine(sv, ev, Color.yellow);
            SetOrigin();

            if (i % 2 == 0)
            {
                m_lineRenderer1.SetPosition(i, sv);
            }
            else
            {
                m_lineRenderer1.SetPosition(i, ev);
            }
        }

        int current = m_audioSource.timeSamples / m_resolution;
        current *= 2;

        Vector3 c = new Vector3(current * .01f, 0, 0);

        Debug.DrawLine(c, c + Vector3.up * 10, Color.white);
        size += 10;
        for (int i = 0; i < 10; i++)
        {
            m_lineRenderer1.GetPosition(i).Set(0,0,0);
        }
        
    }

    private void UpdateLineRenderer()
    {
        m_lineRenderer1.startWidth = traceWidth;
        m_lineRenderer1.endWidth = traceWidth;

        TargetOptional();

        if (size <= 2) { size = 2; }
        m_lineRenderer1.positionCount = size;
    }

    private void TargetOptional()
    {
        if (targetOptional != null)
        {
            origin = Origins.Start;
            length = (transform.position - targetOptional.transform.position).magnitude;
            transform.LookAt(targetOptional.transform.position);
            //			transform.rotation = Quaternion.Euler(transform.localRotation.eulerAngles.x, transform.localRotation.eulerAngles.y - 90, transform.localRotation.eulerAngles.z);
            transform.Rotate(altRotation, -90, 0);
        }
    }

    private void SetOrigin()
    {
        if (origin == Origins.Start) { start = 0; }
        else { start = length / 2; }
    }

    #endregion
}
