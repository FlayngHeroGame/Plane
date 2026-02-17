using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlaneLaunch : MonoBehaviour
{
    public Transform slingOrigin;   // Точка крепления рогатки
    public float launchPower = 20f;

    Rigidbody rb;

    enum LaunchState { Ready, Pulling, Flying }
    LaunchState state = LaunchState.Ready;

    public float maxAngle = 45f; // Половина допустимого конуса в градусах
    public float minPull = 2f;   // Минимальная тяга
    public float maxPull = 10f;  // Максимальная тяга

    Vector3 pullVector;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.isKinematic = true; // Пока не запущен
    }

    void Update()
    {
        // Универсальная обработка ввода для мобильных и десктопа
        // Приоритет тачу, если есть активные касания
        if (Input.touchCount > 0)
        {
            var t = Input.GetTouch(0);
            
            if (t.phase == TouchPhase.Began) StartDrag();
            if (t.phase == TouchPhase.Moved) Drag(t.position);
            if (t.phase == TouchPhase.Ended) Release();
        }
        else
        {
            // Fallback на мышь для десктопа или когда нет касаний
            if (Input.GetMouseButtonDown(0)) StartDrag();
            if (Input.GetMouseButton(0)) Drag(Input.mousePosition);
            if (Input.GetMouseButtonUp(0)) Release();
        }
    }

    void StartDrag()
    {
        if (state != LaunchState.Ready) return;
        state = LaunchState.Pulling;
    }

    void Drag(Vector2 screenPos)
    {
        if (state != LaunchState.Pulling) return;

        Ray ray = Camera.main.ScreenPointToRay(screenPos);
        Plane plane = new Plane(Vector3.up, slingOrigin.position);

        if (plane.Raycast(ray, out float dist))
        {
            Vector3 point = ray.GetPoint(dist);

            pullVector = slingOrigin.position - point;

            // Ограничение максимальной силы
            pullVector = Vector3.ClampMagnitude(pullVector, maxPull);

            // Ограничение направления углом
            pullVector = ClampDirection(pullVector.normalized) * pullVector.magnitude;

            transform.position = slingOrigin.position - pullVector;
            transform.forward = pullVector.normalized;
        }
    }

    void Release()
    {
        if (state != LaunchState.Pulling) return;

        float pullLength = pullVector.magnitude;

        // Если натяжение слишком слабое, возврат в начальное состояние
        if (pullLength < minPull)
        {
            ResetToStart();
            transform.position = slingOrigin.position;
            pullVector = Vector3.zero;
            return;
        }

        // Переход в состояние полёта только после успешной проверки
        state = LaunchState.Flying;

        float normalized = Mathf.InverseLerp(minPull, maxPull, pullLength);
        float force = launchPower * normalized * UpgradeSystem.Instance.SlingPower;

        rb.isKinematic = false;
        rb.AddForce(pullVector.normalized * force, ForceMode.Impulse);

        FindObjectOfType<SlingshotVisual>().OnLaunch();
    }

    Vector3 ClampDirection(Vector3 dir)
    {
        Vector3 baseDir = -slingOrigin.forward; // Вектор назад от рогатки (инвертирован)

        float angle = Vector3.Angle(baseDir, dir);

        if (angle <= maxAngle)
            return dir;

        float t = maxAngle / angle;
        return Vector3.Slerp(baseDir, dir, t);
    }

    void ResetToStart()
    {
        pullVector = Vector3.zero;

        transform.position = slingOrigin.position;
        transform.rotation = slingOrigin.rotation;

        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        rb.isKinematic = true;

        state = LaunchState.Ready;
    }
}
