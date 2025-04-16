using UnityEngine;

public class ShowNextScript : MonoBehaviour
{
    private GameObject currentPrefab;
    private GameObject newPrefab;

    void Start()
    {
        ShapeManager.Instance.OnSpawn += HandleSpawn;
    }

    void HandleSpawn()
    {
        if (currentPrefab)
        {
            Destroy(currentPrefab);
        }
        newPrefab = ShapeManager.Instance.nextShape;
        GameObject tetromino = Instantiate(newPrefab, gameObject.transform.position, Quaternion.identity, gameObject.transform);
        currentPrefab = tetromino;
        Debug.Log(currentPrefab);
    }
}