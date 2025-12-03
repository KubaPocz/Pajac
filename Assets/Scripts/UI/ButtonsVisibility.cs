using UnityEngine;

public class ButtonsVisibility : MonoBehaviour
{
    public GameObject SubAttacks;
    public GameObject SubMoves;
    private void Start()
    {
        HideAttacks();
        HideMoves();
    }
    public void HideAll()
    {
        HideMoves();
        HideAttacks();
    }
    public void HideAttacks()
    {
        SubAttacks.SetActive(false);
    }
    public void ShowAttacks()
    {
        SubAttacks.SetActive(true);
    }
    public void HideMoves()
    {
        SubMoves.SetActive(false);
    }
    public void ShowMoves()
    {
        SubMoves.SetActive(true);
    }

}
