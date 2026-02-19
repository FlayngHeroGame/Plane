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
        
        // Проверка: самолёт на земле или в воздухе
        bool grounded = Physics.Raycast(transform.position, Vector3.down, groundCheckDistance);
        
        // Вся аэродинамика — ТОЛЬКО в воздухе
        // На земле: физика Unity сама управляет трением и ориентацией
        // Это позволяет самолёту нормально катиться по земле и заезжать на трамплин
        if (!grounded)
        {
            // Сопротивление воздуха — только в воздухе
            // На земле уже есть естественное трение поверхности, двойное замедление не нужно
            rb.AddForce(-v * drag, ForceMode.Acceleration);
            
            // Подъёмная сила
            float forwardSpeed = Vector3.Dot(v, transform.forward);
            if (forwardSpeed > 0)
            {
                Vector3 liftForce = transform.up * forwardSpeed * lift * UpgradeSystem.Instance.LiftBonus;
                rb.AddForce(liftForce, ForceMode.Acceleration);
            }
            
            // Ориентация по вектору скорости — только в воздухе
            // На земле физика сама управляет ориентацией для заезда на трамплин
            if (v.sqrMagnitude > 0.01f)
            {
                rb.MoveRotation(
                    Quaternion.Lerp(
                        transform.rotation,
                        Quaternion.LookRotation(v),
                        Time.fixedDeltaTime * 2f));
            }
        }
    }
}
