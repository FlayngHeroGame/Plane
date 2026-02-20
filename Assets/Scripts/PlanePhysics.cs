using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlanePhysics : MonoBehaviour
{
    [Header("Аэродинамика")]
    public float drag = 0.02f;                  // Сопротивление воздуха
    public float lift = 0.5f;                   // Базовая подъёмная сила. подъёмная сила. Больше = дольше держится в воздухе.
    public float velocityAlignment = 3f;        // Как быстро скорость следует за носом самолёта. как быстро скорость следует за носом. Больше = резче повороты. Меньше = инертнее.
    public float inducedDrag = 0.5f;            // Дополнительное сопротивление при высоком угле атаки. торможение при боковом скольжении. Больше = сильнее теряет скорость в крутых манёврах.

    [Header("Гравитационна�� компенсация")]
    public float gravityCompensation = 0.3f;    // Часть гравитации, компенсируемая на скорости (0-1). сколько гравитации компенсируется на скорости. 0 = камень, 1 = почти невесомость.

    [Header("Наземная физика")]
    public float groundCheckDistance = 1.5f;
    public float groundAlignSpeed = 5f;

    Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        if (rb.linearVelocity.magnitude < 0.1f) return;

        Vector3 v = rb.linearVelocity;
        float speed = v.magnitude;
        Vector3 forward = transform.forward;

        // Проверка земли
        RaycastHit hit;
        bool grounded = Physics.Raycast(transform.position, Vector3.down, out hit, groundCheckDistance);

        if (!grounded)
        {
            // ============================================
            // === В ВОЗДУХЕ — АЭРОДИНАМИЧЕСКАЯ МОДЕЛЬ ===
            // ============================================

            // 1) Компонент скорости вдоль носа самолёта
            float forwardSpeed = Vector3.Dot(v, forward);

            // 2) ПЕРЕНАПРАВЛЕНИЕ СКОРОСТИ к направлению носа
            //    Это главное: скорость постепенно поворачивается
            //    туда куда смотрит нос. Чем быстрее летит — тем сильнее эффект.
            //    Джойстик крутит нос → скорость следует за носом → самолёт поворачивает.
            if (forwardSpeed > 0)
            {
                Vector3 desiredVelocity = forward * speed;
                Vector3 steeringForce = (desiredVelocity - v) * velocityAlignment;
                rb.AddForce(steeringForce, ForceMode.Acceleration);
            }

            // 3) ПОДЪЁМНАЯ СИЛА — зависит от скорости и направления носа
            //    Нос вверх → подъёмная сила больше → набор высоты, но теряем скорость
            //    Нос вниз → подъёмная сила меньше/отрицательная → пикирование, набираем скорость
            if (forwardSpeed > 0)
            {
                // Подъёмная сила пропорциональна скорости, направлена по up самолёта
                Vector3 liftForce = transform.up * forwardSpeed * lift * UpgradeSystem.Instance.LiftBonus;
                rb.AddForce(liftForce, ForceMode.Acceleration);
            }

            // 4) СОПРОТИВЛЕНИЕ ВОЗДУХА — базовое + индуцированное
            //    Индуцированное сопротивление: чем больше угол между носом и скоростью,
            //    тем больше торможение (самолёт боком к потоку — тормозит сильно)
            float angleOfAttack = Vector3.Angle(forward, v.normalized) / 90f; // 0..1
            float totalDrag = drag + inducedDrag * angleOfAttack * angleOfAttack;
            rb.AddForce(-v * totalDrag, ForceMode.Acceleration);

            // 5) Частичная компенсация гравитации на скорости
            //    Без этого самолёт падает слишком быстро даже на высокой скорости
            if (forwardSpeed > 5f)
            {
                float compensation = Mathf.Clamp01(forwardSpeed / 30f) * gravityCompensation;
                rb.AddForce(-Physics.gravity * compensation, ForceMode.Acceleration);
            }

            // НЕТ принудительного поворота по вектору скорости!
            // Нос контролируется джойстиком (FlightControl),
            // а скорость следует за носом через velocityAlignment.
        }
        else
        {
            // === НА ЗЕМЛЕ ===

            Vector3 surfaceNormal = hit.normal;
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