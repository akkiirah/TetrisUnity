using System;
using UnityEngine;

public class Shape : MonoBehaviour
{
    [SerializeField] public float speed = 1;
    private bool isLocked;
    public event Action OnLocked;

    public void MoveDown()
    {
        transform.position += new Vector3(0, -1, 0);

        if (!IsValidPosition())
        {
            transform.position += new Vector3(0, 1, 0);
            LockTetromino();
        }
    }

    public void MoveHorizontal(int dir)
    {
        transform.position += new Vector3(dir, 0, 0);

        if (!IsValidPosition()) transform.position += new Vector3(dir * -1, 0, 0);
    }

    public void Rotate()
    {
        transform.Rotate(new Vector3(0, 0, 90));

        if (!IsValidPosition()) transform.Rotate(new Vector3(0, 0, -90));
    }

    public void LockTetromino()
    {
        isLocked = true;

        foreach (Transform child in transform) GridManager.Instance.AddBlockToGrid(child);

        OnLocked?.Invoke();
    }

    public bool IsValidPosition()
    {
        foreach (Transform child in transform)
        {
            var pos = GridManager.RoundVector3(child.position);

            if (!GridManager.IsInsideGrid(pos))
                return false;

            var x = (int)pos.x;
            var y = (int)pos.y;
            if (y < GridManager.gridHeight && GridManager.grid[x, y] != null)
                return false;
        }

        return true;
    }
}