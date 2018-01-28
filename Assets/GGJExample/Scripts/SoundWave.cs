using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class SoundWave : MonoBehaviour
{
    #region Private Variables
    private AudioSource m_audioSource;
    private int m_resolution = 60;

    private float[] m_waveForm;
    private float[] m_samples;

    private LineRenderer m_lineRenderer;

    private float m_start;
    private float m_warpT;
    private float m_angle;
    private float m_sinAngle;
    private float m_sinAngleZ;
    private double m_walkShift;
    private float m_ampT;

    Vector3 sv;
    Vector3 ev;

    private CurvedLinePoint[] m_linePoints = new CurvedLinePoint[0];
    private Vector3[] m_linePositions = new Vector3[0];
    #endregion

    #region Public Variables
    public Material traceMaterial;
    public float traceWidth = 0.3f;
    public GameObject targetOptional;
    public float altRotation;
    public enum Origins { Start, Middle };
    public SoundWave.Origins origin = Origins.Start;
    public float lineSegmentSize = 0.01f;
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

    public Vector3 offset;
    #endregion

    #region Private Methods
    // Use this for initialization
    private void Start()
    {
        m_lineRenderer = transform.GetChild(0).GetComponent<LineRenderer>();
        m_lineRenderer.useWorldSpace = false;
        m_lineRenderer.material = traceMaterial;

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
        if (size > m_waveForm.Length || _GameManager.Instance.isMusicPlaying == false) return;
        UpdateLineRenderer();
    }

    private void WarpOrNot(int i)
    {
        if (warp)
        {
            m_warpT = size - i;
            m_warpT = m_warpT / size;
            m_warpT = Mathf.Sin(Mathf.PI * m_warpT * (warpRandom + 1));
            if (warpInvert) { m_warpT = 1 / m_warpT; }
        }
        else
        {
            warpInvert = false;
        }
    }

    private void UpdateLineRenderer()
    {
        m_lineRenderer.startWidth = traceWidth;
        m_lineRenderer.endWidth = traceWidth;

        TargetOptional();

        if (size <= 2) { size = 2; }
        m_lineRenderer.positionCount = size;

        AmplitudeByFrequency();

        if (warp && warpInvert) { m_ampT = m_ampT / 2; }

        for (int i = 0; i < size; i++)
        {
            sv = new Vector3(i * lineSegmentSize, m_waveForm[i] * m_ampT * m_warpT, 0);
            ev = new Vector3(i * lineSegmentSize, -m_waveForm[i] * m_ampT * m_warpT, 0);

            SetOrigin();
            WarpOrNot(i);

            if (i % 2 == 0)
            {
                m_lineRenderer.SetPosition(i, sv);
            }
            else
            {
                m_lineRenderer.SetPosition(i, ev);
                
            }
            m_lineRenderer.transform.position = new Vector3((-i) * lineSegmentSize, 0, 0);
        }

        if (warpInvert)
        {  //Fixes pinned limits when WarpInverted
            m_lineRenderer.SetPosition(0, sv);
            m_lineRenderer.SetPosition(size - 1, ev);
        }
        int current = m_audioSource.timeSamples / m_resolution;
        current *= 2;
        size = current;

        Vector3 c = new Vector3(current * .01f, 0, 0);

        Debug.DrawLine(c, c + Vector3.up * 10, Color.white);

        LerpColors();
    }

    private void AmplitudeByFrequency()
    {
        if (ampByFreq) { m_ampT = Mathf.Sin(freq * Mathf.PI); }
        else { m_ampT = 1; }
        m_ampT = m_ampT * amp;
    }

    private void TargetOptional()
    {
        if (targetOptional != null)
        {
            origin = Origins.Start;
            length = (transform.position - targetOptional.transform.position).magnitude;

            transform.LookAt(targetOptional.transform.position + offset);
            transform.Rotate(altRotation, -90, 0);
        }
    }

    private void SetOrigin()
    {
        if (origin == Origins.Start) { m_start = 0; }
        else { m_start = length / 2; }
    }

    private void LerpColors()
    {
        float alpha = 1.0f;
        Gradient gradient = new Gradient();
        gradient.SetKeys(
            new GradientColorKey[] {
                new GradientColorKey(Color.Lerp(Color.white, Color.blue, Mathf.PingPong(Time.time, 1)), Mathf.PingPong(Time.time, 1)),
                new GradientColorKey(Color.Lerp(Color.magenta, Color.red, Mathf.PingPong(Time.time, 1)), Mathf.PingPong(Time.time, 1)) },
                new GradientAlphaKey[] { new GradientAlphaKey(alpha, 0.0f), new GradientAlphaKey(alpha, 1.0f) }
            );
        m_lineRenderer.colorGradient = gradient;
    }
    #endregion
}
