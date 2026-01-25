using UnityEngine;

public class DayCycleSoundManager : MonoBehaviour
{
    public void StartWindUp()
    {
        AudioManager.StartWindUpLoop();
    }

    public void StopWindUp()
    {
        AudioManager.StopWindUpLoop();
    }
}
