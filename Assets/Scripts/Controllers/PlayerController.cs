using UnityEngine;

public class PlayerController : MonoBehaviour, CharacterController
{
    public CharacterStats Player;
    public float moveLenght = 5f;
    private bool isPlayerTurn = true;

    private void Awake()
    {
        Player = GameManager.Instance.Player;
    }

    public void TakeTurn()
    {
        isPlayerTurn = true;
    }
    public void MoveRight()
    {
        // Move player to the right
        transform.Translate(Vector3.right * moveLenght * Time.deltaTime);
        Debug.Log("Player moves right");
    }

    public void MoveLeft()
    {
        // Move player to the left
        transform.Translate(Vector3.left * moveLenght * Time.deltaTime);
        Debug.Log("Player moves left");
    }

    public void Sleep()
    {
        // Restore 40 stamina when the player sleeps
        Player.RestoreStamina();
        Debug.Log("Player rests and restores stamina");
    }

    public void Block()
    {
        // For this example, let's assume the player blocks and reduces the damage by 50%
        double blockValue = 0.5 + (Player.Agility * 0.5);
        if (blockValue > 0.8) 
        {
            blockValue = 0.8;
        }
        Debug.Log($"Player blocks the attack - damage reduced by {blockValue}");
    }

    public void AttackLight()
    {
        // Light attack = strength * 1 (as per the stats provided)
        float damage = Player.Strenght * 1;
        Debug.Log($"Player performs a light attack and deals {damage} damage");
    }

    public void AttackMedium()
    {
        // Medium attack = strength * 1.5 (as per the stats provided)
        float damage = Player.Strenght * 1.5f;
        Debug.Log($"Player performs a medium attack and deals {damage} damage");
    }

    public void AttackStrong()
    {
        // Strong attack = strength * 2 (as per the stats provided)
        float damage = Player.Strenght * 2;
        Debug.Log($"Player performs a strong attack and deals {damage} damage");
    }

    public void Dodge()
    {
        // Dodge chance based on agility (Zwinność)
        float dodgeChance = 0.1f * (Player.Agility - GameManager.Instance.Enemies[GameManager.Instance.CurrentEnemy].Agility);
        if (Random.value < dodgeChance)
        {
            Debug.Log($"Player successfully dodges the attack{dodgeChance}");
        }
        else
        {
            Debug.Log("Player fails to dodge the attack");
        }
    }
}
