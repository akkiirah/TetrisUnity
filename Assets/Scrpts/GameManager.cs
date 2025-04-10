using System;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [SerializeField] public float tickInterval = 1f;
    public float tickTimer = 0f;

    public event Action OnTick;
    public event Action OnInputTick;

    void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(gameObject);
        else
            Instance = this;

    }

    public void TetrominoLocked()
    {
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
