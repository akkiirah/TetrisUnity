using System;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [SerializeField] public float tickInterval = 1f;
    private float tickTimer = 0f;

    public event Action OnTick;

    void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(gameObject);
        else
            Instance = this;
    }

    // Diese Methode wird aufgerufen, wenn ein Tetromino gelockt wurde.
    public void TetrominoLocked()
    {
        // Hier kannst du z.B. Punkte berechnen, Reihen überprüfen, Effekte auslösen etc.
        // Dann das nächste Tetromino spawnen:
        ShapeManager.Instance.SpawnNewTetromino();
    }

    void Update()
    {
        tickTimer += Time.deltaTime;
        if (tickTimer >= tickInterval)
        {
            tickTimer = 0f;
            OnTick?.Invoke();
        }
    }
}
