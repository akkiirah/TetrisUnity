using UnityEngine;

public class ShapeManager : MonoBehaviour
{
    public static ShapeManager Instance { get; private set; }
    
    [SerializeField] public GameObject[] tetrominoPrefabs;
    public Shape activeShape;

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
        if (activeShape != null)
            activeShape.MoveDown();
    }

    public void SpawnNewTetromino()
    {
        int i = UnityEngine.Random.Range(0, tetrominoPrefabs.Length);
        GameObject tetromino = Instantiate(tetrominoPrefabs[i], new Vector3(5, 20, 0), Quaternion.identity);
        Shape tetrominoScript = tetromino.GetComponent<Shape>();
        activeShape = tetrominoScript;

        // Abonniere das OnLocked-Event, sodass der GameManager benachrichtigt wird, wenn das Tetromino fixiert.
        tetrominoScript.OnLocked += GameManager.Instance.TetrominoLocked;
    }
}
