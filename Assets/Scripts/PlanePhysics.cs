using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlanePhysics : MonoBehaviour
{
    public float drag = 0.02f;
    public float lift = 0.5f;
    public float groundCheckDistance = 1.5f;    // Дистанция рейкаста для проверки земли
    
    Rigidbody rb;
    
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }
    
    void FixedUpdate()
    {
        if (rb.linearVelocity.magnitude < 0.1f) return;
        
        Vector3 v = rb.linearVelocity;
        
        // Сопротивление воздуха (всегда)
        rb.AddForce(-v * drag, ForceMode.Acceleration);
        
        // Проверка: самолёт на земле или в воздухе
        bool grounded = Physics.Raycast(transform.position, Vector3.down, groundCheckDistance);
        
        // Подъёмная сила — ТОЛЬКО в воздухе
        if (!grounded)
        {
            float forwardSpeed = Vector3.Dot(v, transform.forward);
            if (forwardSpeed > 0)
            {
                Vector3 liftForce = transform.up * forwardSpeed * lift * UpgradeSystem.Instance.LiftBonus;
                rb.AddForce(liftForce, ForceMode.Acceleration);
            }
        }
        
        // Ориентация по вектору скорости (всегда, но мягче на земле)
        float rotationSpeed = grounded ? 1f : 2f;
        rb.MoveRotation(
            Quaternion.Lerp(
                transform.rotation,
                Quaternion.LookRotation(v),
                Time.fixedDeltaTime * rotationSpeed));
    }
}
