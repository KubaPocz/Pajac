using UnityEngine;
using System.Collections;

public class BananaProjectile : MonoBehaviour
{
    [Header("Movement")]
    public Transform target;             // gracz
    public float speed = 12f;            // prędkość lotu
    public float hitDistance = 0.5f;     // dystans trafienia
    public float spinSpeed = 360f;       // obrót (stopnie/sek)

    [Header("Scale shrink")]
    public float minScaleFactor = 0.4f;  // do jakiej skali zmniejsza się przy końcu

    [Header("Hit effect")]
    public GameObject splashPrefab;      // prefab SPLASH (Canvas -> SPLASH -> SplashRoot -> Image + Animator)

    private Vector3 startScale;
    private float totalDistance;

    public void Init(Transform t)
    {
        target = t;

        // zapamiętujemy skalę początkową i całkowity dystans
        startScale = transform.localScale;

        if (target != null)
            totalDistance = Vector3.Distance(transform.position, target.position);
        else
            totalDistance = 0f;
    }

    private void Update()
    {
        if (target == null) return;

        // ruch
        transform.position = Vector3.MoveTowards(
            transform.position,
            target.position,
            speed * Time.deltaTime
        );

        // obrót
        transform.Rotate(0f, 0f, spinSpeed * Time.deltaTime);

        // zmniejszanie skali w miarę zbliżania się
        if (totalDistance > 0f)
        {
            float currentDist = Vector3.Distance(transform.position, target.position);
            float t = Mathf.Clamp01(1f - (currentDist / totalDistance)); // 0 → start, 1 → przy celu

            float scaleFactor = Mathf.Lerp(1f, minScaleFactor, t);
            transform.localScale = startScale * scaleFactor;
        }

        // trafienie
        if (Vector3.Distance(transform.position, target.position) <= hitDistance)
        {
            OnHit();
        }
    }

    private void OnHit()
    {
        // 1. Splash jako dziecko gracza
        if (splashPrefab != null && target != null)
        {
            GameObject splash = Instantiate(
                splashPrefab,
                target.position,
                Quaternion.identity,
                target
            );

            Animator anim = splash.GetComponentInChildren<Animator>();
            if (anim != null)
                anim.Play(0, -1, 0f);

            Destroy(splash, 0.5f); // dopasuj do długości BananaSplash.anim
        }

        // 2. Knockback gracza
        if (target != null)
        {
            HitHop hop = target.GetComponent<HitHop>();
            if (hop != null)
                hop.Play(-1f);
        }

        // 3. Zniszcz banana
        Destroy(gameObject);
    }
}
