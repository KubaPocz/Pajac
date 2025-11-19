using UnityEngine;

public class BattleManager : MonoBehaviour
{
    public static BattleManager Instance;

    private void Awake()
    {
        if(Instance != null)
        {
            Destroy(gameObject);
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }
}
