using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlanePhysics : MonoBehaviour
{
    public float drag = 0.02f;
    public float lift = 0.5f;
    public float groundCheckDistance = 1.5f;
    public float groundAlignSpeed = 5f;     // Скорость выравнивания по поверхности
    
    Rigidbody rb;
    
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }
    
    void FixedUpdate()
    {
        if (rb.linearVelocity.magnitude < 0.1f) return;
        
        Vector3 v = rb.linearVelocity;
        
        // Проверка земли с получением нормали поверхности
        RaycastHit hit;
        bool grounded = Physics.Raycast(transform.position, Vector3.down, out hit, groundCheckDistance);
        
        if (!grounded)
        {
            // === В ВОЗДУХЕ ===
            
            // Сопротивление воздуха
            rb.AddForce(-v * drag, ForceMode.Acceleration);
            
            // Подъёмная сила
            float forwardSpeed = Vector3.Dot(v, transform.forward);
            if (forwardSpeed > 0)
            {
                Vector3 liftForce = transform.up * forwardSpeed * lift * UpgradeSystem.Instance.LiftBonus;
                rb.AddForce(liftForce, ForceMode.Acceleration);
            }
            
            // Ориентация по вектору скорости
            if (v.sqrMagnitude > 0.01f)
            {
                rb.MoveRotation(
                    Quaternion.Lerp(
                        transform.rotation,
                        Quaternion.LookRotation(v),
                        Time.fixedDeltaTime * 2f));
            }
        }
        else
        {
            // === НА ЗЕМЛЕ ===
            
            // Выравнивание по нормали поверхности
            // Это позволяет самолёту плавно заезжать на трамплин,
            // приподнимая нос по наклону рампы, вместо того чтобы
            // врезаться углами бокс-коллайдеров
            
            Vector3 surfaceNormal = hit.normal;
            
            // Направление вперёд проецируем на плоскость поверхности
            Vector3 forwardOnSurface = Vector3.ProjectOnPlane(transform.forward, surfaceNormal);
            
            if (forwardOnSurface.sqrMagnitude > 0.001f)
            {
                Quaternion targetRotation = Quaternion.LookRotation(forwardOnSurface, surfaceNormal);
                rb.MoveRotation(
                    Quaternion.Lerp(
                        transform.rotation,
                        targetRotation,
                        Time.fixedDeltaTime * groundAlignSpeed));
            }
        }
    }
}
