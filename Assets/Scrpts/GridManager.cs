using System;
using UnityEngine;

public class GridManager : Singleton<GridManager>
{
    public static int gridWidth = 10;
    public static int gridHeight = 20;

    public Transform[,] grid = new Transform[gridWidth, gridHeight];

    public event Action OnLinesDeleted;

    public void HandleShapeLocked()
    {
        DeleteFullRows();
    }

    public void AddBlockToGrid(Transform block)
    {
        var pos = RoundVector3(block.position);
        var x = (int)pos.x;
        var y = (int)pos.y;

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
        var x = Mathf.RoundToInt(pos.x);
        var y = Mathf.RoundToInt(pos.y);
        return x >= 0 && x < gridWidth && y >= 0;
    }

    public bool IsValidPosition(Transform _transform)
    {
        foreach (Transform child in _transform)
        {
            var pos = RoundVector3(child.position);
            if (!IsInsideGrid(pos))
                return false;
            int x = (int)pos.x;
            int y = (int)pos.y;
            if (y < gridHeight && grid[x, y] != null)
                return false;
        }
        return true;
    }

    public bool IsRowFull(int y)
    {
        for (int x = 0; x < gridWidth; x++)
        {
            if (grid[x, y] == null)
                return false;
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
                    grid[x, y - 1] = grid[x, y];
                    grid[x, y] = null;
                    grid[x, y - 1].position += new Vector3(0, -1, 0);
                }
            }
        }
    }

    public void DeleteFullRows()
    {
        int rowCount = 0;
        int mod = 0;

        for (int y = 0; y < gridHeight; y++)
        {
            if (IsRowFull(y))
            {
                DeleteRow(y);
                MoveRowsDown(y + 1);
                y--;
                rowCount++;
            }
        }

        int score = 100 << (rowCount - 1);

        GameManager.Instance.score += score;
        OnLinesDeleted?.Invoke();
    }
}
