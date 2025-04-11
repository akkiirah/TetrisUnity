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
        if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
        {
            return 1;
        }
        else if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
        {
            return -1;
        }
        return 0;
    }


    public int GetInputVertical()
    {
        int verticalInput = 0;
        if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
        {
            verticalInput = -1;
            sKeyPressTime = Time.time;
            sKeyIsHeld = true;
            lastRepeatTime = Time.time;
        }
        else if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow) && sKeyIsHeld)
        {
            if (Time.time - sKeyPressTime >= repeatDelay && Time.time - lastRepeatTime >= repeatRate)
            {
                verticalInput = -1;
                lastRepeatTime = Time.time;
            }
        }
        else if (Input.GetKeyUp(KeyCode.S) || Input.GetKeyUp(KeyCode.DownArrow))
        {
            sKeyIsHeld = false;
        }
        return verticalInput;
    }

    public bool GetRotate()
    {
        return Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.UpArrow);
    }
}
