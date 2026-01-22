using UnityEngine;
using System.Collections;

/// Banan:
/// - leci z enemy do gracza,
/// - zmniejsza się,
/// - obraca się,
/// - po dojściu do gracza odpala odrzut gracza i znika.
public class BananaProjectile : MonoBehaviour
{
    public float travelTime = 0.5f;    // czas lotu do gracza
    public float minScale = 0.3f;      // docelowy rozmiar
    public float rotationSpeed = 360f; // stopnie na sekundę

    private Transform target;

    public void Init(Transform targetTransform)
    {
        target = targetTransform;
        if (target != null)
        {
            StartCoroutine(Fly());
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private IEnumerator Fly()
    {
        Vector3 startPos = transform.position;
        Vector3 endPos = target.position;

        Vector3 startScale = transform.localScale;
        Vector3 endScale = startScale * minScale;

        float t = 0f;

        while (t < 1f)
        {
            t += Time.deltaTime / travelTime;
            float lerp = Mathf.Clamp01(t);

            transform.position = Vector3.Lerp(startPos, endPos, lerp);
            transform.localScale = Vector3.Lerp(startScale, endScale, lerp);
            transform.Rotate(0f, 0f, rotationSpeed * Time.deltaTime);

            yield return null;
        }

        // Trafienie w gracza → odrzut gracza W LEWO (enemy jest po lewej)
        HitHop hop = target.GetComponent<HitHop>();
        if (hop != null)
        {
            hop.Play(-1f); // gracz w lewo
        }

        Destroy(gameObject);
    }
}
