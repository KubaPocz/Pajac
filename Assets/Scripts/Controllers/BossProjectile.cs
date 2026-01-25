using UnityEngine;
using System.Collections;

public class BossProjectile : MonoBehaviour
{
    [Header("Movement")]
    public Transform target;
    public float speed = 14f;
    public float hitDistance = 0.5f;
    public float spinSpeed = 540f;

    [Header("Scale shrink")]
    public float minScaleFactor = 0.3f;

    [Header("Hit effect")]
    public GameObject splashPrefab;   // specjalny efekt bossa

    private Vector3 startScale;
    private float totalDistance;

    public void Init(Transform t)
    {
        target = t;

        startScale = transform.localScale;

        if (target != null)
            totalDistance = Vector3.Distance(transform.position, target.position);
        else
            totalDistance = 0f;
    }

    private void Update()
    {
        if (target == null) return;

        transform.position = Vector3.MoveTowards(
            transform.position,
            target.position,
            speed * Time.deltaTime
        );

        transform.Rotate(0f, 0f, spinSpeed * Time.deltaTime);

        if (totalDistance > 0f)
        {
            float currentDist = Vector3.Distance(transform.position, target.position);
            float t = Mathf.Clamp01(1f - (currentDist / totalDistance));
            float scaleFactor = Mathf.Lerp(1f, minScaleFactor, t);
            transform.localScale = startScale * scaleFactor;
        }

        if (Vector3.Distance(transform.position, target.position) <= hitDistance)
        {
            OnHit();
        }
    }

    private void OnHit()
    {
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

            Destroy(splash, 0.5f);
        }

        if (target != null)
        {
            HitHop hop = target.GetComponent<HitHop>();
            if (hop != null)
                hop.Play(-1f);
        }

        Destroy(gameObject);
    }
}
