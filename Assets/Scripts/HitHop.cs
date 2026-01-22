using UnityEngine;
using System.Collections;

/// Efekt odskoku przy trafieniu (do tyłu i powrót).
public class HitHop : MonoBehaviour
{
    public float hopDistance = 40f;   // jak daleko odskakuje
    public float hopTime = 0.15f;    // czas fazy odskoku
    public float returnTime = 0.15f; // czas powrotu

    private bool isPlaying = false;

    /// <summary>
    /// direction = +1f (w prawo), -1f (w lewo) względem osi X.
    /// </summary>
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
