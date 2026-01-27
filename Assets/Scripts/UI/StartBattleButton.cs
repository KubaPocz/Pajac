using UnityEngine;
using UnityEngine.UI;

public class StartBattleButton : MonoBehaviour
{
    public Sprite[] FightButtons;
    private Image buttonImage;
    void Start()
    {
        buttonImage = GetComponent<Image>();
        updateSprite(GameManager.Instance.CurrentEnemy);
    }

    public void updateSprite(int id)
    {
        Debug.Log(id);
        if(id<FightButtons.Length)
            buttonImage.sprite = FightButtons[id];
    }
}
