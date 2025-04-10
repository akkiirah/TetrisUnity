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
        int x = (int)pos.x;
        int y = (int)pos.y;

        if (x < 0 || x >= gridWidth || y < 0 || y >= gridHeight)
        {
            Debug.LogWarning("Block position out of grid bounds: " + pos);
            return;
        }

        grid[x, y] = block;
    }

    public static Vector3 RoundVector3(Vector3 pos)
    {
        return new Vector3(Mathf.Round(pos.x), Mathf.Round(pos.y), Mathf.Round(pos.z));
    }

    public static bool IsInsideGrid(Vector3 pos)
    {
        int x = Mathf.RoundToInt(pos.x);
        int y = Mathf.RoundToInt(pos.y);
        return x >= 0 && x < gridWidth && y >= 0;
    }

    public bool IsValidPosition()
    {
        foreach (Transform child in transform)
        {
            Vector3 pos = GridManager.RoundVector3(child.position);

            if (!GridManager.IsInsideGrid(pos))
                return false;

            int x = (int)pos.x;
            int y = (int)pos.y;
            if (y < GridManager.gridHeight && GridManager.grid[x, y] != null)
                return false;
        }
        return true;
    }

}
