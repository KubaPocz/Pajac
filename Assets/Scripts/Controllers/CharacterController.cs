using UnityEngine.Scripting.APIUpdating;

public interface CharacterController
{
    public void TakeTurn();
    public  void MoveRight();
    public  void MoveLeft();
    public  void Sleep();
    public  void Block();
    public  void AttackLight();
    public  void AttackMedium();
    public  void AttackStrong();
    public  void Dodge();
}