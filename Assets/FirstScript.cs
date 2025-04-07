using UnityEngine;

public class FirstSscript : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        string cubeName = this.gameObject.name;
        Debug.Log($"{cubeName} init");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
