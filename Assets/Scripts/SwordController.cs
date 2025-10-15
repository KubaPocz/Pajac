using UnityEngine;
using UnityEngine.UI;

public class SwordController : MonoBehaviour
{
    public void SetVisibility(bool isVisible)
    {
        gameObject.SetActive(isVisible);
    }
}
