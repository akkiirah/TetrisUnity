using System;
using UnityEngine;

public class Shape : MonoBehaviour
{
    public event Action OnLocked;
    
    [SerializeField] public float speed = 1;
    private bool isLocked = false;

    public void MoveDown()
    {
        int currentY = Mathf.RoundToInt(transform.position.y);

        if (currentY > 0)
        {
            transform.position = new Vector3(
                Mathf.Round(transform.position.x),
                currentY - 1,
                transform.position.z
            );
        }
        else
        {
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
}
