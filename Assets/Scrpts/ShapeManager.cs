using UnityEngine;

public class ShapeManager : MonoBehaviour
{
    [SerializeField] public GameObject[] tetrominoPrefabs;
    public Shape activeShape;
    public Vector3 spawnPosition = new(5, 18, 0);
    public static ShapeManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(gameObject);
        else
            Instance = this;

        GameManager.Instance.OnTick += HandleTick;
    }

    private void Start()
    {
        SpawnNewTetromino();
    }

    private void HandleTick()
    {
        if (activeShape != null)
            activeShape.MoveDown();
    }

    public void SpawnNewTetromino()
    {
        var i = Random.Range(0, tetrominoPrefabs.Length);
        var tetromino = Instantiate(tetrominoPrefabs[i], spawnPosition, Quaternion.identity);

        if (!GridManager.Instance.IsValidPosition(tetromino.transform))
        {
            GameManager.Instance.GameOver();
            Destroy(tetromino);
        }

        var tetrominoScript = tetromino.GetComponent<Shape>();
        activeShape = tetrominoScript;

        tetrominoScript.OnLocked += GameManager.Instance.TetrominoLocked;
    }

    public void MoveShapeDown()
    {
        activeShape.MoveDown();
    }

    public void MoveShapeHorizontal(int dir)
    {
        activeShape.MoveHorizontal(dir);
    }

    public void RotateShape()
    {
        activeShape.Rotate();
    }
}