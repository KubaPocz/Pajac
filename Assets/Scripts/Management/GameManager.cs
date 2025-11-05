using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public CharacterInfo Player;
    public CharacterInfo Monkey;
    public CharacterInfo Elephant;
    public CharacterInfo Clown;
    public CharacterInfo Boss;
    
    public void Awake()
    {
        if(Instance != null)
        {
            Destroy(this);
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

}
