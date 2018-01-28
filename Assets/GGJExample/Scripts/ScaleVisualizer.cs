using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScaleVisualizer : MonoBehaviour
{
    public int detail = 500;
    public float minValue = 1.0f;
    public float amplitude = .1f;

    public float randomAmplitude = 1.0f;
    private Vector3 startScale;

    void Start()
    {
        startScale = transform.localScale;

        randomAmplitude = Random.Range(1.0f, 3.0f);
    }

    void Update()
    {
        float[] info = new float[detail];
        GetComponent<AudioSource>().GetOutputData(info, 0);
        float packagedData = 0.0f;

        for (int x = 0; x < info.Length; x++)
        {
            packagedData += System.Math.Abs(info[x]);
        }

        transform.localScale = new Vector3((packagedData * amplitude) + startScale.y, (packagedData * amplitude) + startScale.y, (packagedData * amplitude) + startScale.z);
    }
}
