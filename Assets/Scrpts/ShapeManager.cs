using UnityEngine;

public class ShapeManager : MonoBehaviour
{
    public static ShapeManager Instance { get; private set; }
    
    [SerializeField] public GameObject[] tetrominoPrefabs;
    public Shape activeShape;
    public Vector3 spawnPosition = new Vector3(5, 18, 0);

    void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(gameObject);
        else
            Instance = this;

        GameManager.Instance.OnTick += HandleTick;
    }

    void Start()
    {
        SpawnNewTetromino();
    }

    private void HandleTick()
    {
        if(activeShape != null)
            activeShape.MoveDown();
    }

    public void SpawnNewTetromino()
    {
        int i = UnityEngine.Random.Range(0, tetrominoPrefabs.Length);
        GameObject tetromino = Instantiate(tetrominoPrefabs[i], spawnPosition, Quaternion.identity);

        if (!GridManager.Instance.IsValidPosition(tetromino.transform))
        {
            GameManager.Instance.GameOver();
            Destroy(tetromino);
        }

        Shape tetrominoScript = tetromino.GetComponent<Shape>();
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
