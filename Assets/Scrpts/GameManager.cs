using System;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public ReflectionProbe reflectionProbe;
    public float tickInterval = 1f;
    private float tickTimer;
    public static GameManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(gameObject);
        else
            Instance = this;
    }

    private void Update()
    {
        tickTimer += Time.deltaTime;
        if (tickTimer >= tickInterval)
        {
            tickTimer = 0f;
            reflectionProbe.RenderProbe();
            OnTick?.Invoke();
        }
    }

    public event Action OnTick;

    public void TetrominoLocked()
    {
        ShapeManager.Instance.SpawnNewTetromino();
    }

    public void GameOver()
    {
        InputManager.Instance.enabled = false;
        GridManager.Instance.enabled = false;
        ShapeManager.Instance.enabled = false;
        Debug.Log("GameOver");
    }
}