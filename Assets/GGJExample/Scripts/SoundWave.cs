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

    private float m_start;
    private float m_warpT;
    private float m_angle;
    private float m_sinAngle;
    private float m_sinAngleZ;
    private double m_walkShift;
    private Vector3 m_posVtx2;
    private Vector3 m_posVtxSizeMinusOne;
    private float m_ampT;

    Vector3 sv;
    Vector3 ev;
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
            //Vector3 sv = new Vector3(i * .01f, m_waveForm[i] * 10, 0);
            //Vector3 ev = new Vector3(i * .01f, -m_waveForm[i] * 10, 0);
            sv = new Vector3(i * .01f, m_waveForm[i] * m_ampT * m_warpT, 0);
            ev = new Vector3(i * .01f, -m_waveForm[i] * m_ampT * m_warpT, 0);

            SetOrigin();
            AngleWave(i);
            WarpOrNot(i);
            //SetPositionOfVertices(i);

            //if (i == 1) { posVtx2 = new Vector3(length / size * i - start, sinAngle * ampT * warpT, sinAngleZ * ampT * warpT); }
            //if (i == size - 1) { posVtxSizeMinusOne = new Vector3(length / size * i - start, sinAngle * ampT * warpT, sinAngleZ * ampT * warpT); }

            if (i % 2 == 0)
            {
                m_lineRenderer1.SetPosition(i, sv);
            }
            else
            {
                m_lineRenderer1.SetPosition(i, ev);
            }
        }

        if (warpInvert)
        {  //Fixes pinned limits when WarpInverted
            m_lineRenderer1.SetPosition(0, sv);
            m_lineRenderer1.SetPosition(size - 1, ev);
        }

        int current = m_audioSource.timeSamples / m_resolution;
        current *= 2;
        size = current;

        Vector3 c = new Vector3(current * .01f, 0, 0);

        Debug.DrawLine(c, c + Vector3.up * 10, Color.white);
    }

    private void AngleWave(int i)
    {
        m_angle = (2 * Mathf.PI / size * i * freq);
        if (centered)
        {
            m_angle -= freq * Mathf.PI;   //Center
            if (centCrest)
            {
                m_angle -= Mathf.PI / 2;  //Crest/Knot
            }
        }
        else { centCrest = false; }

        m_walkShift -= walkAuto / size * Time.deltaTime;
        m_angle += (float)m_walkShift - walkManual;
        m_sinAngle = Mathf.Sin(m_angle);
        if (spiral) { m_sinAngleZ = Mathf.Cos(m_angle); }
        else { m_sinAngleZ = 0; }
    }

    private void SetPositionOfVertices(int i)
    {
        if (i == 1) { m_posVtx2 = new Vector3(length / size * i - m_start, m_sinAngle * m_ampT * m_warpT, m_sinAngleZ * m_ampT * m_warpT); }
        if (i == size - 1) { m_posVtxSizeMinusOne = new Vector3(length / size * i - m_start, m_sinAngle * m_ampT * m_warpT, m_sinAngleZ * m_ampT * m_warpT); }
    }

    private void WarpOrNot(int i)
    {
        if (warp)
        {
            m_warpT = size - i;
            m_warpT = m_warpT / size;
            m_warpT = Mathf.Sin(Mathf.PI * m_warpT * (warpRandom + 1));
            if (warpInvert) { m_warpT = 1 / m_warpT; }
            m_lineRenderer1.SetPosition(i, new Vector3(length / size * i - m_start, m_sinAngle * m_ampT * m_warpT, m_sinAngleZ * m_ampT * m_warpT));
        }
        else
        {
            m_lineRenderer1.SetPosition(i, new Vector3(length / size * i - m_start, m_sinAngle * m_ampT, m_sinAngleZ * m_ampT));
            warpInvert = false;
        }
    }

    private void UpdateLineRenderer()
    {
        m_lineRenderer1.startWidth = traceWidth;
        m_lineRenderer1.endWidth = traceWidth;

        TargetOptional();

        WarpRandom();
        AmplitudeByFrequency();

        if (warp && warpInvert) { m_ampT = m_ampT / 2; }
    }

    private void AmplitudeByFrequency()
    {
        if (ampByFreq) { m_ampT = Mathf.Sin(freq * Mathf.PI); }
        else { m_ampT = 1; }
        m_ampT = m_ampT * amp;
    }

    private void WarpRandom()
    {
        if (warpRandom <= 0) { warpRandom = 0; }
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
        if (origin == Origins.Start) { m_start = 0; }
        else { m_start = length / 2; }
    }

    #endregion
}
