using System;
using UnityEngine;

public class ShapeManager : Singleton<ShapeManager>
{
    [SerializeField] public GameObject[] tetrominoPrefabs;
    [SerializeField] private Transform shapesContainer;
    [SerializeField] private GameObject borderContainer;

    public Shape activeShape;
    public GameObject nextShape { get; private set; }
    public Vector3 spawnPosition = new Vector3(5, 18, 0);

    public event Action OnShapeMove;
    public event Action OnSpawn;

    private void Start()
    {
        GameManager.Instance.OnTick += HandleTick;
    }

    private void HandleTick()
    {
        if (activeShape)
        {
            activeShape.MoveDown();
        }
        else
        {
            SpawnNewTetromino();
        }
    }

    public void ChangeBorderMaterial()
    {
        MeshRenderer activeShapeMaterial = activeShape.transform.GetChild(0).transform.GetChild(0).GetComponent<MeshRenderer>();
        Material clonedMat = new Material(activeShapeMaterial.sharedMaterial);

        Color currentClr = clonedMat.GetColor("_Color");
        Color newEmission = currentClr * 4.6f;

        //clonedMat.SetColor("_EmissionColor", newEmission);
        borderContainer.GetComponent<MeshRenderer>().material = clonedMat;
    }

    public void SpawnNewTetromino()
    {
        GameObject currentPrefab = (nextShape != null)
            ? nextShape
            : tetrominoPrefabs[UnityEngine.Random.Range(0, tetrominoPrefabs.Length)];

        GameObject tetromino = Instantiate(currentPrefab, spawnPosition, Quaternion.identity, shapesContainer);
        nextShape = tetrominoPrefabs[UnityEngine.Random.Range(0, tetrominoPrefabs.Length)];

        Debug.Log(nextShape);

        if (!GridManager.Instance.IsValidPosition(tetromino.transform))
        {
            GameManager.Instance.GameOver();
            Destroy(tetromino);
            return;
        }

        Shape tetrominoScript = tetromino.GetComponent<Shape>();
        activeShape = tetrominoScript;
        ChangeBorderMaterial();

        tetrominoScript.OnLocked += GameManager.Instance.TetrominoLocked;
        tetrominoScript.OnLocked += GridManager.Instance.HandleShapeLocked;
        OnSpawn?.Invoke();
    }

    public void MoveShapeDown()
    {
        activeShape.MoveDown();
        OnShapeMove?.Invoke();
    }

    public void MoveShapeHorizontal(int dir)
    {
        activeShape.MoveHorizontal(dir);
        OnShapeMove?.Invoke();
    }

    public void RotateShape()
    {
        activeShape.Rotate();
        OnShapeMove?.Invoke();
    }
}