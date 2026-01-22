using UnityEngine;
using System.Collections;

/// Prefab laski:
/// - instancjonowany przy ataku jako dziecko gracza,
/// - lekko podnosi się w górę i w prawo,
/// - robi swing przez rotację,
/// - na końcu się zmniejsza i znika.
public class PlayerStaffProjectile : MonoBehaviour
{
    [Header("Movement")]
    public float upOffset = 0.3f;      // jak wysoko nad rękę
    public float rightOffset = 0.4f;   // jak bardzo w prawo od gracza
    public float moveUpTime = 0.1f;    // czas dojścia do pozycji wyjściowej swingu

    [Header("Rotation swing")]
    public float startAngle = -60f;    // kąt początkowy zamachu
    public float endAngle = 20f;       // kąt końcowy zamachu
    public float swingTime = 0.15f;    // czas rotacji

    [Header("Fade out / scale")]
    public float fadeTime = 0.1f;      // czas zaniku
    public float endScaleFactor = 0.4f;// docelowa skala

    private Vector3 startLocalPos;
    private Vector3 startLocalScale;

    private void Awake()
    {
        startLocalPos = transform.localPosition;
        startLocalScale = transform.localScale;
    }

    /// <summary>
    /// Wywołaj po Instantiate – podaj playera (parent).
    /// </summary>
    public void Init(Transform player)
    {
        if (player == null)
        {
            Destroy(gameObject);
            return;
        }

        StartCoroutine(SwingRoutine(player));
    }

    private IEnumerator SwingRoutine(Transform player)
    {
        // ustaw jako dziecko gracza i bazową pozycję
        transform.SetParent(player);
        transform.localPosition = startLocalPos;
        transform.localScale = startLocalScale;

        // pozycja, do której laska ma się „ustawić” przed swingiem:
        // trochę w górę i w prawo względem gracza
        Vector3 targetLocalPos = startLocalPos
                               + Vector3.up * upOffset
                               + Vector3.right * rightOffset;

        // 1. Przejście do pozycji startowej swingu (góra + prawo)
        Vector3 fromPos = startLocalPos;
        Vector3 toPos = targetLocalPos;

        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime / moveUpTime;
            float lerp = Mathf.Clamp01(t);
            transform.localPosition = Vector3.Lerp(fromPos, toPos, lerp);
            yield return null;
        }

        // 2. Swing przez rotację wokół osi Z
        Quaternion fromRot = Quaternion.Euler(0f, 0f, startAngle);
        Quaternion toRot = Quaternion.Euler(0f, 0f, endAngle);

        transform.localRotation = fromRot;

        t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime / swingTime;
            float lerp = Mathf.Clamp01(t);
            transform.localRotation = Quaternion.Lerp(fromRot, toRot, lerp);
            yield return null;
        }

        // 3. Zanik – zmniejszanie i powrót rotacji do bazowej
        Vector3 fromScale = startLocalScale;
        Vector3 toScale = startLocalScale * endScaleFactor;
        Quaternion backRot = Quaternion.identity;

        t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime / fadeTime;
            float lerp = Mathf.Clamp01(t);

            transform.localScale = Vector3.Lerp(fromScale, toScale, lerp);
            transform.localRotation = Quaternion.Lerp(toRot, backRot, lerp);

            yield return null;
        }

        Destroy(gameObject);
    }
}
