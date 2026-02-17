using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlaneLaunch : MonoBehaviour
{
    public Transform slingOrigin;   // ����� ��������� �������
    public float launchPower = 20f;

    Rigidbody rb;

    bool isDragging = false;
    bool launched = false;

    public float maxAngle = 45f; // �������� ���������� � ��������
    public float minPull = 2f;   // ����������� ����
    public float maxPull = 10f;  // ������������ ����

    Vector3 pullVector;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.isKinematic = true; // ���� �� �������
    }

    void Update()
    {
        if (!isDragging && rb.isKinematic)
        {
            if (Input.GetMouseButtonDown(0))
                isDragging = true;
        }

#if UNITY_EDITOR
        if (Input.GetMouseButtonDown(0)) StartDrag();
        if (Input.GetMouseButton(0)) Drag(Input.mousePosition);
        if (Input.GetMouseButtonUp(0)) Release();
#else
        if (Input.touchCount > 0)
        {
            var t = Input.GetTouch(0);

            if (t.phase == TouchPhase.Began) StartDrag();
            if (t.phase == TouchPhase.Moved) Drag(t.position);
            if (t.phase == TouchPhase.Ended) Release();
        }
#endif
    }

    void StartDrag()
    {
        isDragging = true;
    }

    void Drag(Vector2 screenPos)
    {
        if (!isDragging) return;

        Ray ray = Camera.main.ScreenPointToRay(screenPos);
        Plane plane = new Plane(Vector3.up, slingOrigin.position);

        if (plane.Raycast(ray, out float dist))
        {
            Vector3 point = ray.GetPoint(dist);

            /*pullVector = slingOrigin.position - point;
            pullVector = Vector3.ClampMagnitude(pullVector, maxPull);

            transform.position = slingOrigin.position - pullVector;
            transform.forward = pullVector.normalized;*/
            pullVector = slingOrigin.position - point;

            // ������� ������������ �����
            pullVector = Vector3.ClampMagnitude(pullVector, maxPull);

            // ����� ������������ ����
            pullVector = ClampDirection(pullVector.normalized) * pullVector.magnitude;

            float pullLength = pullVector.magnitude;

            // ������������ ��������
            pullLength = Mathf.Min(pullLength, maxPull);

            // ������������ ������
            pullVector = pullVector.normalized * pullLength;


            transform.position = slingOrigin.position - pullVector;
            transform.forward = pullVector.normalized;

        }


    }

    void Release()
    {
        if (!isDragging) return;

        isDragging = false;
        launched = true;

        float pullLength = pullVector.magnitude;

        // ���� �������� ������� ����� � �������� �������
        if (pullLength < minPull)
        {
            ResetToStart();

            transform.position = slingOrigin.position;
            pullVector = Vector3.zero;
            return;
        }

        float normalized = Mathf.InverseLerp(minPull, maxPull, pullLength);
        float force = launchPower * normalized;

        rb.isKinematic = false;
        //rb.AddForce(pullVector * launchPower, ForceMode.Impulse);
        rb.AddForce(pullVector.normalized * force, ForceMode.Impulse);

        FindObjectOfType<SlingshotVisual>().OnLaunch();

        // Включить камеру следования
        var cam = FindObjectOfType<FollowCamera>();
        if (cam != null) cam.StartFollowing();

        // Включить управление в полёте
        var fc = FindObjectOfType<FlightControl>();
        if (fc != null) fc.EnableControl();

    }

    Vector3 ClampDirection(Vector3 dir)
    {
        Vector3 baseDir = slingOrigin.forward; // ������ �� �������

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

        isDragging = false;
    }


}
