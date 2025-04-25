using UnityEngine;

[CreateAssetMenu(fileName = "MusicTrackSO", menuName = "Scriptable Objects/MusicTrackSO")]
public class MusicTrackSO : ScriptableObject
{
    [Header("Core Clip")]
    public AudioClip clip;

    [Header("Playback points (seconds)")]
    public float songStartStateChange = 0f;
    public float songStartAnew = 0f;

    [Header("Fade-In Durations (seconds)")]
    public float fadeInStateChange = 1f;
    public float fadeInAnew = 0.5f;

    [Header("Fade-Out Duration (seconds)")]
    public float fadeOutTime = 4f;

    [Range(0, 1)]
    public float volume = 1f;
}

