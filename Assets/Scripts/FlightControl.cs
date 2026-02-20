using UnityEngine;

public class FlightControl : MonoBehaviour
{
    public FlightJoystick joystick;
    public Rigidbody planeRb;

    [Header("Скорость поворота")]
    public float pitchSpeed = 15f;          // Тангаж (нос вверх/вниз)
    public float rollSpeed = 20f;           // Крен (наклон крыльев влево/вправо)
    public float autoYawFromRoll = 5f;      // Авто-рысканье от крена (поворот носа за креном)

    [Header("Ограничения")]
    public float maxAngularSpeed = 2f;      // Макс. угловая скорость (рад/с)
    public float angularDamping = 3f;       // Затухание вращения при отпускании джойстика
    public float rollReturnSpeed = 2f;      // Скорость возврата крыльев в ровное положение

    bool controlActive = false;

    public void EnableControl()
    {
        controlActive = true;
        if (joystick != null) joystick.EnableJoystick();
    }

    public void DisableControl()
    {
        controlActive = false;
        if (joystick != null) joystick.DisableJoystick();
    }

    void FixedUpdate()
    {
        if (!controlActive || joystick == null || planeRb == null) return;

        Vector2 input = joystick.InputDirection;

        if (input.sqrMagnitude > 0.01f)
        {
            // === PITCH — джойстик вверх/вниз → нос вверх/вниз ===
            // Инвертирован: тянем джойстик вниз — нос поднимается (как в авиасимуляторах)
            Vector3 pitchTorque = planeRb.transform.right * (input.y * pitchSpeed);

            // === ROLL — джойстик влево/вправо → крен крыльев ===
            // Вправо: правое крыло опускается, левое поднимается
            // Минус потому что положительный roll по forward = по часовой (правое крыло вниз)
            Vector3 rollTorque = -planeRb.transform.forward * (input.x * rollSpeed);

            // === АВТО-YAW — нос автоматически поворачивается за креном ===
            // Когда крылья наклонены, самолёт естественно поворачивает в сторону крена
            // Вычисляем текущий крен — угол между локальным up и мировым up
            // Проецируем мировой up на плоскость forward самолёта
            float currentRoll = GetCurrentRoll();
            Vector3 yawTorque = planeRb.transform.up * (-currentRoll / 90f * autoYawFromRoll);

            planeRb.AddTorque(pitchTorque + rollTorque + yawTorque, ForceMode.Acceleration);
        }
        else
        {
            // === АВТО-ВЫРАВНИВАНИЕ КРЫЛЬЕВ при отпущенном джойстике ===
            // Самолёт плавно возвращает крылья в горизонтальное положение
            float currentRoll = GetCurrentRoll();
            if (Mathf.Abs(currentRoll) > 1f) // Мёртвая зона 1 градус
            {
                Vector3 rollCorrection = planeRb.transform.forward * (currentRoll / 90f * rollReturnSpeed);
                planeRb.AddTorque(rollCorrection, ForceMode.Acceleration);
            }

            // Авто-yaw продолжает работать пока есть крен
            if (Mathf.Abs(currentRoll) > 5f)
            {
                Vector3 yawTorque = planeRb.transform.up * (-currentRoll / 90f * autoYawFromRoll);
                planeRb.AddTorque(yawTorque, ForceMode.Acceleration);
            }
        }

        // Затухание вращения — самолёт плавно прекращает вращаться
        planeRb.angularVelocity = Vector3.Lerp(
            planeRb.angularVelocity,
            Vector3.zero,
            Time.fixedDeltaTime * angularDamping);

        // Ограничение максимальной угловой скорости
        if (planeRb.angularVelocity.magnitude > maxAngularSpeed)
        {
            planeRb.angularVelocity = planeRb.angularVelocity.normalized * maxAngularSpeed;
        }
    }

    /// <summary>
    /// Вычисляет текущий угол крена самолёта в градусах.
    /// Положительный = правое крыло внизу, отрицательный = левое крыло внизу.
    /// </summary>
    float GetCurrentRoll()
    {
        // Проецируем мировой вектор "вверх" на плоскость, перпендикулярную forward
        Vector3 flatUp = Vector3.ProjectOnPlane(Vector3.up, planeRb.transform.forward).normalized;
        // Угол между локальным up самолёта и мировым up (в плоскости forward)
        float roll = Vector3.SignedAngle(flatUp, planeRb.transform.up, planeRb.transform.forward);
        return roll;
    }
}