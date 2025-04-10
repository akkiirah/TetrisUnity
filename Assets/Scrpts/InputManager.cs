using UnityEngine;

public class InputManager : MonoBehaviour
{
    private readonly float repeatDelay = .5f;
    private readonly float repeatRate = 0.05f;
    private float lastRepeatTime;
    private bool sKeyIsHeld;

    private float sKeyPressTime;
    public static InputManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(gameObject);
        else
            Instance = this;
    }

    private void Update()
    {
        var horizontalInput = GetInputHorizontal();
        var verticalInput = GetInputVertical();
        var rotateInput = GetRotate();

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

        if (rotateInput) ShapeManager.Instance.RotateShape();
    }

    public int GetInputHorizontal()
    {
        var horizontalInput = Input.GetKeyDown(KeyCode.D) ? 1 : Input.GetKeyDown(KeyCode.A) ? -1 : 0;

        return horizontalInput;
    }

    public int GetInputVertical()
    {
        var verticalInput = 0;

        if (Input.GetKeyDown(KeyCode.S))
        {
            verticalInput = -1;
            sKeyPressTime = Time.time;
            sKeyIsHeld = true;
            lastRepeatTime = Time.time;
        }
        else if (Input.GetKey(KeyCode.S))
        {
            if (sKeyIsHeld)
                if (Time.time - sKeyPressTime >= repeatDelay)
                    if (Time.time - lastRepeatTime >= repeatRate)
                    {
                        verticalInput = -1;
                        lastRepeatTime = Time.time;
                    }
        }
        else if (Input.GetKeyUp(KeyCode.S))
        {
            sKeyIsHeld = false;
        }

        return verticalInput;
    }

    public bool GetRotate()
    {
        return Input.GetKeyDown(KeyCode.Space);
    }
}