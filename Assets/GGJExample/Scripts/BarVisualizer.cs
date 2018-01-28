using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BarVisualizer : MonoBehaviour
{

    public int detail = 500;
    public float minValue = 1.0f;
    public float amplitude = .1f;

    private float randomAmplitude = 1.0f;
    private Vector3 startScale;

    private void Start()
    {
        startScale = transform.localScale;

        randomAmplitude = Random.Range(0.5f, 1.5f);
    }

    private void Update()
    {
        float[] info = new float[detail];
        AudioListener.GetOutputData(info, 0);
        float packagedData = 0.0f;

        for (int x = 0; x < info.Length; x++)
        {
            packagedData += System.Math.Abs(info[x]);
        }

        transform.localScale = new Vector3(minValue, (packagedData * amplitude * randomAmplitude) + startScale.y, minValue);
    }
}
