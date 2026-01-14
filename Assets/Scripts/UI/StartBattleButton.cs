using UnityEngine;
using UnityEngine.UI;

public class StartBattleButton : MonoBehaviour
{
    public Sprite[] FightButtons;
    private Image buttonImage;
    void Start()
    {
        buttonImage = GetComponent<Image>();
        buttonImage.sprite = FightButtons[0];
    }

    public void updateSprite(int id)
    {
        buttonImage.sprite = FightButtons[id];
    }
}
