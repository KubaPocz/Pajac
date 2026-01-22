using UnityEngine;
using System.Collections;

public class HitHop : MonoBehaviour
{
    public float hopDistance = 40f;
    public float hopTime = 0.15f;
    public float returnTime = 0.15f;

    private bool isPlaying = false;

    public void Play(float direction)
    {
        if (!isPlaying)
            StartCoroutine(HopRoutine(direction));
    }

    private IEnumerator HopRoutine(float direction)
    {
        isPlaying = true;

        Vector3 start = transform.position;
        Vector3 forward = start + Vector3.right * direction * hopDistance;

        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime / hopTime;
            transform.position = Vector3.Lerp(start, forward, t);
            yield return null;
        }

        t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime / returnTime;
            transform.position = Vector3.Lerp(forward, start, t);
            yield return null;
        }

        transform.position = start;
        isPlaying = false;
    }
}
