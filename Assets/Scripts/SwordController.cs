using UnityEngine;

public class SwordController : MonoBehaviour
{
    public void SetVisibility(bool isVisible)
    {
        gameObject.SetActive(isVisible);
    }
}
