using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class MusicManager : Singleton<MusicManager>
{
    public event Action<MusicTrackSO> TrackStarted;

    [Header("Global Settings")]
    [SerializeField, Range(0f, 1f)] private float maxVolume = 0.35f;
    [SerializeField] private int highscoreThreshold = 500;
    [SerializeField, Range(0f, 1f)] private float dangerRowThresholdPercentage = 0.6f;

    [Header("Audio Sources")]
    [SerializeField] private AudioSource audioSourceA;
    [SerializeField] private AudioSource audioSourceB;

    [Header("Tracks per State")]
    [SerializeField] private MusicTrackSO[] calmTracks;
    [SerializeField] private MusicTrackSO[] dangerTracks;
    [SerializeField] private MusicTrackSO[] highscoreTracks;

    private AudioSource currentSource;
    private AudioSource nextSource;
    private MusicTrackSO currentTrackSO;
    private Coroutine loopCoroutine;

    private List<MusicTrackSO> calmQueue;
    private List<MusicTrackSO> dangerQueue;
    private List<MusicTrackSO> highscoreQueue;

    private static readonly AnimationCurve defaultFadeCurve =
    AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

    private void Awake()
    {
        currentSource = audioSourceB;
        nextSource = audioSourceA;

        calmQueue = ShuffleList(calmTracks);
        dangerQueue = ShuffleList(dangerTracks);
        highscoreQueue = ShuffleList(highscoreTracks);
    }

    private void Start()
    {
        GameManager.Instance.OnTick += HandleTick;
        QueueTrack(calmTracks, isStateChange: true);
    }

    private void HandleTick()
    {
        int highestRow = GetHighestFilledRow();
        bool isDanger = highestRow >= Mathf.FloorToInt(GridManager.gridHeight * dangerRowThresholdPercentage);
        bool isHighscore = GameManager.Instance.score >= highscoreThreshold;

        if (isDanger && !ArrayContains(dangerTracks, currentTrackSO))
            QueueTrack(dangerTracks, true);
        else if (!isDanger && isHighscore && !ArrayContains(highscoreTracks, currentTrackSO))
            QueueTrack(highscoreTracks, true);
        else if (!isDanger && !isHighscore && !ArrayContains(calmTracks, currentTrackSO))
            QueueTrack(calmTracks, true);
    }

    public void QueueTrack(MusicTrackSO[] pool, bool isStateChange)
    {
        if (loopCoroutine != null)
            StopCoroutine(loopCoroutine);

        MusicTrackSO nextTrack;

        if (pool == calmTracks) nextTrack = DequeueNext(ref calmQueue, calmTracks);
        else if (pool == dangerTracks) nextTrack = DequeueNext(ref dangerQueue, dangerTracks);
        else if (pool == highscoreTracks) nextTrack = DequeueNext(ref highscoreQueue, highscoreTracks);
        else nextTrack = pool[UnityEngine.Random.Range(0, pool.Length)];

        StartCoroutine(CrossfadeTo(nextTrack, isStateChange));
    }

    private IEnumerator CrossfadeTo(MusicTrackSO nextTrack, bool isStateChange)
    {
        float duration = isStateChange
                            ? nextTrack.fadeStateChange
                            : nextTrack.fadeLoop;
        float startSec = isStateChange
                            ? nextTrack.songStartStateChange
                            : nextTrack.songStartAnew;
        float playOffset = Mathf.Max(0f, startSec - duration);
        float targetVol = nextTrack.volume * maxVolume;

        if (currentSource.isPlaying)
            StartCoroutine(FadeWithCurve(currentSource, duration, currentSource.volume, 0f));

        nextSource.clip = nextTrack.clip;
        nextSource.time = playOffset;
        nextSource.volume = 0f;
        nextSource.Play();

        StartCoroutine(FadeWithCurve(nextSource, duration, 0f, targetVol));

        currentTrackSO = nextTrack;
        TrackStarted?.Invoke(nextTrack);

        yield return new WaitForSeconds(duration);

        if (currentSource.isPlaying) currentSource.Stop();
        SwapSources();

        loopCoroutine = StartCoroutine(LoopAtEnd());
    }

    private IEnumerator LoopAtEnd()
    {
        MusicTrackSO[] pool = ArrayContains(calmTracks, currentTrackSO) ? calmTracks
                             : ArrayContains(dangerTracks, currentTrackSO) ? dangerTracks
                             : ArrayContains(highscoreTracks, currentTrackSO) ? highscoreTracks
                             : calmTracks;

        MusicTrackSO nextTrack =
            pool == calmTracks ? DequeueNext(ref calmQueue, calmTracks)
          : pool == dangerTracks ? DequeueNext(ref dangerQueue, dangerTracks)
          : pool == highscoreTracks ? DequeueNext(ref highscoreQueue, highscoreTracks)
          : pool[UnityEngine.Random.Range(0, pool.Length)];

        float duration = nextTrack.fadeLoop;
        float clipLen = currentSource.clip.length;
        float timeUntil = (clipLen - duration) - currentSource.time;
        if (timeUntil > 0f)
            yield return new WaitForSeconds(timeUntil);

        StartCoroutine(CrossfadeTo(nextTrack, isStateChange: false));
    }

    private IEnumerator FadeWithCurve(AudioSource src, float duration, float from, float to)
    {
        for (float t = 0f; t < duration; t += Time.deltaTime)
        {
            float norm = Mathf.Clamp01(t / duration);
            float eval = defaultFadeCurve.Evaluate(norm);
            src.volume = Mathf.LerpUnclamped(from, to, eval);
            yield return null;
        }
        src.volume = to;
    }


    private void SwapSources()
    {
        var tmp = currentSource;
        currentSource = nextSource;
        nextSource = tmp;
    }

    private bool ArrayContains(MusicTrackSO[] arr, MusicTrackSO so)
        => so != null && Array.IndexOf(arr, so) >= 0;

    private int GetHighestFilledRow()
    {
        for (int y = GridManager.gridHeight - 1; y >= 0; y--)
            for (int x = 0; x < GridManager.gridWidth; x++)
                if (GridManager.Instance.grid[x, y] != null)
                    return y;
        return 0;
    }

    private List<MusicTrackSO> ShuffleList(MusicTrackSO[] arr)
    {
        var list = new List<MusicTrackSO>(arr);
        for (int i = list.Count - 1; i > 0; i--)
        {
            int j = UnityEngine.Random.Range(0, i + 1);
            var tmp = list[i];
            list[i] = list[j];
            list[j] = tmp;
        }
        return list;
    }

    private MusicTrackSO DequeueNext(ref List<MusicTrackSO> queue, MusicTrackSO[] pool)
    {
        if (queue.Count == 0)
        {
            queue = ShuffleList(pool);

            if (queue.Count > 1 && queue[0] == currentTrackSO)
            {
                var tmp = queue[0];
                queue[0] = queue[1];
                queue[1] = tmp;
            }
        }

        var next = queue[0];
        queue.RemoveAt(0);
        return next;
    }
}
