using UnityEngine;

public class InputManager : Singleton<InputManager>
{
    private readonly float repeatDelay = 0.5f;
    private readonly float repeatRate = 0.05f;
    private float lastRepeatTime;
    private bool sKeyIsHeld;
    private float sKeyPressTime;

    private void Update()
    {
        int horizontalInput = GetInputHorizontal();
        int verticalInput = GetInputVertical();
        bool rotateInput = GetRotate();

        if (horizontalInput != 0)
        {
            if (horizontalInput == -1)
                ShapeManager.Instance.MoveShapeHorizontal(-1);
            else if (horizontalInput == 1)
                ShapeManager.Instance.MoveShapeHorizontal(1);
        }

        if (verticalInput == -1)
            ShapeManager.Instance.MoveShapeDown();

        if (rotateInput)
            ShapeManager.Instance.RotateShape();
    }

    public int GetInputHorizontal()
    {
        return Input.GetKeyDown(KeyCode.D) ? 1 : Input.GetKeyDown(KeyCode.A) ? -1 : 0;
    }

    public int GetInputVertical()
    {
        int verticalInput = 0;
        if (Input.GetKeyDown(KeyCode.S))
        {
            verticalInput = -1;
            sKeyPressTime = Time.time;
            sKeyIsHeld = true;
            lastRepeatTime = Time.time;
        }
        else if (Input.GetKey(KeyCode.S) && sKeyIsHeld)
        {
            if (Time.time - sKeyPressTime >= repeatDelay && Time.time - lastRepeatTime >= repeatRate)
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
