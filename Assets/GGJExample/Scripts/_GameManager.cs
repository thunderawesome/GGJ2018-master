using UnityEngine;

public class _GameManager : MonoBehaviour
{
    public static _GameManager Instance;

    public bool isMusicPlaying = false;

    private void Awake()
    {
        Instance = this;

    }
}
