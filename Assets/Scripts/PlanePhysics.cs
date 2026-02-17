using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlanePhysics : MonoBehaviour
{
    public float drag = 0.02f;      // ������������� �������
    public float lift = 0.5f;       // ��������� ����

    Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        if (rb.linearVelocity.magnitude < 0.1f) return;

        Vector3 v = rb.linearVelocity;

        // ������������� �������
        rb.AddForce(-v * drag, ForceMode.Acceleration);

        // ��������� ���� ��������������� ��������
        float forwardSpeed = Vector3.Dot(v, transform.forward);

        if (forwardSpeed > 0)
        {
            Vector3 liftForce = transform.up * forwardSpeed * lift;
            rb.AddForce(liftForce, ForceMode.Acceleration);
        }

        // ������ ������� �� ����������� ��������
        transform.rotation =
            Quaternion.Lerp(
                transform.rotation,
                Quaternion.LookRotation(v),
                Time.fixedDeltaTime * 2f);
    }
}
