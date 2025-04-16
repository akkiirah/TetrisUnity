using TMPro;
using UnityEngine;

public class UIManager : Singleton<UIManager>
{
    [SerializeField] private TextMeshProUGUI scoreText;

    void Start()
    {
        GridManager.Instance.OnLinesDeleted += HandleScore;
    }

    void HandleScore()
    {
        scoreText.text = $"Score: {GameManager.Instance.score}";
    }
}
