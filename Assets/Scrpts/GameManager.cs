using System;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    public ReflectionProbe reflectionProbe;
    public float tickInterval = 1f;
    private float tickTimer;

    public event Action OnTick;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        ShapeManager.Instance.OnShapeMove += HandleShapeMove;
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

    public void TetrominoLocked()
    {
        ShapeManager.Instance.SpawnNewTetromino();
    }
    public void HandleShapeMove()
    {
        reflectionProbe.RenderProbe();
    }

    public void GameOver()
    {
        InputManager.Instance.enabled = false;
        GridManager.Instance.enabled = false;
        ShapeManager.Instance.enabled = false;
        Debug.Log("GameOver");
    }
}
