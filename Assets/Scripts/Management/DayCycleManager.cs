using System.Collections;
using UnityEngine;

public class DayCycleManager : MonoBehaviour
{
    public Animator CircusAnimator;
    public Animator BackgroundAnimator;
    public Animator BoardAnimator;
    private void Start()
    {
        StartCoroutine(SetDayWithDelay(3));
    }
    public void SetDay()
    {
        CircusAnimator.SetTrigger("SetDay");
        BackgroundAnimator.SetTrigger("SetDay");
    }
    public void SetNight()
    {
        CircusAnimator.SetTrigger("SetNight");
        BackgroundAnimator.SetTrigger("SetNight");
    }
    private IEnumerator SetDayWithDelay(int delay)
    {
        yield return new WaitForSeconds(delay);
        SetDay();
        yield return new WaitForSeconds(7);
        ShowBoard();
    }
    private void ShowBoard()
    {
        BoardAnimator.SetTrigger("Show");
    }

}
