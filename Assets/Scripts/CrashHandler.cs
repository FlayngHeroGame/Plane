using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class CrashHandler : MonoBehaviour
{
    [Header("Префабы")]
    public GameObject explosionPrefab;
    public GameObject distanceMarkerPrefab;

    [Header("Настройки крушения")]
    public float hardLandingSpeed = 10f;
    public float hardLandingAngle = 45f;
    public float stopSpeedThreshold = 0.5f;
    public float markerOffsetForward = 2f;
    public float groundCheckDistance = 1.5f;

    [Header("Детали самолёта (для разлёта при крушении)")]
    public GameObject[] planeParts;
    public float partExplosionForce = 300f;
    public float partExplosionRadius = 5f;

    [Header("Визуал целого самолёта")]
    public GameObject planeVisual;

    Rigidbody rb;
    bool launched = false;
    bool crashed = false;
    bool stopped = false;
    bool wasInAir = false;
    float launchTime;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        SetPartsActive(false);
    }

    public void OnLaunched()
    {
        launched = true;
        launchTime = Time.time;
    }

    void FixedUpdate()
    {
        if (!launched || stopped || crashed) return;

        if (Time.time - launchTime < 3f)
        {
            bool isGrounded = Physics.Raycast(transform.position, Vector3.down, groundCheckDistance);
            if (!isGrounded)
                wasInAir = true;
            return;
        }

        bool grounded = Physics.Raycast(transform.position, Vector3.down, groundCheckDistance);

        if (!grounded)
            wasInAir = true;

        if (wasInAir && rb.linearVelocity.magnitude < stopSpeedThreshold)
        {
            stopped = true;
            StartCoroutine(StopSequence());
        }
    }

    void OnCollisionEnter(Collision col)
    {
        if (!launched || crashed) return;

        if (!wasInAir) return;

        float verticalSpeed = Mathf.Abs(rb.linearVelocity.y);
        float angle = Mathf.Abs(Vector3.Angle(transform.forward, Vector3.up) - 90f);

        if (verticalSpeed > hardLandingSpeed || angle > hardLandingAngle)
        {
            HardCrash();
        }
    }

    void HardCrash()
    {
        if (crashed) return;
        crashed = true;

        var fc = FindObjectOfType<FlightControl>();
        if (fc != null) fc.DisableControl();

        if (planeVisual != null) planeVisual.SetActive(false);
        SetPartsActive(true);

        foreach (var part in planeParts)
        {
            if (part == null) continue;

            part.transform.SetParent(null);

            Rigidbody partRb = part.GetComponent<Rigidbody>();
            if (partRb == null) partRb = part.AddComponent<Rigidbody>();

            if (part.GetComponent<Collider>() == null)
                part.AddComponent<BoxCollider>();

            partRb.AddExplosionForce(partExplosionForce, transform.position, partExplosionRadius);
        }

        if (explosionPrefab != null)
            Instantiate(explosionPrefab, transform.position, Quaternion.identity);

        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        rb.isKinematic = true;

        // После жёсткого крушения — тоже показать флажок и награду!
        StartCoroutine(StopSequence());
    }

    IEnumerator StopSequence()
    {
        var fc = FindObjectOfType<FlightControl>();
        if (fc != null) fc.DisableControl();

        if (!crashed)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            rb.isKinematic = true;
        }

        yield return new WaitForSeconds(1f);

        Vector3 markerPos = transform.position + Vector3.forward * markerOffsetForward;
        if (distanceMarkerPrefab != null)
            Instantiate(distanceMarkerPrefab, markerPos, Quaternion.identity);

        yield return new WaitForSeconds(1f);

        var rewardScreen = FindObjectOfType<RewardScreen>();
        if (rewardScreen != null)
            rewardScreen.Show();
    }

    void SetPartsActive(bool active)
    {
        if (planeParts == null) return;
        foreach (var part in planeParts)
        {
            if (part != null) part.SetActive(active);
        }
    }
}