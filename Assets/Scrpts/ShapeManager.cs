using System;
using UnityEngine;

public class ShapeManager : Singleton<ShapeManager>
{
    [SerializeField] public GameObject[] tetrominoPrefabs;
    [SerializeField] private Transform shapesContainer;

    public Shape activeShape;
    public Vector3 spawnPosition = new Vector3(5, 18, 0);

    public event Action OnShapeMove;

    private void Start()
    {
        GameManager.Instance.OnTick += HandleTick;
        SpawnNewTetromino();
    }

    private void HandleTick()
    {
        if (activeShape != null)
            activeShape.MoveDown();
    }

    public void SpawnNewTetromino()
    {
        int i = UnityEngine.Random.Range(0, tetrominoPrefabs.Length);
        GameObject tetromino = Instantiate(tetrominoPrefabs[i], spawnPosition, Quaternion.identity, shapesContainer);

        if (!GridManager.Instance.IsValidPosition(tetromino.transform))
        {
            GameManager.Instance.GameOver();
            Destroy(tetromino);
            return;
        }

        Shape tetrominoScript = tetromino.GetComponent<Shape>();
        activeShape = tetrominoScript;

        tetrominoScript.OnLocked += GameManager.Instance.TetrominoLocked;
    }

    public void MoveShapeDown()
    {
        activeShape.MoveDown();
        OnShapeMove?.Invoke();
    }

    public void MoveShapeHorizontal(int dir)
    {
        activeShape.MoveHorizontal(dir);
        OnShapeMove?.Invoke();
    }

    public void RotateShape()
    {
        activeShape.Rotate();
        OnShapeMove?.Invoke();
    }
}
