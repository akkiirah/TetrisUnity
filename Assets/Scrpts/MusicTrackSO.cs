using UnityEngine;

[CreateAssetMenu(fileName = "MusicTrackSO", menuName = "Scriptable Objects/MusicTrackSO")]
public class MusicTrackSO : ScriptableObject
{
    [Header("Song Name")]
    public string displayName;

    [Header("Core Clip")]
    public AudioClip clip;

    [Header("Playback points (seconds)")]
    public float songStartStateChange = 0f;
    public float songStartAnew = 0f;

    [Header("Fade-In Durations (seconds)")]
    public float fadeStateChange = 1f;
    public float fadeLoop = 0.5f;

    [Range(0, 1)]
    public float volume = 1f;
}

