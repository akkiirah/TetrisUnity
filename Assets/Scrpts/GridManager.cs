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
    public bool IsValidPosition(Transform _transform)
    {
        foreach (Transform child in _transform)
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

    public bool IsRowFull(int y)
    {
        for (int x = 0; x < gridWidth; x++)
        {
            if (grid[x, y] == null)
            {
                return false;
            }
        }
        return true;
    }

    public void DeleteRow(int y)
    {
        for (int x = 0; x < gridWidth; x++)
        {
            if (grid[x, y] != null)
            {
                Destroy(grid[x, y].gameObject);
                grid[x, y] = null;
            }
        }
    }

    public void MoveRowsDown(int startY)
    {
        for (int y = startY; y < gridHeight; y++)
        {
            for (int x = 0; x < gridWidth; x++)
            {
                if (grid[x, y] != null)
                {
                    // Aktualisiere die Grid-Datenstruktur:
                    grid[x, y - 1] = grid[x, y];
                    grid[x, y] = null;

                    // Verschiebe den Block visuell um 1 Einheit nach unten
                    grid[x, y - 1].position += new Vector3(0, -1, 0);
                }
            }
        }
    }

    public void DeleteFullRows()
    {
        for (int y = 0; y < gridHeight; y++)
        {
            if (IsRowFull(y))
            {
                DeleteRow(y);
                MoveRowsDown(y + 1);
                // Nachdem die Reihen nachgezogen wurden, muss derselbe Index erneut geprüft werden,
                // da nun eine neue Reihe an dieser Stelle stehen könnte.
                y--;
            }
        }
    }

    void Update()
    {
        DeleteFullRows();
    }
}
