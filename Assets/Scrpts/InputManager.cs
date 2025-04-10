using UnityEngine;

public class InputManager : MonoBehaviour
{
    public static InputManager Instance { get; private set; }

    void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(gameObject);
        else
            Instance = this;
    }

    public int GetInputHorizontal()
    {
        int horizontalInput = Input.GetKeyDown(KeyCode.D) ? 1 : (Input.GetKeyDown(KeyCode.A) ? -1 : 0);

        return horizontalInput;
    }

    public int GetInputVertical()
    {
        int verticalInput = Input.GetKeyDown(KeyCode.W) ? 1 : (Input.GetKeyDown(KeyCode.S) ? -1 : 0);

        return verticalInput;
    }

    public bool GetRotate()
    {
        return Input.GetKeyDown(KeyCode.Space);
    }

    void Update()
    {
        int horizontalInput = GetInputHorizontal();
        int verticalInput = GetInputVertical();
        bool rotateInput = GetRotate();

        if (horizontalInput != 0 || verticalInput != 0)
        {
            switch (horizontalInput)
            {
                case -1:
                    ShapeManager.Instance.MoveShapeHorizontal(-1);
                    break;
                case 1:
                    ShapeManager.Instance.MoveShapeHorizontal(1);
                    break;
            }
            switch (verticalInput)
            {
                case -1:
                    ShapeManager.Instance.MoveShapeDown();
                    break;
            }
        }
        if (rotateInput)
        {
            ShapeManager.Instance.RotateShape();
        }
    }
}

