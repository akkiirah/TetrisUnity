using System;
using UnityEngine;

public class Shape : MonoBehaviour
{
    public event Action OnLocked;
    
    [SerializeField] public float speed = 1;
    private bool isLocked = false;

    public void MoveDown()
    {
        // Bewege das Tetromino um eine Einheit nach unten
        transform.position += new Vector3(0, -1, 0);

        // Ist die neue Position ungültig?
        if (!IsValidPosition())
        {
            // Bewegung zurücksetzen
            transform.position += new Vector3(0, 1, 0);
            LockTetromino();
        }
    }

    public void LockTetromino()
    {
        isLocked = true;
        
        foreach (Transform child in transform)
        {
            GridManager.Instance.AddBlockToGrid(child);
        }

        OnLocked?.Invoke();
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
