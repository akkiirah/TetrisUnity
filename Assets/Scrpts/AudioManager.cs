using UnityEngine;
using System.Collections;

public class AudioManager : Singleton<AudioManager>
{
    [SerializeField] private float maxVolume = .35f;
    [SerializeField] private int highscoreThreshold = 500;
    [SerializeField] private float panicRowThresholdPercentage = .6f;

    [Header("Audio Sources for Crossfade")]
    public AudioSource audioSourceA;
    public AudioSource audioSourceB;

    [Header("Tracks per State (Scriptable Objects)")]
    public MusicTrackSO[] calmTracks;
    public MusicTrackSO[] dangerTracks;
    public MusicTrackSO[] highscoreTracks;

    private AudioSource currentSource;
    private AudioSource nextSource;
    private MusicTrackSO currentTrackSO;
    private Coroutine playRoutine;

    private enum MusicState { Calm, Danger, Highscore }
    private MusicState currentState;
    private int panicRowThreshold;
    private int[] lastClipIndex = new int[3] { -1, -1, -1 };

    void Awake()
    {
        currentSource = audioSourceA;
        nextSource = audioSourceB;
        currentTrackSO = null;
        panicRowThreshold = Mathf.FloorToInt(GridManager.gridHeight * panicRowThresholdPercentage);
    }

    void Start()
    {
        // 1) State auf Calm setzen und eine zufällige Calm-Spur wählen
        currentState = MusicState.Calm;
        int initIndex = Random.Range(0, calmTracks.Length);
        currentTrackSO = calmTracks[initIndex];

        // 2) currentSource (Audio A) damit füllen und starten
        currentSource.clip = currentTrackSO.clip;
        currentSource.time = currentTrackSO.songStartStateChange;
        currentSource.volume = 0f;
        currentSource.loop = false;
        currentSource.Play();

        // 3) Sofort ein Fade-In (StateChange)
        StartCoroutine(FadeIn(
            currentSource,
            currentTrackSO.fadeInStateChange,
            maxVolume * currentTrackSO.volume
        ));

        // 4) Jetzt die Scheduling-Routine für den „nächsten“ Track anstoßen
        playRoutine = StartCoroutine(PlayAndSchedule(currentTrackSO, isStateChange: false));
    }

    void Update()
    {
        var gm = GameManager.Instance;
        int highestRow = GetHighestFilledRow();
        MusicState desired = (highestRow >= panicRowThreshold)
            ? MusicState.Danger
            : (gm.score > highscoreThreshold
                ? MusicState.Highscore
                : MusicState.Calm);

        if (desired != currentState)
            PlayRandomFromState(desired, true);
    }

    private int GetHighestFilledRow()
    {
        for (int y = GridManager.gridHeight - 1; y >= 0; y--)
            for (int x = 0; x < GridManager.gridWidth; x++)
                if (GridManager.Instance.grid[x, y] != null)
                    return y;
        return 0;
    }

    private MusicTrackSO[] TracksForState(MusicState state)
    {
        switch (state)
        {
            case MusicState.Danger: return dangerTracks;
            case MusicState.Highscore: return highscoreTracks;
            default: return calmTracks;
        }
    }

    private void PlayRandomFromState(MusicState state, bool isStateChange)
    {
        currentState = state;
        var tracks = TracksForState(state);
        if (tracks == null || tracks.Length == 0)
        {
            Debug.LogWarning($"Keine Tracks für State {state}");
            return;
        }

        // neuen Index wählen (nicht derselbe wie zuletzt)
        int newIndex = Random.Range(0, tracks.Length);
        while (tracks.Length > 1 && newIndex == lastClipIndex[(int)state])
            newIndex = Random.Range(0, tracks.Length);
        lastClipIndex[(int)state] = newIndex;

        // nur beim echten State-Change das laufende Scheduler-Coroutine stoppen
        if (isStateChange && playRoutine != null)
            StopCoroutine(playRoutine);

        // neues Scheduling starten
        playRoutine = StartCoroutine(PlayAndSchedule(tracks[newIndex], isStateChange));
    }

    private IEnumerator PlayAndSchedule(MusicTrackSO toSO, bool isStateChange)
    {
        // 1) Setup
        var fromSO = currentTrackSO;
        var fromSource = currentSource;
        var toSource = nextSource;

        float startTime = isStateChange
            ? toSO.songStartStateChange
            : toSO.songStartAnew;

        // Neue Quelle vorbereiten
        toSource.clip = toSO.clip;
        toSource.time = startTime;
        toSource.volume = 0f;
        toSource.loop = false;
        toSource.Play();

        // 2) Sofort-Crossfade bei State-Change
        if (isStateChange && fromSO != null)
        {
            StartCoroutine(FadeOut(fromSource, fromSO.fadeOutTime));
            StartCoroutine(FadeIn(toSource, toSO.fadeInStateChange, maxVolume * toSO.volume));
        }

        // 3) Track bis zum Ende minus Fade-Out-Time von toSO spielen
        float fullPlayTime = toSO.clip.length - startTime;
        float fadeOutDur = toSO.fadeOutTime;
        float playUntil = Mathf.Max(0f, fullPlayTime - fadeOutDur);
        yield return new WaitForSeconds(playUntil);

        // 4) Crossfade am echten Track-Ende: toSO fadeOut + nächster fadeIn
        //    alten Track (toSO) ausfaden ...
        StartCoroutine(FadeOut(toSource, fadeOutDur));

        //    nächsten Track (selber State) auswählen
        var tracks = TracksForState(currentState);
        int idx = Random.Range(0, tracks.Length);
        if (tracks.Length > 1)
            while (idx == lastClipIndex[(int)currentState])
                idx = Random.Range(0, tracks.Length);
        lastClipIndex[(int)currentState] = idx;
        var nextSO = tracks[idx];

        //    neue Quelle vorbereiten und einfaden
        fromSource.clip = nextSO.clip;
        fromSource.time = nextSO.songStartAnew;
        fromSource.volume = 0f;
        fromSource.loop = false;
        fromSource.Play();

        StartCoroutine(FadeIn(
            fromSource,
            nextSO.fadeInAnew,
            maxVolume * nextSO.volume
        ));

        // 5) Warten, bis das längste Fade durch ist
        yield return new WaitForSeconds(Mathf.Max(fadeOutDur, nextSO.fadeInAnew));

        // 6) Aufräumen & rekursiv weitermachen
        toSource.Stop();
        currentTrackSO = nextSO;
        // Quellen tauschen, damit nextSource wieder frei ist
        (currentSource, nextSource) = (fromSource, toSource);

        // Jetzt kommt die nächste Iteration – kein State-Change
        playRoutine = StartCoroutine(PlayAndSchedule(nextSO, false));
    }

    private IEnumerator FadeOut(AudioSource src, float duration)
    {
        float startVol = src.volume, t = 0f;
        while (t < duration)
        {
            t += Time.deltaTime;
            src.volume = Mathf.Lerp(startVol, 0f, t / duration);
            yield return null;
        }
        src.volume = 0f;
        src.Stop();
    }

    private IEnumerator FadeIn(AudioSource src, float duration, float targetVol)
    {
        float t = 0f;
        while (t < duration)
        {
            t += Time.deltaTime;
            src.volume = Mathf.Lerp(0f, targetVol, t / duration);
            yield return null;
        }
        src.volume = targetVol;
    }
}
