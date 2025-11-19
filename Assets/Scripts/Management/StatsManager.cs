using UnityEngine;

public class StatsManager : MonoBehaviour
{
    public static StatsManager Instance;
    public CharacterInfo player;
    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }
    private void Start()
    {
        player = GameManager.Instance.Player;
    }
}
