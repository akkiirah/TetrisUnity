using UnityEngine;

public class GridManager : MonoBehaviour
{
    public static GridManager Instance;

    public static int gridWidth = 10;
    public static int gridHeight = 20;

    public static Transform[,] grid = new Transform[gridWidth, gridHeight];

    void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(gameObject);
        else
            Instance = this;
    }

    public void AddBlockToGrid(Transform block)
    {
        Vector3 pos = RoundVector3(block.position);
        grid[(int)pos.x, (int)pos.y] = block;
    }

    public static Vector3 RoundVector3(Vector3 pos)
    {
        return new Vector3(Mathf.Round(pos.x), Mathf.Round(pos.y), Mathf.Round(pos.z));
    }
}
