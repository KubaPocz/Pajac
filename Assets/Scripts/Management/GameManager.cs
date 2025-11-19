using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public CharacterStats Player;
<<<<<<< Updated upstream
    public CharacterStats[] Enemies;
    public int CurrentEnemy = 0;

    //public CharacterStats Monkey;
    //public CharacterStats Elephant;
    //public CharacterStats Clown;
    //public CharacterStats Boss;
    
=======

    public CharacterStats[] Enemies;

>>>>>>> Stashed changes
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
